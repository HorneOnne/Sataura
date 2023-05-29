using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    public class Hook : Item
    {
        [Header("Layer Settings:")]
        [SerializeField] private LayerMask grappableLayer;


        [Header("Refrences:")]
        [SerializeField] private Rigidbody2D characterRb2D;
        public Transform characterTransform;
        public Transform gunPivot;
        public Transform firePoint;
        public GrappleRope grappleRope;


        [Header("Rotation:")]
        [SerializeField] private bool rotateOverTime = true;
        [Range(0, 80)][SerializeField] private float rotationSpeed = 4;


        [Header("Launching")]
        [Range(10, 50)][SerializeField] private float characterDragForce = 20;

        
        // Anchor
        [SerializeField] private HookAnchor anchorPrefab;
        public HookAnchor anchorObject;


        // Cached
        private Camera m_camera;
        Vector2 mousePosition;
        Vector2 hookDirection;
        Vector2 hookInteruptDirection;

        [Header("Runtime References")]
        [SerializeField] private HookData _hookData;

        private void Start()
        {
           
        }

        public override void OnNetworkSpawn()
        {
            _hookData = ((HookData)ItemData);
            m_camera = Camera.main;
            grappleRope.enabled = false;
            var ingamePlayer = GameDataManager.Instance.ingamePlayer;
            characterTransform = ingamePlayer.transform;
            characterRb2D = ingamePlayer.playerMovement.Rb2D;

            if (IsServer)
            {
                int itemID = GameDataManager.Instance.GetItemID(_hookData);
                SetDataServerRpc(itemID, 1);
            }
        }

   

        private void Update()
        {
            if (_hookData == null) return;

            RotateGun(m_camera.ScreenToWorldPoint(Input.mousePosition), false);
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (anchorObject != null) return;
   
                anchorObject = Instantiate(anchorPrefab, characterTransform.position, Quaternion.identity);
                mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                hookDirection = mousePosition - (Vector2)transform.position;

                anchorObject.SetHookProperties(_hookData, hookDirection.normalized, this, this.transform);
                grappleRope.enabled = true;
            }


            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (anchorObject == null) return;
 
                if (anchorObject.State == HookAnchor.AnchorState.Idle ||
                    anchorObject.State == HookAnchor.AnchorState.Anchored)
                {
                    characterRb2D.isKinematic = false;
                    anchorObject._BoxCollider2D.enabled = false;
                    anchorObject.State = HookAnchor.AnchorState.Interrupt;
                }     
                
                if(anchorObject.State == HookAnchor.AnchorState.Retracting)
                {
                    anchorObject._BoxCollider2D.enabled = false;
                }
            }           
        }

        

        private void FixedUpdate()
        {
            if(anchorObject != null)
            {
                if (anchorObject.State == HookAnchor.AnchorState.Anchored)
                {
                    // Move the object towards the target position gradually
                    Vector2 currentPosition = characterRb2D.position;
                    Vector2 newPosition = Vector2.MoveTowards(currentPosition, anchorObject.transform.position, characterDragForce * Time.fixedDeltaTime);
                    characterRb2D.MovePosition(newPosition);  
                }
                else if (anchorObject.State == HookAnchor.AnchorState.Idle)
                {
                    if(characterRb2D.isKinematic == false)
                        characterRb2D.isKinematic = true;

                    characterRb2D.velocity = Vector2.zero;

                }
                else if (anchorObject.State == HookAnchor.AnchorState.Interrupt)
                {
                    Vector2 currentPosition = characterRb2D.position;
                    Vector2 targetPosition = anchorObject.transform.position;
                    hookDirection = (targetPosition - currentPosition).normalized;

                    characterRb2D.velocity = hookDirection.normalized * 3000 * Time.fixedDeltaTime;

                    anchorObject.State = HookAnchor.AnchorState.Retracting;
                }
            }            
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
    }
}
