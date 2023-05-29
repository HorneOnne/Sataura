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
            Invoke("DisableAnimator", Random.Range(3f, 5f));
            Invoke("TurnOnPhysicKinematic", 5f);
        }

        /*private void Start()
        {
            Invoke("DisableAnimator", Random.Range(3f, 5f));
            Invoke("TurnOnPhysicKinematic", 5f);
        }*/



        public void Collect(IngamePlayer player)
        {
            // Start Testing purpose
            if (player.characterData.currencyString == "")
                player.characterData.currency = 0;
            // End Testing purpose

            player.characterData.currency += new BigInteger(TotalCurrency);
            player.characterData.currencyString = AA(player.characterData.currency);
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



