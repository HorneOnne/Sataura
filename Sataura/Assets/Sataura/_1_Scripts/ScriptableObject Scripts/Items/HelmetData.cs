using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "HelmetData", menuName = "Sataura/Item/Equipment/HelmetData", order = 51)]
    public class HelmetData : ItemData
    {
        [Header("Helm Properties")]
        public int armor;
    }
}
