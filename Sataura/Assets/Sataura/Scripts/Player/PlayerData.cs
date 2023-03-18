using UnityEngine;
using System.Numerics;

namespace Sataura
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Sataura/Player/PlayerData")]
    public class PlayerData : ScriptableObject
    {      
        public BigInteger currency;
        public string currencyString;
    }
}