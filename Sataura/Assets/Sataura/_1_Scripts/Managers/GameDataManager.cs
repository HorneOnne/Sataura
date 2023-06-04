using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    /// <summary>
    /// Manages all the item data, recipes, prefabs, and dust particle data,...
    /// </summary>
    public class GameDataManager : MonoBehaviour
    {
        public static GameDataManager Instance { get; private set; }

        [Header("VFX")]
        public GameObject levelUpVFX;
        public GameObject bloodTearVFX_001;
        public GreenPortalVFX greenPortalVFX;
        public HealValueFloatingText healValueFloatingPrefab;


        [Header("ITEM DATA")]
        [SerializeField] private List<ItemData> _itemData;
        public Dictionary<ItemData, int> itemDataDict = new Dictionary<ItemData, int>();
        public Dictionary<int, ItemData> itemDataById = new Dictionary<int, ItemData>();


        [Header("PREFABS")]
        [SerializeField] private List<GameObject> _itemPrefabs = new List<GameObject>();
        [SerializeField] private List<GameObject> _enemyPrefabs = new List<GameObject>();
        [SerializeField] private List<GameObject> _bossPrefabs = new List<GameObject>();
        [SerializeField] private List<GameObject> _projectilesPrefab = new List<GameObject>();
        [SerializeField] private List<Debuff> _debuffVFXPrefabs = new List<Debuff>();



        [Header("Currency")]
        [SerializeField] private GameObject bronzeCoinPrefab;
        [SerializeField] private GameObject sliverCoinPrefab;
        [SerializeField] private GameObject goldCoinPrefab;


        [Header("RECIPE")]
        public List<RecipeData> itemRecipeData;
        public Dictionary<RecipeData, ItemSlot> recipeToItemDict;
        public Dictionary<ItemData, RecipeData> itemToRecipeDict;



        [Header("DUST DATA")]
        public List<ProjectileParticleData> projectileParticleDatas;


        [Header("ITEM OBJECT PARENT")]
        public GameObject itemForDropPrefab;



        public GameObject damagePopupPrefab;

        [Header("MainmenuInformation References")]
        public MainMenuInfomation mainMenuInformation;


        [Header("Players Prefab")]
        public IngamePlayer ingamePlayerPrefab;
        public InventoryPlayer inventoryPlayerPrefab;


        [Header("Players (Runtime)")]
        public Player currentPlayer;
        [HideInInspector] public IngamePlayer ingamePlayer;
        [HideInInspector] public InventoryPlayer inventoryPlayer;


        public Dictionary<ulong, IngamePlayer> players = new Dictionary<ulong, IngamePlayer>();


        public enum GameDataType
        {
            Item,
            Enemy,
            Boss,
            Projectile,
            VFX,
        }

  
        /// <summary>
        /// Initializes the item data and prefab dictionaries, and generates the recipe dictionaries.
        /// </summary>
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }


            GenerateItemDataDict();
            InitializeRecipes();
        }

        private void Start()
        {
            //LoadNetworkObjectIntoNetworkManager();
        }

        private IEnumerator PerformanceLoadNetworkObjectIntoNetworkManager()
        {
            for (int i = 0; i < _itemPrefabs.Count; i++)
            {
                NetworkManager.Singleton.AddNetworkPrefab(_itemPrefabs[i]);
            }
            yield return null;
            for (int i = 0; i < _enemyPrefabs.Count; i++)
            {
                NetworkManager.Singleton.AddNetworkPrefab(_enemyPrefabs[i]);
            }
            yield return null;
            for (int i = 0; i < _bossPrefabs.Count; i++)
            {
                NetworkManager.Singleton.AddNetworkPrefab(_bossPrefabs[i]);
            }
            yield return null;
            for (int i = 0; i < _projectilesPrefab.Count; i++)
            {
                NetworkManager.Singleton.AddNetworkPrefab(_projectilesPrefab[i]);
            }
            yield return null;
        }

        private void LoadNetworkObjectIntoNetworkManager()
        {
            for (int i = 0; i < _itemPrefabs.Count; i++)
            {
                NetworkManager.Singleton.AddNetworkPrefab(_itemPrefabs[i]);
            }
            for (int i = 0; i < _enemyPrefabs.Count; i++)
            {
                NetworkManager.Singleton.AddNetworkPrefab(_enemyPrefabs[i]);
            }
            for (int i = 0; i < _bossPrefabs.Count; i++)
            {
                NetworkManager.Singleton.AddNetworkPrefab(_bossPrefabs[i]);
            }
            for (int i = 0; i < _projectilesPrefab.Count; i++)
            {
                NetworkManager.Singleton.AddNetworkPrefab(_projectilesPrefab[i]);
            }
        }



        /// <summary>
        /// Generates the <see cref="itemDataDict"/> and <see cref="itemDataById"/> dictionaries.
        /// </summary>
        private void GenerateItemDataDict()
        {
            for (int i = 0; i < _itemData.Count; i++)
            {
                if (_itemData[i] != null && itemDataDict.ContainsKey(_itemData[i]))
                    Debug.LogError($"[ItemDictionary]: \tItem at {i} already exist.");
                else
                {
                    itemDataDict.Add(_itemData[i], i);
                    itemDataById.Add(i, _itemData[i]);
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



        #region Item Prefab
        private GameObject GetItemPrefabType<T>()
        {
            for (int i = 0; i < _itemPrefabs.Count; i++)
            {
                if (_itemPrefabs[i].GetComponent<T>() != null)
                {
                    return _itemPrefabs[i];
                }
            }
            return null;
        }
        public GameObject GetItemPrefab(ItemType itemType)
        {
            GameObject itemPrefab = null;
            switch (itemType)
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
        #endregion Item Prefab





        #region Item Recipe
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
        #endregion Item Recipe




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




        #region Enemies
        private GameObject GetEnemyPrefabByType<T>()
        {
            for (int i = 0; i < _enemyPrefabs.Count; i++)
            {
                if (_enemyPrefabs[i].GetComponent<T>() != null)
                {
                    return _enemyPrefabs[i];
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
        #endregion Enemies



        #region Bosses
        private GameObject GetBossPrefabByType<T>()
        {
            for (int i = 0; i < _bossPrefabs.Count; i++)
            {
                if (_bossPrefabs[i].GetComponent<T>() != null)
                {
                    return _bossPrefabs[i];
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
        #endregion Bosses


        public Sataura.Debuff GetDebuffEffectVFXPrefabs(DebuffEffect debuffEffect)
        {
            switch (debuffEffect)
            {
                case DebuffEffect.Slowly:
                    for (int i = 0; i < _debuffVFXPrefabs.Count; i++)
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
                    projectilePrefab = GetProjectilePrefabByType<WoodenSwordProjectile>();
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
            for (int i = 0; i < _projectilesPrefab.Count; i++)
            {
                if (_projectilesPrefab[i].GetComponent<T>() != null)
                {
                    return _projectilesPrefab[i];
                }
            }
            return null;
        }


        public void AddNetworkPlayer(ulong clientId, IngamePlayer player)
        {
            if (players.ContainsKey(clientId) == false)
                players.Add(clientId, player);
        }

    }
}