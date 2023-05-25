using UnityEngine;

namespace Sataura
{
    public class BlackHoldVFX : MonoBehaviour
    {
        public ParticleSystem _corePs;
        public ParticleSystem _glowPs;
        public ParticleSystem _sparkPs;



        public void ChangeStartSize(float scaleFactor)
        {
            ParticleSystem.MainModule coreModule = _corePs.main;
            float newStartSize = _corePs.main.startSize.constant * scaleFactor;
            coreModule.startSize = newStartSize;

            ParticleSystem.MainModule glowModule = _glowPs.main;
            newStartSize = _glowPs.main.startSize.constant * scaleFactor;
            glowModule.startSize = newStartSize;

            ParticleSystem.MainModule sparkModule = _sparkPs.main;
            newStartSize = _sparkPs.main.startSize.constant * scaleFactor;
            sparkModule.startSize = newStartSize;
        }

        public void StopVFX()
        {
            if(_corePs.isPlaying || _corePs.isEmitting == true)
            {
                _corePs.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }

            if (_glowPs.isPlaying || _glowPs.isEmitting == true)
            {
                _glowPs.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }


            if (_sparkPs.isPlaying || _sparkPs.isEmitting == true)
            {
                _sparkPs.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }
}

