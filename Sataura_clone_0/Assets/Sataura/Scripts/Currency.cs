using UnityEngine;
using Unity.Netcode;
using System.Numerics;

namespace Sataura
{
    public class Currency : NetworkBehaviour, ICollectible
    {
        [Header("Values")]
        [Range(1,99)]
        public int coinValue;

        

        [SerializeField] private CurrencyType currencyType;


        [Header("References")]
        [SerializeField] Animator animator;
        [SerializeField] Rigidbody2D rb2D;


        #region Properties
        public int TotalCurrency
        {
            get
            {
                int totalCurrency = 0;

                switch (currencyType)
                {
                    case CurrencyType.Gold:
                        totalCurrency = coinValue * 10000;
                        break;
                    case CurrencyType.Sliver:
                        totalCurrency = coinValue * 100;
                        break;
                    case CurrencyType.Bronze:
                        totalCurrency = coinValue * 1;
                        break;
                    default:
                        break;
                }

                return totalCurrency;
            }
        }

        public Rigidbody2D Rb2D { get => rb2D; }


        #endregion

        public override void OnNetworkSpawn()
        {
            Debug.Log("OnNetworkSpawn");
            Invoke("DisableAnimator", 5f);
        }

        private void Start()
        {
            Invoke("DisableAnimator", 5f);
            Invoke("TurnOnPhysicKinematic", 5f);
        }



        public void Collect(Player player)
        {
            // Start Testing purpose
            if (player.playerData.currencyString == "")
                player.playerData.currency = 0;
            // End Testing purpose

            Debug.Log($"Total Currency: {TotalCurrency}");

            player.playerData.currency += new BigInteger(TotalCurrency);
            player.playerData.currencyString = AA(player.playerData.currency);
        }

        private string AA(BigInteger veryLargeNumber)
        {
            if (veryLargeNumber >= new BigInteger(1000000000000000000))
            {
                return (veryLargeNumber / new BigInteger(1000000000000000000)).ToString("0.#") + "ab";
            }
            if (veryLargeNumber >= new BigInteger(1000000000000000))
            {
                return (veryLargeNumber / new BigInteger(1000000000000000)).ToString("0.#") + "aa";
            }
            if (veryLargeNumber >= new BigInteger(1000000000000))
            {
                return (veryLargeNumber / new BigInteger(1000000000000)).ToString("0.#") + "T";
            }
            if (veryLargeNumber >= 1000000000)
            {
                return (veryLargeNumber / 1000000000).ToString("0.#") + "B";
            }
            if (veryLargeNumber >= 1000000)
            {
                return (veryLargeNumber / 1000000).ToString("0.#") + "M";
            }
            if (veryLargeNumber >= 1000)
            {
                return (veryLargeNumber / 1000).ToString("0.#") + "K";
            }


            return veryLargeNumber.ToString();
        }



        private void DisableAnimator()
        {
            animator.enabled = false;
        }

        private void TurnOnPhysicKinematic()
        {
            rb2D.isKinematic = true;
        }
    }
}



