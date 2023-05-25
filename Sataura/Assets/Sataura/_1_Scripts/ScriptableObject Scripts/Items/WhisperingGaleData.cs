using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "WhisperingGale", menuName = "Sataura/Item/Weapons/WhisperingGale", order = 51)]
    public class WhisperingGaleData : ItemData
    {
        [Header("WhisperingGale Data")]
        public int damage;
        public float size = 1;
        public float timeExist = 1.0f;

        [Header("UseType")]
        public int useType = 1;     
    }
}
