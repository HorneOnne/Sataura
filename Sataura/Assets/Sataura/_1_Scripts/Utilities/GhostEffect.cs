using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Sataura
{
    public class GhostEffect : MonoBehaviour
    {
        [SerializeField] private GameObject ghostPrefab;
        [SerializeField] private SpriteRenderer sr;


        private float createGhostTime = .05f;
        private float createGhostTimeCount = 0.0f;
        private void Update()
        {
            if(Time.time - createGhostTimeCount > createGhostTime)
            {
                createGhostTimeCount = Time.time;
                CreateGhost();
            }
        }



        private void CreateGhost()
        {
            var ghostObject = Instantiate(ghostPrefab, transform.position, transform.rotation);
            ghostObject.transform.localScale = sr.gameObject.transform.localScale;
            Destroy(ghostObject, 0.3f);

            ghostObject.GetComponent<SpriteRenderer>().sprite = sr.sprite;
        }

    }

}

