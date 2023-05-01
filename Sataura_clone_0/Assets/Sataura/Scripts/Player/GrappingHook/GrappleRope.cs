using UnityEngine;

namespace Sataura
{
    public class GrappleRope : MonoBehaviour
    {
        [Header("General refrences:")]
        public GrapplingGun grapplingGun;
        [SerializeField] LineRenderer m_lineRenderer;



        private void Awake()
        {
            m_lineRenderer = GetComponent<LineRenderer>();
            m_lineRenderer.enabled = false;
        }

        private void Start()
        {
          
        }

        private void OnEnable()
        {
            m_lineRenderer.enabled = true;
        }

        private void OnDisable()
        {
            m_lineRenderer.enabled = false;
        }



        void FixedUpdate()
        {
            DrawRopeNoWaves();
        }


        void DrawRopeNoWaves()
        {
            if(grapplingGun.anchorObject == null)
            {
                Debug.Log("grapplingGun.anchorObject == null"); 
                return;
            }

            Debug.Log("DrawRopeNoWaves");
            m_lineRenderer.positionCount = 2;
            m_lineRenderer.SetPosition(0, grapplingGun.anchorObject.transform.position);
            m_lineRenderer.SetPosition(1, grapplingGun.firePoint.position);
        }

        /*void DrawRopeNoWaves()
        {
            m_lineRenderer.positionCount = 2;
            m_lineRenderer.SetPosition(0, grapplingGun.grapplePoint);
            m_lineRenderer.SetPosition(1, grapplingGun.firePoint.position);
        }*/

    }
}
