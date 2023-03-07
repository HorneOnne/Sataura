﻿using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "New Sword Object", menuName = "UltimateItemSystem/Item/Equipment/Helm", order = 51)]
    public class HelmData : UpgradeableItemData
    {
        [Header("Helm Properties")]
        public int armor;
    }
}
