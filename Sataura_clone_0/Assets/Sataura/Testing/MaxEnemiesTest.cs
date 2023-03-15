using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public class MaxEnemiesTest : MonoBehaviour
{
    [SerializeField] Transform prefab;

    private List<Enemy001> enemies;

    private void Start()
    {
        enemies = new List<Enemy001>();
        float xPos;
        float yPos;

        for(int i = 0; i < 10000; i++)
        {
            xPos = Random.Range(-400, 400);
            yPos = Random.Range(-400, 400);

            Transform e = Instantiate(prefab, new Vector2(xPos, yPos), Quaternion.identity);
            enemies.Add(e.GetComponent<Enemy001>());
        }
    }

    Vector2 mousePos;
    Vector2 direction;
    float distance;
    int count = 0;

    private void FixedUpdate()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        count = 0;

        for (int i = 0; i < 10000; i++)
        {
            distance = Vector2.Distance(mousePos, enemies[i].rb2D.position);

          
            if (distance < 1f)
            {
                count++;
                continue;
            }
            if (distance < 200)
            {
                enemies[i].rb2D.simulated = true;

                direction = mousePos - enemies[i].rb2D.position;

                // Normalize the direction to get a unit vector
                direction.Normalize();

                enemies[i].rb2D.MovePosition(enemies[i].rb2D.position + direction * 5 * Time.fixedDeltaTime);
                count++;
                
            }     
            else
            {
                enemies[i].rb2D.simulated = false;
            }

            if(count >= 3000)
            {
                break;
            }
        }
    }


    

}
