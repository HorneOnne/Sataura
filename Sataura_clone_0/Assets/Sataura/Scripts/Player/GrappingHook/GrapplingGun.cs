using UnityEngine;

namespace Sataura
{
    public class GrapplingGun : MonoBehaviour
    {
        [Header("Scripts:")]
        public GrappleRope grappleRope;
        [Header("Layer Settings:")]
        [SerializeField] private LayerMask grappableLayer;

        [Header("Main Camera")]
        private Camera m_camera;

        [Header("Transform Refrences:")]
        public Transform gunHolder;
        public Transform gunPivot;
        public Transform firePoint;

        [Header("Rotation:")]
        [SerializeField] private bool rotateOverTime = true;
        [Range(0, 80)][SerializeField] private float rotationSpeed = 4;

        [Header("Distance:")]
        [SerializeField] private bool hasMaxDistance = true;
        [SerializeField] private float maxDistance = 4;

        [Header("Launching")]
        [SerializeField] private bool launchToPoint = true;
        [SerializeField] private LaunchType Launch_Type = LaunchType.Transform_Launch;
        [Range(0, 5)][SerializeField] private float launchSpeed = 5;

        [Header("No Launch To Point")]
        [SerializeField] private bool autoCongifureDistance = false;
        [SerializeField] private float targetDistance = 3;
        [SerializeField] private float targetFrequency = 3;


        private enum LaunchType
        {
            Transform_Launch,
            Physics_Launch,
        }

        [Header("Component Refrences:")]
        public SpringJoint2D m_springJoint2D;

        [HideInInspector] public Vector2 grapplePoint;
        [HideInInspector] public Vector2 DistanceVector;
        Vector2 Mouse_FirePoint_DistanceVector;

        public Rigidbody2D ballRigidbody;
        private Vector2 hookAnchorPoint;

        // Anchor
        [SerializeField] private GameObject anchorPrefab;
        public GameObject anchorObject;
        private Rigidbody2D anchorRb2D;


        private void Start()
        {
            m_camera = Camera.main;
            grappleRope.enabled = false;
            m_springJoint2D.enabled = false;
            ballRigidbody.gravityScale = 1;
        }

        private void Update()
        {
            Mouse_FirePoint_DistanceVector = m_camera.ScreenToWorldPoint(Input.mousePosition) - gunPivot.position;
            RotateGun(m_camera.ScreenToWorldPoint(Input.mousePosition), false);

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                SetGrapplePoint();
            }


            if (grappleRope.enabled)
            {
                RotateGun(grapplePoint, false);
            }


            if (launchToPoint && grappleRope.isGrappling)
            {
                if (Launch_Type == LaunchType.Transform_Launch)
                {
                    if (hookCollided == false)
                    {
                        Vector2 direction = (Vector2)anchorObject.transform.position - (Vector2)this.transform.position;
                        anchorRb2D.velocity = -direction.normalized * 1000 * Time.fixedDeltaTime;

                    }

                    if (Vector2.Distance(anchorObject.transform.position, this.transform.position) < .4f)
                    {
                        Destroy(anchorObject);
                        this.grappleRope.enabled = false;
                        return;
                    }

                    Debug.Log("LaunchType.Transform_Launch");
                    if (Vector2.Distance(hookAnchorPoint, gunHolder.position) > 0.3f)
                    {                      
                        //gunHolder.position = Vector3.Lerp(gunHolder.position, grapplePoint, Time.deltaTime * launchSpeed);

                        float distance = Vector3.Distance(gunHolder.position, grapplePoint);
                        float duration = distance / (launchSpeed * 10); // calculate the duration of the interpolation based on distance and launch speed
                        float t = Time.deltaTime / duration; // calculate the t value based on the duration and delta time
                        gunHolder.position = Vector3.Lerp(gunHolder.position, grapplePoint, t);
                    }                  
                }
            }


            // Release hook.
            if (Input.GetKeyDown(KeyCode.Space))
            {
                grappleRope.enabled = false;
                m_springJoint2D.enabled = false;
                ballRigidbody.gravityScale = 1;
            }

            /*else if (Input.GetKey(KeyCode.Mouse0))
            {
                if (grappleRope.enabled)
                {
                    RotateGun(grapplePoint, false);
                }

                if (launchToPoint && grappleRope.isGrappling)
                {
                    if (Launch_Type == LaunchType.Transform_Launch)
                    {
                        Debug.Log("LaunchType.Transform_Launch");
                        gunHolder.position = Vector3.Lerp(gunHolder.position, grapplePoint, Time.deltaTime * launchSpeed);
                    }
                }

            }*/
            /*else if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                grappleRope.enabled = false;
                m_springJoint2D.enabled = false;
                ballRigidbody.gravityScale = 1;
            }*/
        }


        private void ResetGrappleRope()
        {
            grappleRope.enabled = false;
        }


        void RotateGun(Vector3 lookPoint, bool allowRotationOverTime)
        {
            Vector3 distanceVector = lookPoint - gunPivot.position;

            float angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
            if (rotateOverTime && allowRotationOverTime)
            {
                Quaternion startRotation = gunPivot.rotation;
                gunPivot.rotation = Quaternion.Lerp(startRotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * rotationSpeed);
            }
            else
                gunPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }


        float distance = 20f;
        public bool hookCollided;
        void SetGrapplePoint()
        {
            Debug.Log("SetGrapplePoint");
            RaycastHit2D _hit = Physics2D.Raycast(firePoint.position, Mouse_FirePoint_DistanceVector.normalized, distance, grappableLayer);
            if (_hit)
            {
                if ((Vector2.Distance(_hit.point, firePoint.position) <= maxDistance) || !hasMaxDistance)
                {
                    hookCollided = true;

                    launchToPoint = true;
                    grapplePoint = _hit.point;
                    DistanceVector = grapplePoint - (Vector2)gunPivot.position;
                    grappleRope.enabled = true;


                    // Set gravity to 0 prevent object fall down.
                    // -
                    ballRigidbody.gravityScale = 0;

                    // Set hook anchor point
                    // -
                    hookAnchorPoint = _hit.point;
                }
            }
            else
            {
                hookCollided = false;


                launchToPoint = false;

                var predictNewPoint = GetPointAtDistanceAndDirection(firePoint.position, distance, Mouse_FirePoint_DistanceVector.normalized);
                grapplePoint = predictNewPoint;
                DistanceVector = grapplePoint - (Vector2)gunPivot.position;
                grappleRope.enabled = true;


                // Set hook anchor point
                // -
                hookAnchorPoint = predictNewPoint;
            }


            if (anchorObject == null)
            {
                anchorObject = Instantiate(anchorPrefab, grapplePoint, Quaternion.identity);
                anchorRb2D = anchorObject.GetComponent<Rigidbody2D>();
            }         
        }




        private Vector2 GetPointAtDistanceAndDirection(Vector2 startPosition, float distance, Vector2 direction)
        {
            // Normalize the direction vector to get a vector of length 1
            Vector2 normalizedDirection = direction.normalized;

            // Calculate the position at the desired distance and direction
            Vector2 pointAtDistanceAndDirection = startPosition + normalizedDirection * distance;

            return pointAtDistanceAndDirection;
        }


        public void Grapple()
        {
            Debug.Log("Call Grapple()");


            if (launchToPoint == false && autoCongifureDistance == false)
            {
                m_springJoint2D.distance = targetDistance;
                m_springJoint2D.frequency = targetFrequency;
            }

            if (launchToPoint == false)
            {
                Debug.Log($"launchToPoint: {launchToPoint}");
                ballRigidbody.gravityScale = 1;

                if (autoCongifureDistance)
                {
                    m_springJoint2D.autoConfigureDistance = true;
                    m_springJoint2D.frequency = 0;
                }

                m_springJoint2D.connectedAnchor = grapplePoint;
                m_springJoint2D.enabled = true;
            }
            else
            {
                if (Launch_Type == LaunchType.Transform_Launch)
                {
                    Debug.Log("Grapple  LaunchType.Transform_Launch");

                    //ballRigidbody.gravityScale = 0;
                    ballRigidbody.velocity = Vector2.zero;
                }
                /*if (Launch_Type == LaunchType.Physics_Launch)
                {
                    m_springJoint2D.connectedAnchor = grapplePoint;
                    m_springJoint2D.distance = 0;
                    m_springJoint2D.frequency = launchSpeed;
                    m_springJoint2D.enabled = true;
                }*/
            }
        }

        private void OnDrawGizmos()
        {
            if (hasMaxDistance)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(firePoint.position, maxDistance);
            }
        }
    }
}
