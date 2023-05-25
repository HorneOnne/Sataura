using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    /// <summary>
    /// Manages all the item data, recipes, prefabs, and dust particle data,...
    /// </summary>
    public class GameDataManager : Singleton<GameDataManager>
    {
        private const string itemPrefabPrefix = "IP_";
        private const string projectilePrefabPrefix = "PP_";
        private const string enemyPrefabPrefix = "EP_";
        private const string itemDataPrefix = "I_";
        private const string craftingRecipePrefix = "CR_";


        [Header("VFX")]
        public GameObject levelUpVFX;
        public GameObject bloodTearVFX_001;
        public GreenPortalVFX greenPortalVFX;
        public HealValueFloatingText healValueFloatingPrefab;


        [Header("ITEM DATA")]
        [SerializeField] List<ItemData> itemData;

        /// <summary>
        /// A list of all the item prefabs.
        /// </summary>
        [Header("PREFABS")]
        [SerializeField] List<GameObject> itemPrefabs = new List<GameObject>();

        [SerializeField] List<GameObject> enemyPrefabs = new List<GameObject>();

        [SerializeField] List<GameObject> bossPrefabs = new List<GameObject>();

        [SerializeField] List<Debuff> _debuffVFXPrefabs = new List<Debuff>();

        [SerializeField] List<GameObject> _projectiles = new List<GameObject>();

        [Header("Currency")]
        [SerializeField] private GameObject bronzeCoinPrefab;
        [SerializeField] private GameObject sliverCoinPrefab;
        [SerializeField] private GameObject goldCoinPrefab;


        /// <summary>
        /// A list of all the recipe data.
        /// </summary>
        [Header("RECIPE")]
        public List<RecipeData> itemRecipeData;

        /// <summary>
        /// A dictionary that maps item data to their index in the <see cref="itemData"/> list.
        /// </summary>
        public Dictionary<ItemData, int> itemDataDict = new Dictionary<ItemData, int>();

        /// <summary>
        /// A dictionary that maps item data indices to their corresponding item data.
        /// </summary>
        public Dictionary<int, ItemData> itemDataById = new Dictionary<int, ItemData>();


        /// <summary>
        /// A dictionary that maps recipe data to their corresponding item slot.
        /// </summary>
        public Dictionary<RecipeData, ItemSlot> recipeToItemDict;

        /// <summary>
        /// A dictionary that maps item data to their corresponding recipe data.
        /// </summary>
        public Dictionary<ItemData, RecipeData> itemToRecipeDict;


        private Dictionary<string, GameObject> projectilePrefabByNameDict = new Dictionary<string, GameObject>();



        [Header("DUST DATA")]
        public List<ProjectileParticleData> projectileParticleDatas;

        //[Space(50)]
        [Header("NETWORK OBJECT")]
        public List<GameObject> networkObjects;


        [Header("ITEM OBJECT PARENT")]
        public Transform itemContainerParent;

        [Header("ITEM OBJECT PARENT")]
        public GameObject itemForDropPrefab;


        [Header("Players")]
        public GameObject damagePopupPrefab;

        [Header("Players (Runtime)")]
        public GameObject singleModePlayer;
        public Dictionary<ulong, Player> players = new Dictionary<ulong, Player>();

        




        /// <summary>
        /// Initializes the item data and prefab dictionaries, and generates the recipe dictionaries.
        /// </summary>
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            GenerateItemDataDict();
            InitializeRecipes();

            if (NetworkManager.Singleton == null)
            {
                return;
            }


            for (int i = 0; i < networkObjects.Count; i++)
            {
                NetworkManager.Singleton.AddNetworkPrefab(networkObjects[i]);
            }
        }


        private void Start()
        {
           /* if (NetworkManager.Singleton == null)
            {
                return;
            }

            for (int i = 0; i < networkObjects.Count; i++)
            {
                NetworkManager.Singleton.AddNetworkPrefab(networkObjects[i]);
            }*/
        }

        /// <summary>
        /// Generates the <see cref="itemDataDict"/> and <see cref="itemDataById"/> dictionaries.
        /// </summary>
        private void GenerateItemDataDict()
        {
            for (int i = 0; i < itemData.Count; i++)
            {
                if (itemData[i] != null && itemDataDict.ContainsKey(itemData[i]))
                    Debug.LogError($"[ItemDictionary]: \tItem at {i} already exist.");
                else
                {
                    itemDataDict.Add(itemData[i], i);
                    itemDataById.Add(i, itemData[i]);
                }
            }
        }


        public int GetItemID(ItemData itemData)
        {
            if (itemData == null)
                return -1;

            return itemDataDict[itemData];
        }


        public ItemData GetItemData(int id)
        {
            if (id == -1)
                return null;

            return itemDataById[id];
        }




        private GameObject GetItemPrefabType<T>()
        {
            for (int i = 0; i < itemPrefabs.Count; i++)
            {
                if (itemPrefabs[i].GetComponent<T>() != null)
                {
                    return itemPrefabs[i];
                }               
            }
            return null;
        }

        public GameObject GetItemPrefab(ItemType itemType)
        {
            GameObject itemPrefab = null;
            switch(itemType)
            {
                case ItemType.Sword:
                    itemPrefab = GetItemPrefabType<Sword>();
                    break;
                case ItemType.Bow:
                    itemPrefab = GetItemPrefabType<Bow>();
                    break;
                case ItemType.Hammer:
                    itemPrefab = GetItemPrefabType<Hammer>();
                    break;
                case ItemType.LightingStaff:
                    itemPrefab = GetItemPrefabType<LightningStaff>();
                    break;
                case ItemType.WhisperingGale:
                    itemPrefab = GetItemPrefabType<WhisperingGale>();
                    break;
                case ItemType.Hook:
                    itemPrefab = GetItemPrefabType<Hook>();
                    break;
                default: 
                    break;
            }

            if (itemPrefab == null)
                throw new System.Exception($"Not found item prefab type {itemType} in GameDataManager.cs.");
            else
                return itemPrefab;
        }

     
 


        /// <summary>
        /// Gets the projectile prefab for the item with the specified name.
        /// </summary>
        /// <param name="name">The name of the item prefab to retrieve.</param>
        /// <returns>The prefab for the item with the specified name, or null if no prefab with that name exists.</returns>
        public GameObject GetProjectilePrefab(string name)
        {
            if (projectilePrefabByNameDict.ContainsKey(name))
                return projectilePrefabByNameDict[name];
            else
            {
                throw new System.Exception($"Not found projectile prefab name {name} in GameDataManager.cs.");
            }
        }


        public GameObject GetProjectilePrefab<T>(T item)
        {
            string name = $"{projectilePrefabPrefix}{typeof(T).Name}";
            if (projectilePrefabByNameDict.ContainsKey($"{name}"))
                return projectilePrefabByNameDict[name];
            else
            {
                throw new System.Exception($"Not found projectile prefab name {name} in GameDataManager.cs.");
            }
        }


 


        /// <summary>
        /// Initializes the recipe dictionaries.
        /// </summary>
        public void InitializeRecipes()
        {
            recipeToItemDict = new Dictionary<RecipeData, ItemSlot>();
            itemToRecipeDict = new Dictionary<ItemData, RecipeData>();
            int index = 0;
            foreach (var recipe in itemRecipeData)
            {
                if (recipeToItemDict.ContainsKey(recipe))
                {
                    Debug.LogError($"[ItemRecipeManager]: \tRecipe at {index} already exist.");
                }
                else
                {
                    //recipeScriptableObjectDict.Add(recipe, recipe.outputItem);
                    recipeToItemDict.Add(recipe, new ItemSlot(recipe.outputItem, recipe.quantityItemOutput));
                }

                index++;

            }

            itemRecipeData.Clear();

            foreach (var recipe in recipeToItemDict)
            {
                if (itemToRecipeDict.ContainsKey(recipe.Value.ItemData) == false)
                {
                    itemToRecipeDict.Add(recipe.Value.ItemData, recipe.Key);
                }
            }
        }


        /// <summary>
        /// Gets the item slot for the specified recipe data.
        /// </summary>
        /// <param name="recipe">The recipe data to retrieve the item slot for.</param>
        /// <returns>The item slot for the specified recipe data, or null if no item slot exists for that recipe.</returns>
        public ItemSlot GetItemFromRecipe(RecipeData recipe)
        {
            if (recipeToItemDict.ContainsKey(recipe))
            {
                ItemSlot returnedSlot = new ItemSlot(recipeToItemDict[recipe]);
                return returnedSlot;
            }
            return null;
        }


        /// <summary>
        /// Returns the recipe data that corresponds to the given item data, if it exists.
        /// </summary>
        /// <param name="itemData">The item data to find the corresponding recipe for.</param>
        /// <returns>The recipe data that corresponds to the given item data, or null if it does not exist.</returns>
        public RecipeData GetRecipeFromItem(ItemData itemData)
        {
            if (itemToRecipeDict.ContainsKey(itemData))
            {
                return itemToRecipeDict[itemData];
            }
            return null;
        }



        /// <summary>
        /// Returns a list of sprite frames for the projectile particle data at the given index.
        /// </summary>
        /// <param name="index">The index of the projectile particle data to get sprite frames for.</param>
        /// <returns>A list of sprite frames for the projectile particle data at the given index.</returns>
        public List<Sprite> GetProjectileParticleFrames(int index)
        {
            return projectileParticleDatas[index].frames;
        }

        public GameObject GetBronzeCoin() => bronzeCoinPrefab;
        public GameObject GetSliverCoin() => sliverCoinPrefab;
        public GameObject GetGoldCoin() => goldCoinPrefab;



        private GameObject GetEnemyPrefabByType<T>()
        {
            for (int i = 0; i < enemyPrefabs.Count; i++)
            {
                if (enemyPrefabs[i].GetComponent<T>() != null)
                {
                    return enemyPrefabs[i];
                }
            }
            return null;
        }
        public GameObject GetEnemyPrefab(EnemyType enemyType)
        {
            GameObject enemyPrefab = null;
            switch (enemyType)
            {
                case EnemyType.PinkSlime:
                    enemyPrefab = GetEnemyPrefabByType<PinkSlime>();
                    break;
                case EnemyType.BlueSlime:
                    enemyPrefab = GetEnemyPrefabByType<BlueSlime>();
                    break;
                case EnemyType.MotherSlime:
                    enemyPrefab = GetEnemyPrefabByType<MotherSlime>();
                    break;
                case EnemyType.BabySlime:
                    enemyPrefab = GetEnemyPrefabByType<BabySlime>();
                    break;
                case EnemyType.Bat:
                    enemyPrefab = GetEnemyPrefabByType<Bat>();
                    break;
                case EnemyType.BlackBat:
                    enemyPrefab = GetEnemyPrefabByType<BlackBat>();
                    break;
                case EnemyType.Bonehead:
                    enemyPrefab = GetEnemyPrefabByType<WhiteSkull>();
                    break;
                case EnemyType.Cursedwraith:
                    enemyPrefab = GetEnemyPrefabByType<PurpleSkull>();
                    break;
                case EnemyType.ObsidianMaw:
                    enemyPrefab = GetEnemyPrefabByType<Golem>();               
                    break;
                default:
                    break;
            }

            if (enemyPrefab != null) 
                return enemyPrefab;
            else
                return null;
        }


        private GameObject GetBossPrefabByType<T>()
        {
            for (int i = 0; i < bossPrefabs.Count; i++)
            {
                if (bossPrefabs[i].GetComponent<T>() != null)
                {
                    return bossPrefabs[i];
                }
            }
            return null;
        }
        public GameObject GetBossPrefab(BossType bossType)
        {
            GameObject bossPrefab = null;
            switch (bossType)
            {
                case BossType.KingSlime:
                    bossPrefab = GetBossPrefabByType<KingSlime>();
                    break;              
                default:
                    break;
            }

            if (bossPrefab != null)
                return bossPrefab;
            else
                return null;
        }

        public Sataura.Debuff GetDebuffEffectVFXPrefabs(DebuffEffect debuffEffect)
        {
            switch(debuffEffect)
            {
                case DebuffEffect.Slowly:
                    for(int i = 0; i < _debuffVFXPrefabs.Count; i++)
                    {
                        if (_debuffVFXPrefabs[i]._DebuffEffect == DebuffEffect.Slowly)
                        {
                            return _debuffVFXPrefabs[i];
                        }
                    }
                    break;
                default:
                    break;
            }
            return null;
        }

        public GameObject GetProjectilePrefab(ProjectileType projectileType)
        {
            GameObject projectilePrefab = null;

            switch (projectileType)
            {
                case ProjectileType.WoodenSwordProjectile:
                    projectilePrefab = GetProjectilePrefabByType<WoodenSwordProejctile>();
                    break;
                case ProjectileType.EvoWoodenSwordProjectile:
                    projectilePrefab = GetProjectilePrefabByType<EvoWoodenSwordProejctile>();
                    break;
                case ProjectileType.GolemBullet:
                    projectilePrefab = GetProjectilePrefabByType<GolemBullet>();
                    break;
                case ProjectileType.BookOfWindProjectile:
                    projectilePrefab = GetProjectilePrefabByType<BookOfWindProjectile>();
                    break;
                case ProjectileType.LightningStaffProjectile:
                    projectilePrefab = GetProjectilePrefabByType<LightningStaffProjectile>();
                    break;
                case ProjectileType.NormalArrow:
                    projectilePrefab = GetProjectilePrefabByType<NormalArrowProjectile>();
                    break;
                case ProjectileType.EvoArrow:
                    projectilePrefab = GetProjectilePrefabByType<EvoArrowProjectile>();
                    break;
                case ProjectileType.BlackWandProjectile:
                    projectilePrefab = GetProjectilePrefabByType<BlackWandProjectile>();
                    break;
                case ProjectileType.BlackWandDamageZone:
                    projectilePrefab = GetProjectilePrefabByType<BlackWandDamageZone>();
                    break;
                case ProjectileType.BlackHole:
                    projectilePrefab = GetProjectilePrefabByType<BlackHoleProjectile>();
                    break;
                case ProjectileType.HammerProjectile:
                    projectilePrefab = GetProjectilePrefabByType<HammerProjectile>();
                    break;
                case ProjectileType.NoMoreHammerProjectile:
                    projectilePrefab = GetProjectilePrefabByType<NoMoreHammerProjectile>();
                    break;
                default:
                    break;
            }

            if (projectilePrefab == null)
                return null;
            else
                return projectilePrefab;
        }
        private GameObject GetProjectilePrefabByType<T>()
        {
            for (int i = 0; i < _projectiles.Count; i++)
            {
                if (_projectiles[i].GetComponent<T>() != null)
                {
                    return _projectiles[i];
                }
            }
            return null;
        }


        public void AddNetworkPlayer(ulong clientId, Player player)
        {
            if (players.ContainsKey(clientId) == false)
                players.Add(clientId, player);
        }

    }
}