﻿using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "Arrow", menuName = "UltimateItemSystem/Item/Projectiles/Arrow", order = 51)]
    public class ArrowData : ItemData
    {
        [Header("Bow Properties")]
        public int damage;
        [Tooltip("Index for getting particles frames in ProjectileParticleDataManager")]
        public int particle;
    }
}