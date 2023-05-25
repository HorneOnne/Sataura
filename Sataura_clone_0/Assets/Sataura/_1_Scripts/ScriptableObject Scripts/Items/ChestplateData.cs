﻿using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "ChestplateData", menuName = "Sataura/Item/Equipment/ChestplateData", order = 51)]
    public class ChestplateData : ItemData
    {
        [Header("ChestplateData Data")]
        public int armor;
    }
}