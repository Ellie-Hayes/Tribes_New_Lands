using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCheck : MonoBehaviour
{
    TribeAI parentTribeScript;
    public bool archercheck;
    public bool forestcheck;
    GameObject archerTarget;

    public Transform EnemyCheckCircle;
    public LayerMask enemyLayer;
    bool incoroutine; 

    void Start()
    {
        parentTribeScript = GetComponentInParent<TribeAI>();
    }

    private void Update()
    {
    }

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && (parentTribeScript.foodInInventory >= 1))
        {
            parentTribeScript.StartCoroutine("SpawnInventory");
        }
        if (collision.gameObject.CompareTag("Pickups"))
        {
            Debug.Log("Pickup pickup");
            BerrySpawn berryscript = collision.gameObject.GetComponent<BerrySpawn>();
            if (berryscript.OnGround)
            {
                if (!forestcheck)
                {
                    parentTribeScript.foodInInventory += 1;
                    Destroy(collision.gameObject);
                }
                
            }
            if (forestcheck)
            {
                parentTribeScript.forestMove = true;
                parentTribeScript.berrytomove = collision.gameObject;
            }

        }
        if (collision.gameObject.layer == 9)
        {
            if (collision.gameObject.transform.position.x > transform.position.x) { parentTribeScript.PointToMove = transform.position.x - 15; }
            else { parentTribeScript.PointToMove = transform.position.x + 15; }

            parentTribeScript.hasplacetomove = true;
            parentTribeScript.waiting = false;

            
        }

        if (collision.gameObject.layer == 9 && archercheck)
        {
            parentTribeScript.GetTarget(collision.gameObject);
            parentTribeScript.shoot();
            parentTribeScript.attackableInRange = true;
        }



    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!archercheck)
        {
            if (collision.gameObject.CompareTag("Pickups"))
            {
                Debug.Log("Pickup pickup");
                BerrySpawn berryscript = collision.gameObject.GetComponent<BerrySpawn>();
                if (berryscript.OnGround)
                {
                    parentTribeScript.foodInInventory += 1;
                    Destroy(collision.gameObject);
                }

            }
        }
        
       
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (archercheck)
        {
            parentTribeScript.attackableInRange = false;
        }
    }

    
}
