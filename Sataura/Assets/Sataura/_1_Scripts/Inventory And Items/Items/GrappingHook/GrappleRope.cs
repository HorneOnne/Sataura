using UnityEngine;

namespace Sataura
{
    public class GrappleRope : MonoBehaviour
    {
        [Header("General refrences:")]
        public Hook _hook;
        [SerializeField] LineRenderer _lineRenderer;



        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.enabled = false;
        }

        private void Start()
        {
          
        }

        private void OnEnable()
        {
            _lineRenderer.enabled = true;
        }

        private void OnDisable()
        {
            _lineRenderer.positionCount = 0;
            _lineRenderer.enabled = false;      
        }



        void FixedUpdate()
        {
            DrawRopeNoWaves();
        }


        void DrawRopeNoWaves()
        {
            if(_hook.anchorObject == null)
            {
                return;
            }

            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, _hook.anchorObject.transform.position);
            _lineRenderer.SetPosition(1, _hook.firePoint.position);
        }

        /*void DrawRopeNoWaves()
        {
            m_lineRenderer.positionCount = 2;
            m_lineRenderer.SetPosition(0, grapplingGun.grapplePoint);
            m_lineRenderer.SetPosition(1, grapplingGun.firePoint.position);
        }*/

    }
}
