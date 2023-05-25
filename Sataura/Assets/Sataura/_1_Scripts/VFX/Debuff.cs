using UnityEngine;

namespace Sataura
{
    public class Debuff : MonoBehaviour
    {
        [SerializeField] private DebuffEffect _debuffEffect;
        [SerializeField] private ParticleSystem _ps;

        public DebuffEffect _DebuffEffect { get => _debuffEffect; }
        public ParticleSystem _Ps { get => _ps; }

    }
}

