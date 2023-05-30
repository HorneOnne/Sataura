using System.Collections;
using UnityEngine;


namespace Sataura
{
    public class EvoArrowProjectile : NetworkProjectile
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


   
        public void Shoot(BowData bowData, Transform target)
        {
            this.bowData = bowData;
            this._target = target;
            _randomReleaseDirection = new Vector3(Random.Range(-1, 1f), Random.Range(-1, 1f), 0);
        }



        private void FixedUpdate()
        {
            if (_isHitGround) return;
           
            _timeExist += Time.fixedDeltaTime;
            if(_timeExist < 0.5f)
            {
                rb2D.velocity = (transform.up + _randomReleaseDirection).normalized * bowData.releaseSpeed;
            }
            else
            {
                if (_target == null)
                {
                    rb2D.velocity = transform.up * bowData.releaseSpeed;
                    return;
                }

                Vector2 direction = (Vector2)_target.position - rb2D.position;
                direction.Normalize();
                float rotateAmount = Vector3.Cross(direction, transform.up).z;
                rb2D.angularVelocity = -rotateAmount * 300f;
                rb2D.velocity = transform.up * bowData.releaseSpeed;
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
            if (networkObject.IsSpawned)
                networkObject.Despawn();
        }





        /// <summary>
        /// Sets the arrow properties when it collides with something.
        /// </summary>
        private void ArrowPropertiesWhenCollide()
        {
            rb2D.isKinematic = true;
            spriteRenderer.enabled = false;
            arrowCollider.enabled = false;
            rb2D.velocity = Vector2.zero;
        }


        public int GetDamage()
        {
            ArrowPropertiesWhenCollide();
            var returnDamage = bowData.damage;

            if (networkObject.IsSpawned)
                networkObject.Despawn();

            return returnDamage;
        }

        public float GetKnockback()
        {
            return 0.0f;
        }
    }
}
