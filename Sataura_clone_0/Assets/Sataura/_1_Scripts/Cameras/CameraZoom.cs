using UnityEngine;
using Cinemachine;

namespace Sataura
{
    public class CameraZoom : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private float zoomSpeed = 0.1f; // Zoom speed of the camera
        [SerializeField] private float minZoom = 2f; // Minimum zoom distance
        [SerializeField] private float maxZoom = 10f; // Maximum zoom distance

        private float currentZoom = 0f;


        void Start()
        {
            currentZoom = virtualCamera.m_Lens.OrthographicSize;
        }

        void Update()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll != 0)
            {
                currentZoom -= scroll * zoomSpeed;
                currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
                virtualCamera.m_Lens.OrthographicSize = currentZoom;
            }
        }

    }
}

