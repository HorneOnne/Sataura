﻿/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Sataura
{
    public class ParallaxBackground : MonoBehaviour
    {

        [SerializeField] private Vector2 parallaxEffectMultiplier;
        [SerializeField] private bool infiniteHorizontal;
        [SerializeField] private bool infiniteVertical;

        private Transform cameraTransform;
        private Vector3 lastCameraPosition;
        private float textureUnitSizeX;
        private float textureUnitSizeY;

        private float textureUnitSizeXAfterScale;
        private float textureUnitSizeYAfterScale;

        private void Start()
        {
            cameraTransform = Camera.main.transform;
            lastCameraPosition = cameraTransform.position;
            Sprite sprite = GetComponent<SpriteRenderer>().sprite;
            Texture2D texture = sprite.texture;
            textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
            textureUnitSizeY = texture.height / sprite.pixelsPerUnit;

            textureUnitSizeXAfterScale = textureUnitSizeX * transform.localScale.x;
            textureUnitSizeYAfterScale = textureUnitSizeY * transform.localScale.y;


            GetComponent<SpriteRenderer>().size = new Vector2(textureUnitSizeX * 3, textureUnitSizeY);
        }
        

        private void LateUpdate()
        {
            Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
            transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier.x, deltaMovement.y * parallaxEffectMultiplier.y);
            lastCameraPosition = cameraTransform.position;

            if (infiniteHorizontal)
            {
                if (Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeXAfterScale)
                {
                    float offsetPositionX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeXAfterScale;
                    transform.position = new Vector3(cameraTransform.position.x + offsetPositionX, transform.position.y);
                }
            }

            if (infiniteVertical)
            {
                if (Mathf.Abs(cameraTransform.position.y - transform.position.y) >= textureUnitSizeYAfterScale)
                {
                    float offsetPositionY = (cameraTransform.position.y - transform.position.y) % textureUnitSizeYAfterScale;
                    transform.position = new Vector3(transform.position.x, cameraTransform.position.y + offsetPositionY);
                }
            }
        }

    }
}
