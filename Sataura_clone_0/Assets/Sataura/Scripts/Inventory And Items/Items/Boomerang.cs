using Unity.Netcode;
using UnityEngine;

namespace Sataura
{

    public class Boomerang : Item, ICanCauseDamage
    {
        //private GameObject boomerangProjectilePrefab;
        private BoomerangProjectile_001 boomerangProjectileObject;

        private bool isReturning = true;
        private BoomerangData boomerangData;



        [Header("References")]
        [SerializeField] private GameObject boomerangProjectilePrefab;



        private void OnEnable()
        {
            EventManager.OnBoomerangReturned += ResetBoomerangState;
        }

        private void OnDisable()
        {
            EventManager.OnBoomerangReturned -= ResetBoomerangState;
        }




        public override bool Use(Player player, Vector2 mousePosition)
        {
            boomerangData = (BoomerangData)this.ItemData;
            /*if (boomerangProjectilePrefab == null)
                boomerangProjectilePrefab = BoomerangProjectileSpawner.Instance.prefab;*/

            switch (boomerangData.attackType)
            {
                case 1:
                    if (isReturning)
                    {
                        isReturning = false;

                        boomerangProjectileObject = Instantiate(boomerangProjectilePrefab, transform.position, Quaternion.identity).GetComponent<BoomerangProjectile_001>();
                        boomerangProjectileObject.GetComponent<NetworkObject>().Spawn();
                        boomerangProjectileObject.SetData(this.ItemData);
                        boomerangProjectileObject.Throw(player, (BoomerangData)this.ItemData);
                    }
                    break;
                case 2:
                    boomerangProjectileObject = Instantiate(boomerangProjectilePrefab, transform.position, Quaternion.identity).GetComponent<BoomerangProjectile_001>();
                    boomerangProjectileObject.GetComponent<NetworkObject>().Spawn();
                    boomerangProjectileObject.SetData(this.ItemData);
                    boomerangProjectileObject.Throw(player, (BoomerangData)this.ItemData);
                    break;
                default:
                    break;
            }
            


            return true;
        }


        private void ResetBoomerangState()
        {
            isReturning = true;
        }


        public int GetDamage()
        {
            return ((BoomerangData)ItemData).damage;
        }

        public float GetKnockback()
        {
            return 0.0f;
        }
    }
}
