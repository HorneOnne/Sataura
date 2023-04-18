using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sataura
{
    public class UIHealthBar : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                transform.SetParent(GameDataManager.Instance.singleModePlayer.transform);
            }
        }
    }
}

