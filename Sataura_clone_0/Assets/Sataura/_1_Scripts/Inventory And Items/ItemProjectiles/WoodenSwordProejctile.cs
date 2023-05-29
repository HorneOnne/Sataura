using UnityEngine;

namespace Sataura
{
    public class WoodenSwordProejctile : NetworkProjectile, ICanCauseDamage
    {
        [SerializeField] private Animator _anim;
        private SwordData _swordData;
 
        public void SetUp(IngamePlayer player, SwordData swordData, Vector2 nearestEnemyPosition, bool upSide = true)
        {
            this._swordData = swordData;
            transform.localScale *= swordData.size * player.characterData._currentArea;

            if(upSide)
            {
                if (nearestEnemyPosition.x < player.transform.position.x)
                {
                    transform.position += new Vector3(-2, 0.25f, 0);
                    transform.localScale = new Vector3(-transform.localScale.x, -transform.localScale.y, 1);
                }
                else
                {
                    transform.position += new Vector3(2, 0.25f, 0);
                    transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, 1);
                }
            }
            else
            {
                if (nearestEnemyPosition.x < player.transform.position.x)
                {
                    transform.position += new Vector3(2, -0.25f, 0);
                    transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 1);                   
                }
                else
                {
                    transform.position += new Vector3(-2, -0.25f, 0);
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1);
                }
            }
            
        }


        public int GetDamage()
        {
            return _swordData.damage;
        }

        public float GetKnockback()
        {
            return _swordData.knockback;
        }

   
        public void DespawnNetworkObject()
        {
            if(base._networkObject.IsSpawned)
            {
                base._networkObject.Despawn();
            }
            
        }

        public enum WhipDirection
        {
            TOPLEFT,
            TOPRIGHT, 
            BOTTOMLEFT, 
            BOTTOMRIGHT,
        }
    }
}

