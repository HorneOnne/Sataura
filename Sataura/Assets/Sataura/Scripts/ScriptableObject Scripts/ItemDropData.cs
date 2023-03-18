using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "ItemDropData", menuName = "UltimateItemSystem/ItemDropData", order = 51)]
    public class ItemDropData : ScriptableObject
    {
        public List<ItemDrop> itemDrops;
        public int currencyDropFrom;
        public int currencyDropTo;


        /*
         * Base unit: Bronze
         * 
         * 100 Bronze = 1 Sliver
         * 100 Sliver = 1 Gold
         * 
         * Ex: currency = 234 (2 sliver + 34 bronze)
         */
    }

}
