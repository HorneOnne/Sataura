using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "SwiftStriders", menuName = "Sataura/Item/Accessories/SwiftStriders", order = 51)]
    public class SwiftStriders : ItemData
    {
        [Header("SwiftStriders Data")]
        public int moveSpeedPercentIncrease;
        public int jumpForcePercentIncrease;
    }


}