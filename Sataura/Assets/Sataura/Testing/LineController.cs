using UnityEngine;

namespace Sataura
{
    public class LineController : MonoBehaviour
    {
        [SerializeField] private LineRenderer lr;
        [SerializeField] private Texture[] textures;

        private int animationStep;

        [SerializeField] private float fps = 30f;
        private float fpsCounter = 0f;

        [SerializeField] private Transform target;
        private Camera mainCamera;

        private void Start()
        {
            lr = GetComponent<LineRenderer>();
            mainCamera = Camera.main;

            lr.positionCount = 2;
            lr.SetPosition(0, transform.position);
        }

        private void Update()
        {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            lr.SetPosition(1, new Vector3(mousePos.x, mousePos.y, 0));


            fpsCounter += Time.deltaTime;
            if (fpsCounter >= 1 / fps)
            {
                animationStep++;
                if(animationStep == textures.Length)
                {
                    animationStep = 0;
                }

                lr.material.SetTexture("_MainTex", textures[animationStep]);

                fpsCounter = 0.0f;
            }

        }

    }

}
