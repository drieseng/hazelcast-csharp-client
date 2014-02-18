using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hazelcast.Client.Request.Partition;
using Hazelcast.Core;
using Hazelcast.IO;
using Hazelcast.IO.Serialization;
using Hazelcast.Logging;
using Hazelcast.Net.Ext;

namespace Hazelcast.Client.Spi
{
    internal sealed class ClientPartitionService : IClientPartitionService
    {
        private static readonly ILogger logger = Logger.GetLogger(typeof (IClientPartitionService));

        private readonly HazelcastClient client;

        private readonly ConcurrentDictionary<int, Address> partitions = new ConcurrentDictionary<int, Address>();

        private readonly AtomicBoolean updating = new AtomicBoolean(false);

        private volatile int partitionCount;

        private Thread partitionThread;

        public ClientPartitionService(HazelcastClient client)
        {
            this.client = client;
        }

        #region IClientPartitionService

        public Address GetPartitionOwner(int partitionId)
        {
            Address rtn;
            partitions.TryGetValue(partitionId, out rtn);
            return rtn;
        }

        internal int GetPartitionId(Data key)
        {
            int pc = partitionCount;
            if (pc <= 0)
            {
                return 0;
            }
            int hash = key.GetPartitionHash();
            return (hash == int.MinValue) ? 0 : Math.Abs(hash)%pc;
        }

        public int GetPartitionId(object key)
        {
            Data data = client.GetSerializationService().ToData(key);
            return GetPartitionId(data);
        }

        public int GetPartitionCount()
        {
            return partitionCount;
        }

        #endregion

        public void Start()
        {
            GetInitialPartitions();

            partitionThread = new Thread(RefreshPartitionsWithFixedDelay) {IsBackground = true};
            partitionThread.Start();
        }

        public void Stop()
        {
            try
            {
                partitionThread.Abort();
            }
            catch(Exception e)
            {
                logger.Finest("Shut down partition refresher thread problem...");
            }
            partitions.Clear();

        }

        public void RefreshPartitions()
        {
            partitionThread.Interrupt();
        }

        private void RefreshPartitionsWithFixedDelay()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                try
                {
                    __RefreshPartitions();
                    Thread.Sleep(10000);
                }
                catch (ThreadInterruptedException)
                {
                    logger.Finest("Partition Refresher thread wakes up");
                }
            }
        }

        private void __RefreshPartitions()
        {
            logger.Finest("Refresh Partitions at " + DateTime.Now.ToLocalTime());
            if (updating.CompareAndSet(false, true))
            {
                try
                {
                    IClientClusterService clusterService = client.GetClientClusterService();
                    Address master = clusterService.GetMasterAddress();
                    PartitionsResponse response = GetPartitionsFrom((ClientClusterService) clusterService, master);
                    if (response != null)
                    {
                        ProcessPartitionResponse(response);
                    }
                }
                catch (HazelcastInstanceNotActiveException)
                {
                }
                catch (Exception e)
                {
                    logger.Warning(e);
                }
                finally
                {
                    updating.Set(false);
                }
            }
        }

        private void GetInitialPartitions()
        {
            IClientClusterService clusterService = client.GetClientClusterService();
            ICollection<IMember> memberList = clusterService.GetMemberList();
            foreach (IMember member in memberList)
            {
                Address target = member.GetAddress();
                PartitionsResponse response = GetPartitionsFrom((ClientClusterService) clusterService, target);
                if (response != null)
                {
                    ProcessPartitionResponse(response);

                    return;
                }
            }
            throw new InvalidOperationException("Cannot get initial partitions!");
        }

        private PartitionsResponse GetPartitionsFrom(ClientClusterService clusterService, Address address)
        {
            try
            {
                Task<PartitionsResponse> task =
                    client.GetInvocationService()
                        .InvokeOnTarget<PartitionsResponse>(new GetPartitionsRequest(), address);
                PartitionsResponse partitionsResponse = task.Result;
                return client.GetSerializationService().ToObject<PartitionsResponse>(partitionsResponse);
            }
            catch (Exception e)
            {
                logger.Severe("Error while fetching cluster partition table!", e);
            }
            return null;
        }

        private void ProcessPartitionResponse(PartitionsResponse response)
        {
            Address[] members = response.GetMembers();
            int[] ownerIndexes = response.GetOwnerIndexes();
            if (partitionCount == 0)
            {
                partitionCount = ownerIndexes.Length;
            }
            partitions.Clear();
            for (int partitionId = 0; partitionId < partitionCount; partitionId++)
            {
                int ownerIndex = ownerIndexes[partitionId];
                if (ownerIndex > -1)
                {
                    partitions.TryAdd(partitionId, members[ownerIndex]);
                }
            }
        }
    }


    internal class Partition:IPartition 
    {

        private readonly int partitionId;
        private readonly HazelcastClient client;

        public Partition(HazelcastClient client, int partitionId)
        {
            this.client = client;
            this.partitionId = partitionId;
        }

        public int GetPartitionId()
        {
            return partitionId;
        }

        public IMember GetOwner()
        {
            Address owner = client.GetPartitionService().GetPartitionOwner(partitionId);
            if (owner != null) {
                return client.GetClientClusterService().GetMember(owner);
            }
            return null;
        }

        public override string ToString() {
            var sb = new StringBuilder("PartitionImpl{");
            sb.Append("partitionId=").Append(partitionId);
            sb.Append('}');
            return sb.ToString();
        }
    }

}