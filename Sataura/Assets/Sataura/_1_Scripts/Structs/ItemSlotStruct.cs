using Unity.Netcode;

namespace Sataura
{
    [System.Serializable]
    public struct ItemSlotStruct : INetworkSerializable
    {
        public int itemID;
        public int itemQuantity;

        public ItemSlotStruct(int itemID, int itemQuantity)
        {
            this.itemID = itemID;
            this.itemQuantity = itemQuantity;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref itemID);
            serializer.SerializeValue(ref itemQuantity);
        }
    }
}
