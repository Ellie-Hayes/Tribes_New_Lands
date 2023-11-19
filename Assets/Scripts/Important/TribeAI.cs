using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TribeAI : MonoBehaviour
{
    [Header("General")]
    Renderer rend;
    Rigidbody2D rb;
    public GameObject player;
    public float speed;
    public Vector3 offset;
    private GlobalData globaldata;
    public float PointToMove;
    public bool waiting = false;
    float waitcountdown;
    bool NotDoingJob = true;
    public bool hasplacetomove;
    bool knockedback;
    Vector2 newplace;

    [Header("Builder specifics")]
    private Transform placeToMove;
    private GameObject TargetToMove;
    public bool Builder;
    public bool MoveToConstruct;
    public float stoppingDistance;

    [Header("Spearman specifics")]
    public bool Spearman;
    public GameObject pylon;
    GameObject target;
    public bool attackableInRange;
    public List<GameObject> AttackableEnemies;
    bool inAttackCoroutine;
    bool archerpleasestopmoving = true;

    //public Transform EnemyCheckCircle;
    //public LayerMask enemyLayer;

    [Header("Uncategorised specifics")]
    public bool jobless;
    private float searchCountdown = 2;
    public string TagString;
    private Buy Benchscript;
    private FarmScript farmscript;

    public GameObject farmerPrefab;
    public GameObject builderPrefab;
    public GameObject archerPrefab;

    [Header("Farmer specifics")]
    public bool Farmer;
    public bool HasFarm;
    public GameObject mapfarm;
    public int foodInInventory;
    public int foodamountspawned;
    private float harvestcountdown = 10f;
    bool incoroutine;
    public GameObject foodprefab;
    List<GameObject> spawnedfoodList;
    bool GetWheat;
    int berryListNumber;

    [Header("Forest tribe")]
    public bool forest;
    public bool forestMove;
    public GameObject berrytomove;
    public GameObject joblessPrefab;
    public GameObject campCenterpoint;

    [Header("Anim bools")]
    private Animator anim;
    public bool Isattacking;
    bool IsIdle;
    bool IsIdleanim;
    bool IsWalking;
    bool IsDead;
    bool isfacingeft;

    public Transform groundSummonCheck;
    public bool MustAttack;
    public LayerMask Tribelayer;

    [Header("Health")]
    public int health;

    void Start()
    {
        anim = GetComponent<Animator>();
        globaldata = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GlobalData>();
        spawnedfoodList = new List<GameObject>();
        player = GameObject.FindGameObjectWithTag("Player");
        rend = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody2D>();

        Physics2D.IgnoreLayerCollision(6, 8);
        Physics2D.IgnoreLayerCollision(6, 6);
        Physics2D.IgnoreLayerCollision(3, 6);
        Physics2D.IgnoreLayerCollision(8, 8);
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player.GetComponent<Collider2D>());

        AttackableEnemies = new List<GameObject>();
    }

    void Update()
    {
        if (!globaldata.paused)
        {
            if ((foodInInventory >= 1) && (transform.position.x == player.transform.position.x))
            {
                Debug.Log("PlayerHit");
                StartCoroutine("SpawnInventory");
            }

            BuilderMove();
            joblessAI();
            FarmerAI();
            timer();
            AIroam();
            bread();
            FallBack();
            forestTribeMove();

            if (isfacingeft) { transform.localScale = new Vector2(1, 1); }
            if (!isfacingeft) { transform.localScale = new Vector2(-1, 1); }

            animations();
        }

        if (health <= 0) { Destroy(gameObject); }

        if (attackableInRange)
        {
            Isattacking = true;
            NotDoingJob = false;
            IsWalking = false;
            IsIdleanim = false;
        }
        else if((!globaldata.IsNight) && Spearman && !attackableInRange)
        {
            Isattacking = false;
            NotDoingJob = true;
        }
        if (Spearman && target == null)
        {
            attackableInRange = false;
            Isattacking = false;
            NotDoingJob = true;
        }

        if (globaldata.IsNight && Spearman)
        {
            NotDoingJob = false;

            if (transform.position.x <= 0)
            {
                newplace = new Vector2(globaldata.archerStations[0].transform.position.x, transform.position.y);
                isfacingeft = false;
            }
            else
            {
                newplace = new Vector2(globaldata.archerStations[1].transform.position.x, transform.position.y);
                isfacingeft = true;
            }
            
            transform.position = Vector2.MoveTowards(transform.position, newplace, speed);

            if (Vector2.Distance(transform.position, newplace) < 0.1f && !attackableInRange)
            {
                IsWalking = false;
                IsIdleanim = true;
            }

            if (globaldata.minutes == 7.00)
            {
                NotDoingJob = true;
            }

        }

        if (MustAttack)
        {
            hasplacetomove = false;
            Isattacking = true;
            IsIdle = false;
            IsWalking = false;
            IsIdleanim = false;
            waiting = false;
        }
        
    }

    private void FixedUpdate()
    {
        MustAttack = Physics2D.OverlapCircle(groundSummonCheck.position, 20f, Tribelayer);

        Debug.Log(MustAttack);
    }

    public void shoot()
    {
        hasplacetomove = false;
        GameObject instantiatedPylon = Instantiate(pylon, transform.position, transform.rotation);
        Projectile projectile = instantiatedPylon.GetComponent<Projectile>();
        projectile.GetArcher(this.gameObject);
        projectile.GetTarget(target);
        Isattacking = false;
        NotDoingJob = true;
    }

    public void GetTarget(GameObject targetget)
    {
        target = targetget;
        shoot();
    }

    public void destroy()
    {
        Destroy(gameObject);
    }

    public void GetTreeTransform(Transform targetTree)
    {
        placeToMove = targetTree;
        TargetToMove = placeToMove.transform.gameObject;
        Debug.Log(TargetToMove);
        MoveToConstruct = true;
    }

    void joblessAI()
    {

        if (jobless && MoveToConstruct)
        {
            hasplacetomove = false;
            NotDoingJob = false;
            IsWalking = true;
            IsIdleanim = false;
            newplace = new Vector2(placeToMove.transform.position.x, transform.position.y);
            if (placeToMove.position.x >= transform.position.x) { isfacingeft = false; }
            if (placeToMove.position.x < transform.position.x) { isfacingeft = true; }

            transform.position = Vector2.MoveTowards(transform.position, newplace, speed);

            Buy Benchscript = placeToMove.gameObject.GetComponent<Buy>();

            if (Vector2.Distance(transform.position, newplace) < 0.1f)
            {
                if (Benchscript.ToolAmount >= 1)
                {
                    Benchscript.Tools[Benchscript.ToolAmount].SetActive(false);
                    Benchscript.ToolAmount -= 1;
                    if (Benchscript.FarmingBench)
                    {
                        Instantiate(farmerPrefab, transform.position, transform.rotation);
                        Destroy(gameObject);
                    }
                    else if (Benchscript.HammerBench)
                    {
                        Instantiate(builderPrefab, transform.position, transform.rotation);
                        Destroy(gameObject);
                    }
                    else if (Benchscript.SpearBench)
                    {
                        Instantiate(archerPrefab, transform.position, transform.rotation);
                        Destroy(gameObject);
                    }

                    NotDoingJob = true;
                    MoveToConstruct = false;
                }
                if (Benchscript.ToolAmount == 0)
                {
                    MoveToConstruct = false;
                }
                IsWalking = false;
                IsIdleanim = true;
                Debug.Log("arrived");


            }
        }
    }

    void timer()
    {
        searchCountdown -= Time.deltaTime;

        if (searchCountdown <= 0f)
        {
            if ((jobless || (Farmer && !HasFarm)) && !MoveToConstruct)
            {
                Debug.Log("Farmer or jobless searching");
                searchCountdown = 3f;
                GameObject[] searchobj = GameObject.FindGameObjectsWithTag(TagString);
                float ShortestDistance = Mathf.Infinity;
                GameObject nearestObj = null;
                foreach (GameObject ToolBench in searchobj)
                {
                    
                    Buy Benchscript = ToolBench.GetComponent<Buy>();
                    FarmScript farmscript = ToolBench.GetComponent<FarmScript>();

                    if ((jobless && Benchscript.ToolAmount >= 1) || (Farmer && farmscript.farmer == null))
                    {
                        hasplacetomove = false;
                        IsWalking = true;
                        IsIdleanim = false;
                        Debug.Log("He/he");
                        float DistanceToEnemy = Vector3.Distance(transform.position, ToolBench.transform.position);
                        if (DistanceToEnemy < ShortestDistance)
                        {
                            Debug.Log("Farmer or jobless found");
                            ShortestDistance = DistanceToEnemy;
                            nearestObj = ToolBench;
                            placeToMove = nearestObj.transform;
                            MoveToConstruct = true;
                            IsWalking = false;
                            IsIdleanim = true;
                        }
                    }
                }
            }
        }
    

        if (waiting)
        {
            waitcountdown -= Time.deltaTime;
            IsIdleanim = true;
            IsWalking = false;
            if ((waitcountdown <= 0f) || !NotDoingJob)
            {
                Debug.Log("Notwaiting");
                if (Farmer && !Isattacking && HasFarm)
                {
                    waitcountdown = Random.Range(4f, 13f);
                    Isattacking = true;
                    IsIdle = false;
                    IsIdleanim = false;
                }
                else
                {
                    waiting = false;
                    IsIdle = false;
                    hasplacetomove = false;
                    IsIdleanim = false;
                    Isattacking = false;
                }

            }
        }

        if (Farmer && HasFarm)
        {
            harvestcountdown -= Time.deltaTime;

            if (harvestcountdown <= 0f)
            {
                
                harvestcountdown = 100f;
                foodInInventory += 7;
                StartCoroutine("SpawnInventory");
            }

        }
    }

    void BuilderMove()
    {
        if (Builder && MoveToConstruct)
        {
            hasplacetomove = false;
            NotDoingJob = false;
            IsWalking = true;
            IsIdleanim = false;
            newplace = new Vector2(TargetToMove.transform.position.x, transform.position.y);
            if (newplace.x >= transform.position.x) { isfacingeft = false; }
            if (newplace.x < transform.position.x) { isfacingeft = true; }

            if (Vector2.Distance(transform.position, newplace) < 1f)
            {
                StartCoroutine("wait");
                Isattacking = true;
                IsWalking = false;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, newplace, speed);
            }
        }
    }

    void AIroam()
    {
        if (NotDoingJob && !knockedback && !inAttackCoroutine)
        {
            if (!waiting)
            {
                if (!hasplacetomove)
                {
                    if (!HasFarm) { PointToMove = Random.Range(transform.position.x - 20, transform.position.x + 20); }
                    if (Farmer && HasFarm) { PointToMove = Random.Range(mapfarm.transform.position.x - 5, mapfarm.transform.position.x + 5); }
                    if (forest) { PointToMove = Random.Range(campCenterpoint.transform.position.x - 12, campCenterpoint.transform.position.x + 12); }

                    if (globaldata.IsNight && !forest)
                    {
                        if (PointToMove <= globaldata.villagecoords[0] + 3) { PointToMove = globaldata.villagecoords[0] + 3; } // outside the left village wall
                        if (PointToMove >= globaldata.villagecoords[1] - 3) { PointToMove = globaldata.villagecoords[1] - 3; } //outside the right village wall
                    }

                    hasplacetomove = true;
                   
                }

                IsWalking = true;
                IsIdleanim = false;
                newplace = new Vector2(PointToMove, transform.position.y);
                if (newplace.x >= transform.position.x) { isfacingeft = false; }
                if (newplace.x < transform.position.x) { isfacingeft = true; }
                transform.position = Vector2.MoveTowards(transform.position, newplace, speed);

                if (Vector2.Distance(transform.position, newplace) < 0.1f)
                {
                    waiting = true;
                    IsIdle = true;
                    waitcountdown = Random.Range(4f, 13f);
                    IsIdleanim = true;
                    Debug.Log(waiting);
                    IsWalking = false;
                    IsIdleanim = true;
                }

               
            }
            
        }
    }

    void FarmerAI()
    {
        if (Farmer && MoveToConstruct)
        {
            NotDoingJob = false;
            IsWalking = true;
            IsIdleanim = false;

            newplace = new Vector2(placeToMove.transform.position.x, transform.position.y);
            if (newplace.x >= transform.position.x) { isfacingeft = false; }
            if (newplace.x < transform.position.x) { isfacingeft = true; }
            transform.position = Vector2.MoveTowards(transform.position, newplace, speed);

            if (Vector2.Distance(transform.position, newplace) < 0.1f)
            {
                FarmScript farm = placeToMove.GetComponent<FarmScript>();
                if (farm.farmer == null)
                {
                    HasFarm = true;
                    farm.farmer = this.gameObject;
                    mapfarm = placeToMove.gameObject;
                }

                NotDoingJob = true;
                MoveToConstruct = false;
                IsWalking = false;
                IsIdleanim = true;
            }
        }

    }

    void animations()
    {
        anim.SetBool("IsAttacking", Isattacking || MustAttack);
        anim.SetBool("IsIdle", IsIdleanim);
        anim.SetBool("IsWalking", IsWalking);
        anim.SetBool("IsDead", IsDead);
    }

    IEnumerator SpawnInventory()
    {
        if (!incoroutine && (!MoveToConstruct && Builder))
        {
            incoroutine = true;
            if (foodInInventory > 30) { foodamountspawned = foodInInventory - 30; }
            else { foodamountspawned = foodInInventory; }

            for (int i = 0; i < foodamountspawned; i++)
            {
                yield return new WaitForSeconds(0.25f);
                GameObject food = (GameObject)Instantiate(foodprefab, transform.position, transform.rotation);
                spawnedfoodList.Add(food);
                Debug.Log("Spawningfood");

            }

            yield return new WaitForSeconds(5);

            foreach (var x in spawnedfoodList)
            {
                Debug.Log(x.ToString());
            }

            GetWheat = true;
        }
        

    }

    void bread()
    {
        if (GetWheat && incoroutine)
        {
            if (berryListNumber >= spawnedfoodList.Count)
            {
                GetWheat = false;
                berryListNumber = 0;
                spawnedfoodList.Clear();
                foodInInventory -= foodamountspawned;
                incoroutine = false;
                Debug.Log("Clearing Berry list");
            }
            else if (berryListNumber < spawnedfoodList.Count)
            {
                Debug.Log(berryListNumber);
                if (spawnedfoodList[berryListNumber].gameObject == null)
                {
                    berryListNumber += 1;
                }
                else if (spawnedfoodList[berryListNumber].gameObject != null)
                {
                    Debug.Log("Not arrived");
                    newplace = new Vector2(spawnedfoodList[berryListNumber].transform.position.x, transform.position.y);
                    if (newplace.x >= transform.position.x) { isfacingeft = false; }
                    if (newplace.x < transform.position.x) { isfacingeft = true; }
                    transform.position = Vector2.MoveTowards(transform.position, newplace, speed);

                    if (Vector2.Distance(transform.position, newplace) < 0.1f)
                    {
                        Debug.Log("Arrived");
                        foodInInventory += 1;
                        Destroy(spawnedfoodList[berryListNumber]);
                        berryListNumber += 1;
                    }
                }
                
            }
        }
    }

   IEnumerator wait()
   {
        if (!incoroutine)
        {
            incoroutine = true;
            Buy buyscript = TargetToMove.gameObject.GetComponent<Buy>();
            buyscript.UnderConstruction = true;
            Isattacking = true;
            IsIdleanim = false;
            IsWalking = false;

            yield return new WaitForSeconds(buyscript.timeToConstruct);

            if (buyscript.wall)
            {
                buyscript.upgradewall();
            }
            if (buyscript.Tree)
            {
                Destroy(TargetToMove.gameObject);
                GameObject food = (GameObject)Instantiate(foodprefab, TargetToMove.transform.position, transform.rotation);
                globaldata.runforestcheck = true;
            }
            if (buyscript.camp)
            {
                buyscript.upgradeCamp();
            }

            Debug.Log("arrived");
            MoveToConstruct = false;
            Isattacking = false;
            NotDoingJob = true;
            IsWalking = false;
            IsIdleanim = true;
            incoroutine = false;
        }
        
   }

    public void FallBack()
    {
        if (globaldata.FallBack && (!Farmer && HasFarm) && !MoveToConstruct && !inAttackCoroutine)
        {
            if (transform.position.x < 0 && transform.position.x < globaldata.villagecoords[0])
            {
                hasplacetomove = false;
                newplace = new Vector2(globaldata.villagecoords[0] + 5, transform.position.y);
                transform.position = Vector2.MoveTowards(transform.position, newplace, speed);
                isfacingeft = false;

                if (Vector2.Distance(transform.position, newplace) < 0.1f)
                {
                    if (Spearman)
                    {
                        spearmanAI();
                        isfacingeft = true;
                    }
                }

            }
            if (transform.position.x > 0 && transform.position.x > globaldata.villagecoords[1])
            {
                hasplacetomove = false;
                newplace = new Vector2(globaldata.villagecoords[1] - 5, transform.position.y);
                transform.position = Vector2.MoveTowards(transform.position, newplace, speed);
                isfacingeft = true;

                if (Vector2.Distance(transform.position, newplace) < 0.1f)
                {
                    if (Spearman)
                    {
                        spearmanAI();
                        isfacingeft = false;
                    }
                }
            }
        }
    }

    void spearmanAI()
    {
        if (globaldata.IsNight)
        {
            NotDoingJob = false;
            IsIdleanim = true;
            IsWalking = false;
        }
    }

    public void TakeDamage(int damageTaken)
    {
        health -= damageTaken;
        StartCoroutine("Attacked");
    }

    public void getAttacker(GameObject enemy)
    {
        if (enemy.transform.position.x > transform.position.x) { rb.AddForce(new Vector2(-200, 0)); }
        if (enemy.transform.position.x < transform.position.x) { rb.AddForce(new Vector2(200, 0)); }
        knockedback = true;
    }

    IEnumerator Attacked()
    {
        Debug.Log("yarrr");
        rend.material.color = new Color32(255, 126, 126, 255);
        yield return new WaitForSeconds(0.5f);
        rend.material.color = new Color32(255, 255, 255, 255);

    }

    void forestTribeMove()
    {
        if (forestMove)
        {
            NotDoingJob = false;

            if (berrytomove != null)
            {
                newplace = new Vector2(berrytomove.transform.position.x, transform.position.y);
                if (berrytomove.transform.position.x >= transform.position.x) { isfacingeft = false; }
                if (berrytomove.transform.position.x < transform.position.x) { isfacingeft = true; }

                transform.position = Vector2.MoveTowards(transform.position, newplace, speed);

                if (Vector2.Distance(transform.position, newplace) < 0.1f || berrytomove == null)
                {
                    if (berrytomove != null)
                    {
                        Destroy(berrytomove);
                        Instantiate(joblessPrefab, transform.position, transform.rotation);
                        Destroy(gameObject);
                    }

                    forestMove = false;
                    NotDoingJob = true;
                }
            }
            else
            {
                forestMove = false;
                NotDoingJob = true;
            }
        }
    }

    public void Attack()
    {
        Debug.Log("IHATEARCHERSATTAHGFUHGFDUIGDFGDDF");
        if (!inAttackCoroutine)
        {
            inAttackCoroutine = true;
            Debug.Log("IHATEARCHERSstart");
            GameObject[] unitysucks = AttackableEnemies.ToArray();

            for (int i = 0; i < unitysucks.Length; i++) // um like cycles through the gameobject list the enemy can attack and if they are close enough he attacks 
            {
                Debug.Log(unitysucks[i]);
                if (unitysucks[i] == null)
                {

                }
                else if ((Vector2.Distance(transform.position, unitysucks[i].transform.position) <= 50))// the 5f is its reach
                {
                    Debug.Log("IHATEARCHERS");
                    GetTarget(unitysucks[i]);
                    shoot();
                    Isattacking = false;
                    NotDoingJob = true;
                    inAttackCoroutine = false;
                    return;

                }
            }

            inAttackCoroutine = false;
        }
        
    }

    public void ArcherEnemiesUpdate()
    {
        AttackableEnemies.Clear();

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in enemies)
        {
            AttackableEnemies.Add(enemy);
        }

        GameObject[] animals = GameObject.FindGameObjectsWithTag("Animal");

        foreach (var animal in animals)
        {
            AttackableEnemies.Add(animal);
        }
    }

    public void FinishAttack()
    {
        Isattacking = false;
        waiting = true;
        waitcountdown = Random.Range(4f, 6f);
        NotDoingJob = true;
        hasplacetomove = false;
    }
}
