using UnityEngine;

namespace Sataura
{
    public class WoodenSwordProjectile : MultipleTargetPhysicalProjectile
    {
        [Space(10)]
        [Header("===== WoodenSwordProejctile =====")]
        [Header("References")]
        [SerializeField] private Animator _anim;
        private bool _upSide = true;

        public void SetUpside(bool isUpside = true)
        {
            _upSide = isUpside;
        }

        private void LogicalFireProjectile(IngamePlayer player, Vector2 nearestEnemyPosition, bool upSide)
        {
            transform.localScale *= weaponData.size * player.characterData._currentArea;
         
            if (upSide)
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

        public override void Fire(IngamePlayer fromPlayer, Vector2 targetPosition, WeaponData weaponData, bool updateProjectileSize = true)
        {
            base.Fire(fromPlayer, targetPosition, weaponData);

            LogicalFireProjectile(fromPlayer, targetPosition, _upSide);
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

