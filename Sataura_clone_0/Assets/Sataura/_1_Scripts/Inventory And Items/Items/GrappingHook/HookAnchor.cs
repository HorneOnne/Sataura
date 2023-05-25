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
        private Rigidbody2D _rb2D;
        private BoxCollider2D _boxCollider2D;
        [SerializeField] private LayerMask canHookLayers;
        private Transform grapplingGunTransform;
        private Hook grapplingGun;

        [Header("Runtime References")]
        private HookData _hookData = null;

        [Header("Properites")]
        [SerializeField] private AnchorState anchorState;
        private Vector2 hookDirection;
        private float minDistance = 1.0f;


        #region Properties
        public Rigidbody2D Rb2D { get { return _rb2D; } }
        public BoxCollider2D _BoxCollider2D { get { return _boxCollider2D; } }
        public AnchorState State { get { return anchorState; } set { anchorState = value; } }
        #endregion

        private void Start()
        {
            _rb2D = GetComponent<Rigidbody2D>();
            _boxCollider2D = GetComponent<BoxCollider2D>();

            anchorState = AnchorState.Fired;
        }


        private void Update()
        {
            if (_hookData == null) return;

            float distance = Vector2.Distance(transform.position, grapplingGunTransform.position);
            switch (anchorState)
            {
                case AnchorState.Fired:
                    if (distance > _hookData.reach)
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
                    _rb2D.velocity = hookDirection * Time.fixedDeltaTime * (_hookData.velocity * 50);
                    break;
                case AnchorState.Anchored:
                    _rb2D.velocity = Vector2.zero;
                    break;
                case AnchorState.Retracting:
                    Vector2 retractDirection = grapplingGunTransform.position - transform.position;
                    _rb2D.velocity = retractDirection.normalized * Time.fixedDeltaTime * (_hookData.velocity * 50);
                    break;

            }

        }


        public void SetHookProperties(HookData _hookData, Vector2 hookDirection, Hook grapplingGun, Transform grapplingGunTransform)
        {
            this._hookData = _hookData;
            this.hookDirection = hookDirection;
            this.grapplingGunTransform = grapplingGunTransform;
            this.grapplingGun = grapplingGun;
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
