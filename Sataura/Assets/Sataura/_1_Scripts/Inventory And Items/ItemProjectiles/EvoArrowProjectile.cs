using System.Collections;
using UnityEngine;


namespace Sataura
{
    public class EvoArrowProjectile : NetworkProjectile, ICanCauseDamage
    { 
        [Header("Properties")]
        [SerializeField] private LayerMask groundLayer;
        private BoxCollider2D arrowCollider;
        private Transform _target;
        private Vector3 _randomReleaseDirection;
        private float _timeExist = 0.0f;
        private bool _isHitGround = false;

        [Header("Runtime References")]
        [SerializeField] private BowData bowData;


        public override void OnNetworkSpawn()
        {
            arrowCollider = GetComponent<BoxCollider2D>();

            StartCoroutine(Despawn(5f));
        }


   
        public void Shoot(BowData bowData, ArrowData arrowData, Transform target)
        {
            this.bowData = bowData;
            this._target = target;
            _randomReleaseDirection = new Vector3(Random.Range(-1, 1f), Random.Range(-1, 1f), 0);
            SetDustServerRpc(arrowData.particle);     
        }



        private void FixedUpdate()
        {
            if (_isHitGround) return;
           
            _timeExist += Time.fixedDeltaTime;
            if(_timeExist < 0.5f)
            {
                rb.velocity = (transform.up + _randomReleaseDirection).normalized * bowData.releaseSpeed;
            }
            else
            {
                if (_target == null)
                {
                    rb.velocity = transform.up * bowData.releaseSpeed;
                    return;
                }

                Vector2 direction = (Vector2)_target.position - rb.position;
                direction.Normalize();
                float rotateAmount = Vector3.Cross(direction, transform.up).z;
                rb.angularVelocity = -rotateAmount * 300f;
                rb.velocity = transform.up * bowData.releaseSpeed;
            }

                  
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!IsServer) return;

            if (groundLayer == (groundLayer | (1 << collision.gameObject.layer)))
            {
                _isHitGround = true;
                // Collision detected with a game object that is in the canHookLayers LayerMask
                // You can add your desired code or logic here
                ArrowPropertiesWhenCollide();
                StartCoroutine(Despawn(1f));

                
                SoundManager.Instance.PlaySound(SoundType.ArrowProjectileHitGround, playRandom: true, collision.transform.position);
            }
        }


        private IEnumerator Despawn(float time)
        {
            yield return new WaitForSeconds(time);
            if (_networkObject.IsSpawned)
                _networkObject.Despawn();
        }





        /// <summary>
        /// Sets the arrow properties when it collides with something.
        /// </summary>
        private void ArrowPropertiesWhenCollide()
        {
            rb.isKinematic = true;
            spriteRenderer.enabled = false;
            arrowCollider.enabled = false;
            rb.velocity = Vector2.zero;
        }


        public int GetDamage()
        {
            ArrowPropertiesWhenCollide();
            var returnDamage = bowData.baseAttackDamage;

            if (_networkObject.IsSpawned)
                _networkObject.Despawn();

            return returnDamage;
        }

        public float GetKnockback()
        {
            return 0.0f;
        }
    }
}
