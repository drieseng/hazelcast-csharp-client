using Hazelcast.IO;
using Hazelcast.IO.Serialization;
using Hazelcast.Serialization.Hook;


namespace Hazelcast.Client.Request.Map
{
	public class MapDeleteRequest : IPortable
	{
		protected internal string name;

		protected internal Data key;

		protected internal int threadId;

		public MapDeleteRequest()
		{
		}

		public MapDeleteRequest(string name, Data key, int threadId)
		{
			this.name = name;
			this.key = key;
			this.threadId = threadId;
		}

		public virtual int GetFactoryId()
		{
			return MapPortableHook.FId;
		}

		public virtual int GetClassId()
		{
			return MapPortableHook.Delete;
		}

		protected internal virtual object GetKey()
		{
			return key;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void WritePortable(IPortableWriter writer)
		{
			writer.WriteUTF("n", name);
			writer.WriteInt("t", threadId);
			IObjectDataOutput output = writer.GetRawDataOutput();
			key.WriteData(output);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void ReadPortable(IPortableReader reader)
		{
			name = reader.ReadUTF("n");
			threadId = reader.ReadInt("t");
			IObjectDataInput input = reader.GetRawDataInput();
			key = new Data();
			key.ReadData(input);
		}
	}
}
