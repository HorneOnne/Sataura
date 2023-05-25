using UnityEngine;

namespace Sataura
{
    public class SnowSlashVFX : MonoBehaviour
    {
        public ParticleSystem _mainPs;
        public ParticleSystem _childPs;

  
        public void ChangeSnowSlashVFXStartLifetime(float newLifetime)
        {
            ParticleSystem.MainModule mainModule = _mainPs.main;
            mainModule.startLifetime = newLifetime;

            ParticleSystem.MainModule childModule = _childPs.main;
            childModule.startLifetime = newLifetime;
        }
    }
}

