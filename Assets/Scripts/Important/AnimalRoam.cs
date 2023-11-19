using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalRoam : MonoBehaviour
{
    public float speed;
    public int health;
    bool waiting;
    bool hasplacetomove;
    float PointToMove;
    public bool walking;
    float waitcountdown;
    public Animator anim;
    public GameObject foodPrefab;

    private void Awake()
    {
        GameObject[] archers = GameObject.FindGameObjectsWithTag("Spearman");

        foreach (var archer in archers)
        {
            TribeAI tribe = archer.GetComponent<TribeAI>();
            tribe.ArcherEnemiesUpdate();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!waiting)
        {
            if (!hasplacetomove)
            {
                PointToMove = Random.Range(transform.position.x - 20, transform.position.x + 20);
                hasplacetomove = true;

            }

            walking = true;
            Vector2 newplace = new Vector2(PointToMove, transform.position.y);
            if (newplace.x >= transform.position.x) { transform.localScale = new Vector2(-1, 1); }
            if (newplace.x < transform.position.x) { transform.localScale = new Vector2(1, 1); }
            transform.position = Vector2.MoveTowards(transform.position, newplace, speed);

            if (Vector2.Distance(transform.position, newplace) < 0.1f)
            {
                waiting = true;
                waitcountdown = 10f;
                hasplacetomove = false;
            }


        }

        if (waiting)
        {
            waitcountdown -= Time.deltaTime;
            walking = false;
            if (waitcountdown <= 0f)
            {
                Debug.Log("Notwaiting");
                waiting = false;

            }
        }

        SetAnimations();

        if (health <= 0)
        {
            Instantiate(foodPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.layer == 6)
        {
            if (collision.gameObject.transform.position.x > transform.position.x) { PointToMove = transform.position.x - 15; }
            else { PointToMove = transform.position.x + 15; }
            
            hasplacetomove = true;
            waiting = false;
        }
    }

    void SetAnimations()
    {
        anim.SetBool("IsWalking", walking);
    }

    public void TakeDamage(int damage) // damage to the enemy
    {
        health -= damage;
    }


}
