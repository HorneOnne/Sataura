using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    public class Sword : Item
    {
        private GameObject swordProjectilePrefab;
        private GameObject swordProjectileObject;
        private SwordData swordData;

        private NetworkObject swordNetworkObject;
        private float initialZAngle;


        // Passive
        [SerializeField] private GameObject passiveProjectilePrefab;

        public override void OnNetworkSpawn()
        {
            swordProjectilePrefab = GameDataManager.Instance.GetProjectilePrefab("PP_SwordProjectile_001");
            swordData = ((SwordData)ItemData);

            if(IsServer)
            {
                int itemID = GameDataManager.Instance.GetItemID(swordData);
                SetDataServerRpc(itemID, 1);       
            }
        }

        public override bool Use(Player player, Vector2 mousePosition)
        {           
            switch(swordData.useType)
            {
                case 1:
                    UseType01(player, mousePosition);
                    break;
                case 2:
                    UseType02(player, mousePosition);
                    break;
                default:
                    Debug.LogWarning($"Not found useType {swordData.useType} in SwordData.");
                    break;
            }

            return true;
        }



        float usagePassiveTimeCount = 0.0f;
        public override void UsePassive(Player player, Vector2 mousePosition)
        {
            if(Time.time - usagePassiveTimeCount > (1.0f / ItemData.usagePassiveVelocity))
            {
                usagePassiveTimeCount = Time.time;
            }
            else
            {
                return;
            }

            var passiveObject = Instantiate(passiveProjectilePrefab, player.transform.position, Quaternion.identity);
            var passiveProjectile = passiveObject.GetComponent<NetworkProjectile>();
            passiveProjectile.networkObject.Spawn();
        }



        private void UseType01(Player player, Vector2 mousePosition)
        {
            initialZAngle = (mousePosition.x - transform.position.x > 0) ? 90 : 0;

            swordProjectilePrefab = GameDataManager.Instance.GetProjectilePrefab("PP_SwordProjectile_001");
            swordProjectileObject = Instantiate(swordProjectilePrefab, transform.position, Quaternion.Euler(0,0, initialZAngle), player.transform); 
            swordNetworkObject = swordProjectileObject.GetComponent<NetworkObject>();
            swordNetworkObject.Spawn();
            swordNetworkObject.TrySetParent(player.transform);
 
            int itemID = GameDataManager.Instance.GetItemID(this.ItemSlot.ItemData);            
            swordProjectileObject.GetComponent<SwordProjectile_001>().SetDataServerRpc(itemID, true);            
            swordProjectileObject.GetComponent<SwordProjectile_001>().LoadSwordProjectileDataServerRpc(mousePosition);

            SoundManager.Instance.PlaySound(SoundType.Sword, playRandom: true);
        }

        private void UseType02(Player player, Vector2 mousePosition)
        {
            swordProjectilePrefab = GameDataManager.Instance.GetProjectilePrefab("PP_SwordProjectile_001");
            swordProjectileObject = Instantiate(swordProjectilePrefab, transform.position, transform.rotation, player.transform);
            swordProjectileObject.transform.localScale = new Vector3(4, 4, 1);
            swordProjectileObject.SetActive(true);
            swordProjectileObject.GetComponent<NetworkProjectile>().SetData(this.ItemSlot.ItemData);

            var swordProjectile002 = SworldProjectile002Spawner.Instance.Pool.Get().GetComponent<SwordProjectile_002>();
            swordProjectile002.SetData(swordData);
            swordProjectile002.Shoot(this.transform.position);

            Utilities.RotateObjectTowardMouse2D(mousePosition,swordProjectile002.transform, -45f);
        }

    }
}