using UnityEngine;

namespace Sataura
{
    [System.Serializable]
    public struct ItemDrop
    {
        public ItemSlot itemDrop;
        [Range(0f, 100f)]
        public float dropRate;
    }

}
