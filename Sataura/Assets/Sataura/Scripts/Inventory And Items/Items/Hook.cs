using UnityEngine;

namespace Sataura
{
    public class Hook : Item
    {
        private Camera mainCamera;
        [SerializeField] LineRenderer lineRenderer;
        [SerializeField] DistanceJoint2D distanceJoint2D;

        private void Start()
        {
            mainCamera = Camera.main;
            distanceJoint2D.enabled = false;
        }


        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                lineRenderer.SetPosition(0, mousePos);
                lineRenderer.SetPosition(1, transform.position);
                distanceJoint2D.connectedAnchor = mousePos;
                distanceJoint2D.enabled = true;
                lineRenderer.enabled = true;
            }
            else if(Input.GetKeyUp(KeyCode.E))
            {
                lineRenderer.enabled = false;
                distanceJoint2D.enabled = false;
            }

            if(distanceJoint2D.enabled)
            {
                lineRenderer.SetPosition(1, transform.position);
            }
        }

    }
}
