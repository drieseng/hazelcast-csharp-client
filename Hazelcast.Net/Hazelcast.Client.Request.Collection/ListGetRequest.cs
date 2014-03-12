using Hazelcast.IO.Serialization;
using Hazelcast.Serialization.Hook;

namespace Hazelcast.Client.Request.Collection
{
    internal class ListGetRequest : CollectionRequest
    {
        internal int index = -1;


        public ListGetRequest(string name, int index) : base(name)
        {
            this.index = index;
        }

        public override int GetClassId()
        {
            return CollectionPortableHook.ListGet;
        }

        /// <exception cref="System.IO.IOException"></exception>
        public override void WritePortable(IPortableWriter writer)
        {
            base.WritePortable(writer);
            writer.WriteInt("i", index);
        }

    }
}