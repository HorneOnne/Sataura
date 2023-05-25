using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "AmplifyingBand", menuName = "Sataura/Item/Accessories/AmplifyingBand", order = 51)]
    public class AmplifyingBand : ItemData
    {
        [Header("AmplifyingBand Data")]
        public int areaPercentIncrease;
    }
}