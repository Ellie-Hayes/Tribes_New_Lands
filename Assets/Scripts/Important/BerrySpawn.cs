using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerrySpawn : MonoBehaviour
{
    private Rigidbody2D rb;
    public Vector3 pop;
    bool playedAnim;
    public GameObject player;
    public int popdirection;
    public bool playerSpawned;
    public bool fromUI;
    private BoxCollider2D boxcoll;

    private GlobalData globaldata;
    public bool OnGround;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        globaldata = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GlobalData>();
        boxcoll = GetComponent<BoxCollider2D>();

        if (fromUI)
        {
            rb.isKinematic = true;
            boxcoll.enabled = false;
        }
    }

    private void Update()
    {
        if (!OnGround && !fromUI)
        {
            player = GameObject.FindGameObjectWithTag("Player"); 
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player.GetComponent<Collider2D>());
        }
        
        if (!playedAnim && !fromUI)
        {
            float randompopamount = Random.Range(60, 130);
            if (!playerSpawned) { popdirection = (int)Random.Range(0, 2); }
            if (popdirection == 1) { pop = new Vector3(-100, randompopamount, 0); }
            if (popdirection == 0) { pop = new Vector3(100, randompopamount, 0); }

            rb.AddForce(pop);
            playedAnim = true;

            GameObject[] otherObjects = GameObject.FindGameObjectsWithTag("Pickups");

            foreach (GameObject obj in otherObjects)
            {
                Physics2D.IgnoreCollision(obj.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            }
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            OnGround = true;
            player = GameObject.FindGameObjectWithTag("Player");
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player.GetComponent<Collider2D>(), false);
           
        }

        if (collision.gameObject.CompareTag("Player") && OnGround)
        {
            globaldata.foodAmount += 1;
            Destroy(gameObject);
        }
    }

    public void DropUI()
    {
        rb.isKinematic = false;
        boxcoll.enabled = true;
        fromUI = false;
        playedAnim = true;
    }
}
