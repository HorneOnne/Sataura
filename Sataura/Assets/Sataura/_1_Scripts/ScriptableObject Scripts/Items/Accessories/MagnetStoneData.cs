using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "MagnetStone", menuName = "Sataura/Item/MagnetStoneData", order = 51)]
    public class MagnetStoneData : ItemData
    {
        [Header("MagnetStone Properties")]
        public int lootPercentIncrease;
    }
}
