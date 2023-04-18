using UnityEngine;

namespace Sataura
{
    public class BloodTearVFX: MonoBehaviour
    {
        public enum VFXState
        {
            Start,
            Loop,
            End
        }

        [SerializeField] private Sprite[] startSprites;
        [SerializeField] private Sprite[] loopSprites;
        [SerializeField] private Sprite[] endSprites;

        private SpriteRenderer sr;
        [SerializeField] private float fps = 30f;
        private float fpsCounter = 0f;
        private int animationStep = 0;

        private VFXState vfxState;

        private void Start()
        {
            vfxState= VFXState.Start;
            sr = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
   

            if(Input.GetKeyDown(KeyCode.E))
            {
                ChangeVFXState(VFXState.End);
            }

            switch(vfxState)
            {
                case VFXState.Start:
                    StartVFX();
                    break;
                case VFXState.Loop:
                    LoopVFX();
                    break;
                case VFXState.End:
                    EndVFX();
                    break;
            }
          
        }


        private void StartVFX()
        {
            fpsCounter += Time.deltaTime;
            if (fpsCounter >= 1 / fps)
            {
                animationStep++;
                if (animationStep == startSprites.Length)
                {
                    animationStep = 0;
                    ChangeVFXState(VFXState.Loop);
                }

                sr.sprite = startSprites[animationStep];
                fpsCounter = 0.0f;
            }
        }

        private void LoopVFX()
        {
            fpsCounter += Time.deltaTime;
            if (fpsCounter >= 1 / fps)
            {
                animationStep++;
                if (animationStep == loopSprites.Length)
                {
                    animationStep = 0;
                    //ChangeVFXState(VFXState.End);
                }

                sr.sprite = loopSprites[animationStep];
                fpsCounter = 0.0f;
            }
        }

        private void EndVFX()
        {
            fpsCounter += Time.deltaTime;
            if (fpsCounter >= 1 / fps)
            {
                animationStep++;
                if (animationStep == endSprites.Length)
                {
                    animationStep = 0;
                    Destroy(this.gameObject);
                }

                sr.sprite = endSprites[animationStep];
                fpsCounter = 0.0f;
            }
        }


        public void ChangeVFXState(VFXState vfxState)
        {
            this.vfxState= vfxState;
        }

    }
}

