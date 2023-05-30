using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "ScopeLens", menuName = "Sataura/Item/Accessories/ScopeLens", order = 51)]
    public class ScopeLens : ItemData
    {
        [Header("ScopeLens Data")]
        public int awarePercentIncrease;
    }
}