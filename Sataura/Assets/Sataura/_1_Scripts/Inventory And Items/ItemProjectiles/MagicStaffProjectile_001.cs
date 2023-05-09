﻿using System.Collections;
using UnityEngine;

namespace Sataura
{
    /// <summary>
    /// Represents a projectile fired by a magic staff that can cause damage to targets it collides with.
    /// </summary>
    [RequireComponent(typeof(CircleCollider2D))]
    public class MagicStaffProjectile_001 : NetworkProjectile, ICanCauseDamage
    {
        /// <summary>
        /// The data associated with the magic staff used to fire this projectile.
        /// </summary>
        private MagicStaffData staffData;

        /// <summary>
        /// The position where this projectile was initially fired from.
        /// </summary>
        private Vector2 initialPosition;


        /// <summary>
        /// The amount of time elapsed since this projectile was fired and will be returned to the object pool if it has not collided with a target.
        /// </summary>
        private float elapsedTimeToReturn = 0.0f;

        /// <summary>
        /// The maximum amount of time this projectile can be in the game world before being returned to the object pool if it has not collided with a target.
        /// </summary>
        private const float TIME_TO_RETURN = 5.0f;

        /// <summary>
        /// The amount of time this projectile has to return to the object pool after colliding with a target.
        /// </summary>
        private const float TIME_TO_RETURN_WHEN_COLLIDE = 3.0f;


  
        public override void OnNetworkSpawn()
        {
            this.staffData = (MagicStaffData)ItemData;
        }




        public void Shoot()
        {
            // Get the MagicStaffData instance associated with the magic staff used to fire this projectile.
            this.staffData = (MagicStaffData)ItemData;

            // Record the position where this projectile was fired from.
            initialPosition = transform.position;

            // Create a particle effect at the starting position.
            SetDust(staffData.particle);
            // Calculate the direction of the projectile based on the mouse position.
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePosition - initialPosition;

            // Set the velocity of the projectile based on the direction and release speed of the magic staff.
            rb.velocity = direction.normalized * staffData.releaseProjectileSpeed;
        }

        private void Update()
        {
            // Check if the maximum time limit for this projectile has been reached.
            elapsedTimeToReturn += Time.deltaTime;
            if (elapsedTimeToReturn > TIME_TO_RETURN)
            {
                elapsedTimeToReturn = 0.0f;
                ReturnToPool();
            }
        }


        public int GetDamage()
        {
            return staffData.damage;
        }

        public float GetKnockback()
        {
            return 0.0f;
        }



        /// <summary>
        /// This method returns the projectile to the object pool for reuse. It checks if the projectile has already been returned to the pool before, if so, it immediately returns.
        /// </summary>
        private void ReturnToPool()
        {
            MagicStaffProjectileSpawner.Instance.Pool.Release(this.gameObject);
        }

    }
}