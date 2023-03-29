using UnityEngine;
using UnityEngine.UI;

namespace Sataura
{
    [CreateAssetMenu(fileName = "UpgradeItemSkillData", menuName = "Sataura/UpgradeItemSkillData", order = 51)]
    public class UpgradeItemSkillData : ScriptableObject
    {
        public ItemType itemType;

        [Header("UI Infomation")]
        public Sprite icon;
        public string itemSkillName;
        [Multiline(7)]
        public string itemSkillDesc;

    }
}
