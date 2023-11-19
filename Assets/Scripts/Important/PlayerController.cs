using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Attributes")]
    public float speed;
    public Vector3 jumpForce;
    private Rigidbody2D rb;
    public Animator transition;
    public Vector3 offset;
    public int health;
    float healthRegen;
    int maxHealth;
    private Renderer rend;
    public Renderer arrowRend;
    private Color grey;
    public GameObject HealthArrowDisplay;
    public bool InTransaction; 

    [Header("Animation parameters")]
    private Animator anim;
    private bool OnGround = true;

    [Header("Misc")]
    public GameObject Meat;
    private bool facingLeft;
    GlobalData globalData;
    bool knockedback;
    public GameObject fadeOverlay;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        globalData = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GlobalData>();
        rend = GetComponent<Renderer>();
        arrowRend = HealthArrowDisplay.GetComponent<Renderer>();
        maxHealth = health;

        Physics2D.IgnoreLayerCollision(6, 11);
        Physics2D.IgnoreLayerCollision(11, 3);
        Physics2D.IgnoreLayerCollision(8, 8);
    }

    void Update()
    {
        if (!globalData.paused && !knockedback)
        {
            if (!InTransaction)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    transform.position = new Vector3(transform.position.x - speed, transform.position.y, transform.position.z * Time.deltaTime);
                    transform.localScale = new Vector2(1, 1);
                    facingLeft = true;


                }
                if (Input.GetKey(KeyCode.D))
                {
                    transform.position = new Vector3(transform.position.x + speed, transform.position.y, transform.position.z * Time.deltaTime);
                    transform.localScale = new Vector2(-1, 1);
                    facingLeft = false;

                }
                if (Input.GetKey(KeyCode.W) && OnGround == true)
                {
                    rb.AddForce(jumpForce);
                    OnGround = false;
                }
            }
            
            if (Input.GetKeyDown(KeyCode.DownArrow) && globalData.foodAmount > 0)
            {
                GameObject SpawnedMeat = (GameObject)Instantiate(Meat, transform.position + offset, transform.rotation);
                BerrySpawn berrySpawn = SpawnedMeat.GetComponent<BerrySpawn>();
                if (facingLeft) { berrySpawn.popdirection = 0; }
                if (!facingLeft) { berrySpawn.popdirection = 1; }

                berrySpawn.playerSpawned = true;
                globalData.foodAmount -= 1;

            }
        }

        if (health <= 0)
        {
            Destroy(gameObject);
            Menus menu = fadeOverlay.GetComponent<Menus>();
            menu.Fadeout = true;
        }

        anim.SetBool("OnGround", OnGround);
        float moveHorizonal = Input.GetAxis("Horizontal");
        anim.SetFloat("horizontalSpeed", Mathf.Abs(moveHorizonal));
        healthArrow();

    }
   
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            OnGround = true;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        
    }

    public void getAttacker(GameObject enemy)
    {
        StartCoroutine("Attacked");
        if (enemy.transform.position.x > transform.position.x) { rb.AddForce(new Vector2(-200, 0)); }
        if (enemy.transform.position.x < transform.position.x) { rb.AddForce(new Vector2(200, 0)); }
        knockedback = true;
        Debug.Log("getattacker");
    }

    IEnumerator Attacked()
    {
        Debug.Log("yarrr");

        grey = new Color32(255, 85, 85, 255);
        rend.material.color = grey;
        yield return new WaitForSeconds(0.5f);
        rend.material.color = new Color32(255, 255, 255, 255);
        knockedback = false;
    }

    void healthArrow()
    {
        if (health < (maxHealth / 3) * 2)
        {
            if (health < maxHealth / 3) { arrowRend.material.color = new Color32(168, 87, 97, 255); }
            else { arrowRend.material.color = new Color32(188, 187, 127, 255); }
        }
        else { arrowRend.material.color = new Color32(127, 188, 129, 255); }

        if (!globalData.IsNight)
        {
            healthRegen += Time.deltaTime * 0.5f;

            if (healthRegen >= 1 && health < maxHealth)
            {
                health += 1;
                healthRegen = 0;
            }
        }
    }
}
