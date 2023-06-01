﻿using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "HookData", menuName = "Sataura/Item/Equipment/HookData", order = 51)]
    public class HookData : ItemData
    {
        [Header("Hook Data")]
        public float reach;
        public float velocity;
    }
}