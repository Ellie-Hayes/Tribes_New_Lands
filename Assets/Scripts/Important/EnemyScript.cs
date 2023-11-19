using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class EnemyScript : MonoBehaviour
{
    [Header("Attributes")]
    public float speed;
    public int health;
    public int damage = 100;
    public bool DaySpawned;

    bool DayMoveBack = false;
    float spawnpoint;
    bool SpawnedLeft;


    [Header("Animations")]
    bool isdead;
    bool hasberry;
    bool hasStar;
    GameObject berryOrStar;

    [Header("Attacking")]
    GameObject target;
    float attackcooldown;
    bool canAttack;
    bool AttackableInRange;
    bool incoroutine;

    bool attachedToPlayer;
    public bool wait;
    GameObject player;
    public List<GameObject> Attackables;

    public Transform groundSummonCheck;
    private bool MustAttack;
    public LayerMask Tribelayer;
    public LayerMask wallLayer;
    public LayerMask PlayerLayer;

    [Header("Scripts")]
    GlobalData globalData;
    Animator anim;

    private static int layer = 6;
    //[MenuItem("Tools/Select Objects in Layer 31", false, 50)]

    private void Awake()
    {
        GameObject[] archers = GameObject.FindGameObjectsWithTag("Spearman");

        foreach (var archer in archers)
        {
            TribeAI tribe = archer.GetComponent<TribeAI>();
            tribe.ArcherEnemiesUpdate();
        }
    }

    void Start()
    {
        globalData = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GlobalData>();
        anim = GetComponent<Animator>();
        if (transform.position.x < 0) { SpawnedLeft = true; }
        else { SpawnedLeft = false; }
        spawnpoint = transform.position.x;

        Physics2D.IgnoreLayerCollision(6, 9);
        Physics2D.IgnoreLayerCollision(8, 9);
        Physics2D.IgnoreLayerCollision(9, 9);
        Physics2D.IgnoreLayerCollision(9, 3);
        Physics2D.IgnoreLayerCollision(9, 11);
        

        Attackables = new List<GameObject>();

        SelectObjectsInLayer();
    }

    // Update is called once per frame
    void Update()
    {
        enemyMovement();
        animations();

        if (health <= 0)
        {
            if (hasberry)
            {
                Rigidbody2D berryrigid = berryOrStar.GetComponent<Rigidbody2D>();
                BoxCollider2D berrycoll = berryOrStar.GetComponent<BoxCollider2D>();
                berryrigid.isKinematic = false;
                berrycoll.enabled = true;
                berryOrStar.transform.SetParent(null);
            }

            Destroy(gameObject);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            foreach (var enemy in Attackables)
            {
                Debug.Log(enemy);
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            health = 0;
        }
        Debug.Log(attachedToPlayer);

        if (Input.GetKeyDown(KeyCode.M))
        {
            foreach (var x in Attackables)
            {
                Debug.Log(x);
            }
        }

        
    }

    private void FixedUpdate()
    {
        MustAttack = Physics2D.OverlapCircle(groundSummonCheck.position, 0.5f, wallLayer) || Physics2D.OverlapCircle(groundSummonCheck.position, 0.5f, Tribelayer) || Physics2D.OverlapCircle(groundSummonCheck.position, 0.5f, PlayerLayer);

        Debug.Log(MustAttack);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (SpawnedLeft && DaySpawned) { if (transform.position.x > spawnpoint + 20) { attachedToPlayer = false; } }
            else if (!SpawnedLeft && DaySpawned) { if (transform.position.x > spawnpoint - 20) { attachedToPlayer = false; } }
            else
            {
                Debug.Log("ontrigger");
                attachedToPlayer = true;
                player = collision.gameObject;
            }
            
        }
        if (collision.gameObject.CompareTag("Pickups") && !hasberry)
        {
            hasberry = true;
            collision.gameObject.transform.position = new Vector2(transform.position.x, transform.position.y + 2);
            collision.gameObject.transform.parent = transform;
            Rigidbody2D berryrigid = collision.gameObject.GetComponent<Rigidbody2D>();
            BoxCollider2D berrycoll = collision.gameObject.GetComponent<BoxCollider2D>();
            berryrigid.isKinematic = true;
            berrycoll.enabled = false;
            berryOrStar = collision.gameObject;
        }
    }

    void enemyMovement()
    {
        if (!wait && !hasberry)
        {
            if (!attachedToPlayer)
            {
                if (!DaySpawned)
                {
                    if (globalData.IsNight)
                    {
                        if (SpawnedLeft)
                        {
                            transform.position = new Vector3(transform.position.x + speed, transform.position.y, transform.position.z * Time.deltaTime);
                            transform.localScale = new Vector2(1, 1);
                        }
                        if (!SpawnedLeft)
                        {
                            transform.position = new Vector3(transform.position.x - speed, transform.position.y, transform.position.z * Time.deltaTime);
                            transform.localScale = new Vector2(-1, 1);
                        }
                    }

                    if (!globalData.IsNight && !globalData.paused) { moveBack(); }
                }

                if (DaySpawned)
                {

                    if (SpawnedLeft && !DayMoveBack)
                    {
                        transform.position = new Vector3(transform.position.x + speed, transform.position.y, transform.position.z * Time.deltaTime);
                        transform.localScale = new Vector2(1, 1);

                        if (transform.position.x > spawnpoint + 20)
                        {
                            DayMoveBack = true;
                            attachedToPlayer = false;
                        }

                    }
                    if (!SpawnedLeft && !DayMoveBack)
                    {
                        transform.position = new Vector3(transform.position.x - speed, transform.position.y, transform.position.z * Time.deltaTime);
                        transform.localScale = new Vector2(-1, 1);

                        if (transform.position.x > spawnpoint - 20)
                        {
                            DayMoveBack = true;
                            attachedToPlayer = false;
                        }
                    }
                    if (DayMoveBack)
                    {
                        moveBack();
                    }



                }
            }
            else
            {
                Vector2 newplace = new Vector2(player.transform.position.x, transform.position.y);
                if (!wait)
                {
                    transform.position = Vector2.MoveTowards(transform.position, newplace, speed);
                }

                if (player.transform.position.x > transform.position.x) { transform.localScale = new Vector2(1, 1); }
                if (player.transform.position.x <= transform.position.x) { transform.localScale = new Vector2(-1, 1); }

            }
        }

        if (attachedToPlayer && Vector2.Distance(transform.position, player.transform.position) > 5f)
        {
            attachedToPlayer = false;
            AttackableInRange = false;
        }

        //if ((SpawnedLeft && transform.position.x >= (globalData.villagecoords[0] - 2)) && globalData.leftSideBuiltOn)
        //{
        //    transform.position = new Vector2(globalData.villagecoords[0] - 2, transform.position.y);
        //}
        //if ((!SpawnedLeft && transform.position.x <= (globalData.villagecoords[0] + 2)) && globalData.rightSideBuiltOn)
        //{
        //    transform.position = new Vector2(globalData.villagecoords[0] + 2, transform.position.y);
        //}

        if (hasberry)
        {
            moveBack();
        }
    }

    void moveBack()
    {
        Vector2 newplace = new Vector2(spawnpoint, transform.position.y);

        if (DaySpawned && !hasberry)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (SpawnedLeft && player.transform.position.x < spawnpoint + 20 )
            {
                newplace = new Vector2(player.transform.position.x, transform.position.y);
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, newplace, speed);
        if (Vector2.Distance(transform.position, newplace) < 0.1f) { Destroy(gameObject); }

        if (SpawnedLeft) { transform.localScale = new Vector2(-1, 1); }
        else { transform.localScale = new Vector2(1, 1); }
    }

    public void Attack()
    {
        incoroutine = true;

        GameObject[] unitysucks = Attackables.ToArray();

        for (int i = 0; i < unitysucks.Length; i++) // um like cycles through the gameobject list the enemy can attack and if they are close enough he attacks 
        {
            Debug.Log(unitysucks[i]);
            if (unitysucks[i] == null)
            {
                wait = false;
                //yield break;
            }
            else if ((Vector2.Distance(transform.position, unitysucks[i].transform.position) <= 2.5))// the 5f is its reach
            {
                if (unitysucks[i].name == "Player")
                {
                    wait = false;
                    GameObject player = unitysucks[i];
                    PlayerController playerController = unitysucks[i].GetComponent<PlayerController>(); //different GO have differebt health systems so it gets it here
                    playerController.TakeDamage(damage);
                    playerController.getAttacker(gameObject);
                    if (playerController.health <= 0) { Attackables.Remove(player); }
                }
                if (unitysucks[i].layer == 6)
                {
                    wait = false;
                    GameObject tribeGO = unitysucks[i];
                    TribeAI tribe = unitysucks[i].GetComponent<TribeAI>();//different GO have differebt health systems so it gets it here
                    tribe.TakeDamage(damage);
                    tribe.getAttacker(gameObject);
                    if (tribe.health <= 0) { Attackables.Remove(tribeGO); }

                }
                if (unitysucks[i].tag == "Wall")
                {
                    GameObject wall = unitysucks[i];
                    wait = false;
                    Buy buy = unitysucks[i].GetComponent<Buy>();//different GO have differebt health systems so it gets it here
                    buy.WallTakeDamage(damage);
                    if (buy.wallheath[buy.currentwalltype] <= 0) { Attackables.Remove(wall); }
                }
                //yield break;
            }
        }
        /*
        foreach (var attackable in Attackables)
        {
            Debug.Log(attackable);
            if (attackable == null)
            {
                wait = false;
                //break;
            }
            else if ((Vector2.Distance(transform.position, attackable.transform.position) <= 400))// the 5f is its reach
            {
                if (attackable.name == "Player")
                {
                    wait = false;
                    GameObject player = attackable;
                    PlayerController playerController = attackable.GetComponent<PlayerController>(); //different GO have differebt health systems so it gets it here
                    playerController.TakeDamage(damage);
                    playerController.getAttacker(gameObject);
                    if (playerController.health <= 0) { Attackables.Remove(player); }
                }
                if (attackable.layer == 6)
                {
                    wait = false;
                    GameObject tribeGO = attackable;
                    TribeAI tribe = attackable.GetComponent<TribeAI>();//different GO have differebt health systems so it gets it here
                    tribe.TakeDamage(damage);
                    tribe.getAttacker(gameObject);
                    if (tribe.health <= 0) { Attackables.Remove(tribeGO); }

                }
                if (attackable.tag == "Wall")
                {
                    GameObject wall = attackable;
                    wait = false;
                    Buy buy = attackable.GetComponent<Buy>();//different GO have differebt health systems so it gets it here
                    buy.WallTakeDamage(damage);
                    if (buy.wallheath[buy.currentwalltype] <= 0) { Attackables.Remove(wall); }
                }
                return;
            }
        }
        
        for (int i = 0; i < Attackables.Count; i++) // um like cycles through the gameobject list the enemy can attack and if they are close enough he attacks 
        {
            Debug.Log(Attackables[i]);
            if (Attackables[i] == null)
            {
                wait = false;
                break;
            }
            if ((Vector2.Distance(transform.position, Attackables[i].transform.position) <= 400))// the 5f is its reach
            {
                if (Attackables[i].name == "Player")
                {
                    wait = false;
                    GameObject player = Attackables[i];
                    PlayerController playerController = Attackables[i].GetComponent<PlayerController>(); //different GO have differebt health systems so it gets it here
                    playerController.TakeDamage(damage);
                    playerController.getAttacker(gameObject);
                    if (playerController.health <= 0) { Attackables.Remove(player); }
                }
                if (Attackables[i].layer == 6)
                {
                    wait = false;
                    GameObject tribeGO = Attackables[i];
                    TribeAI tribe = Attackables[i].GetComponent<TribeAI>();//different GO have differebt health systems so it gets it here
                    tribe.TakeDamage(damage);
                    tribe.getAttacker(gameObject);
                    if (tribe.health <= 0) { Attackables.Remove(tribeGO); }

                }
                if (Attackables[i].tag == "Wall")
                {
                    GameObject wall = Attackables[i];
                    wait = false;
                    Buy buy = Attackables[i].GetComponent<Buy>();//different GO have differebt health systems so it gets it here
                    buy.WallTakeDamage(damage);
                    if (buy.wallheath[buy.currentwalltype] <= 0) { Attackables.Remove(wall); }
                }
                return;
            }
        }*/


        wait = false;
    }

    public void StartAttack()
    {
        wait = true;
    }

    void animations()
    {
        anim.SetBool("IsAttacking", MustAttack);
    }

    public void TakeDamage(int damage) // damage to the enemy
    {
        health -= damage;
    }

    // Listadding - basically adds all like gameobjects the enemy can attack to a list which this code deals with 
    public void SelectObjectsInLayer()
    {
        var objects = GetSceneObjects();
        GetObjectsInLayer(objects, layer);
    }

    private static GameObject[] GetSceneObjects()
    {
        return Resources.FindObjectsOfTypeAll<GameObject>()
                .Where(go => go.hideFlags == HideFlags.None).ToArray();
    }

    private void GetObjectsInLayer(GameObject[] root, int layer)
    {
        foreach (GameObject t in root)
        {
            if (t.layer == layer)
            {
                Attackables.Add(t);
            }
        }

        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");

        foreach (var wall in walls)
        {
            Attackables.Add(wall);
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        Attackables.Add(player);
    }

    IEnumerator EnemyAttack()
    {
        
        if (!incoroutine)
        {
            incoroutine = true;

            GameObject[] unitysucks = Attackables.ToArray();

            for (int i = 0; i < unitysucks.Length; i++) // um like cycles through the gameobject list the enemy can attack and if they are close enough he attacks 
            {
                Debug.Log(unitysucks[i]);
                if (unitysucks[i] == null)
                {
                    wait = false;
                    //yield break;
                }
                if ((Vector2.Distance(transform.position, unitysucks[i].transform.position) <= 400))// the 5f is its reach
                {
                    if (unitysucks[i].name == "Player")
                    {
                        wait = false;
                        GameObject player = unitysucks[i];
                        PlayerController playerController = unitysucks[i].GetComponent<PlayerController>(); //different GO have differebt health systems so it gets it here
                        playerController.TakeDamage(damage);
                        playerController.getAttacker(gameObject);
                        if (playerController.health <= 0) { Attackables.Remove(player); }
                    }
                    if (unitysucks[i].layer == 6)
                    {
                        wait = false;
                        GameObject tribeGO = unitysucks[i];
                        TribeAI tribe = unitysucks[i].GetComponent<TribeAI>();//different GO have differebt health systems so it gets it here
                        tribe.TakeDamage(damage);
                        tribe.getAttacker(gameObject);
                        if (tribe.health <= 0) { Attackables.Remove(tribeGO); }

                    }
                    if (unitysucks[i].tag == "Wall")
                    {
                        GameObject wall = unitysucks[i];
                        wait = false;
                        Buy buy = unitysucks[i].GetComponent<Buy>();//different GO have differebt health systems so it gets it here
                        buy.WallTakeDamage(damage);
                        if (buy.wallheath[buy.currentwalltype] <= 0) { Attackables.Remove(wall); }
                    }
                    //yield break;
                }
            }

            incoroutine = false;
            yield return null;
        }
        
    }


}
