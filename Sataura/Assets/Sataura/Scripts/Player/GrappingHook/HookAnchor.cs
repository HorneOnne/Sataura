using UnityEngine;

namespace Sataura
{
    public class HookAnchor : MonoBehaviour
    {
        public enum AnchorState
        {
            Idle,
            Fired,
            Anchored,
            Retracting,
            Interrupt,
            Broken
        }


        [Header("References")]
        private Rigidbody2D rb2D;
        [SerializeField] private LayerMask canHookLayers;
        private Transform grapplingGunTransform;
        private GrapplingGun grapplingGun;


        [Header("Properites")]
        [SerializeField] private AnchorState anchorState;
        private Vector2 hookDirection;
        [SerializeField] private float maxDistance;
        private float minDistance = 1.0f;
        [SerializeField] private float launchForce;


        #region Properties
        public Rigidbody2D Rb2D { get { return rb2D; } }
        public AnchorState State { get { return anchorState; } set { anchorState = value; } }
        #endregion

        private void Start()
        {
            rb2D = GetComponent<Rigidbody2D>();

            anchorState = AnchorState.Fired;
        }


        private void Update()
        {
            float distance = Vector2.Distance(transform.position, grapplingGunTransform.position);
            switch (anchorState)
            {
                case AnchorState.Fired:
                    if (distance > maxDistance)
                    {
                        anchorState = AnchorState.Retracting;
                    }
                    break;
                case AnchorState.Retracting:
                    if (distance < minDistance)
                    {
                        anchorState = AnchorState.Broken;
                    }
                    break;
                case AnchorState.Anchored:
                    if (distance < minDistance)
                    {
                        anchorState = AnchorState.Idle;
                    }
                    break;
                case AnchorState.Broken:
                    this.grapplingGun.grappleRope.enabled = false;
                    Destroy(this.gameObject);
                    break;
            }     
        }

        private void FixedUpdate()
        {
            switch (anchorState)
            {
                case AnchorState.Idle:
                    break;
                case AnchorState.Fired:
                    rb2D.velocity = hookDirection * Time.fixedDeltaTime * launchForce;
                    break;
                case AnchorState.Anchored:
                    rb2D.velocity = Vector2.zero;
                    break;
                case AnchorState.Retracting:
                    Vector2 retractDirection = grapplingGunTransform.position - transform.position;
                    rb2D.velocity = retractDirection.normalized * Time.fixedDeltaTime * launchForce;
                    break;

            }

        }


        public void SetHookProperties(Vector2 hookDirection, GrapplingGun grapplingGun, Transform grapplingGunTransform, float maxDistance, float launchForce)
        {
            this.hookDirection = hookDirection;
            this.grapplingGunTransform = grapplingGunTransform;
            this.grapplingGun = grapplingGun;
            this.maxDistance = maxDistance;
            this.launchForce = launchForce;
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (canHookLayers == (canHookLayers | (1 << collision.gameObject.layer)))
            {
                // Collision detected with a game object that is in the canHookLayers LayerMask
                // You can add your desired code or logic here

                anchorState = AnchorState.Anchored;
            }
        }
    }
}
