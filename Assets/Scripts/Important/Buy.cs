using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buy : MonoBehaviour
{
    [Header("Attributes")]
    public Vector3 offset;
    public GameObject[] costDisplays;
    public bool HasMultipleUpgrades;

    private GlobalData globaldata;
    private int costDisplayNumber;
    bool playerInRange;

    [Header("Button sprites")]
    public GameObject Button;
    public Sprite PressedButton;
    public Sprite UnpressedButton;

    [Header("Interacting")]
    public GameObject interactCanvas;
    public InteractUI interact;

    [Header("Upgrading")]
    public SpriteRenderer spriteRenderer;
    public Sprite[] chippedWalls;
    public Sprite[] brokenWalls;
    public Sprite[] newSprite;
    public GameObject decorations;
    public bool wall;

    public int currentwalltype;
    public float timeToConstruct;
    public bool UnderConstruction;
    public bool InsideWalls;

    [Header("Repairing")]
    public Sprite[] rubbleSprites;
    public int[] wallheath;
    public int[] maxwallhealth;
    public GameObject repairWallCostDisplay;

    [Header("Trees")]
    public bool Tree;
    public Transform transform;
    private int buildersFree;

    [Header("Tools")]
    public bool SpearBench;
    public bool HammerBench;
    public bool FarmingBench;
    public int ToolAmount;
    public GameObject[] Tools;

    [Header("Wells and farms")]
    public GameObject wellsprite;
    public GameObject farmStakes;
    public GameObject farmsprite;
    public bool well;

    [Header("Camp")]
    public bool camp;
    public GameObject[] stars;
    int starNumber;

    private void Start()
    {
        transform = this.gameObject.transform;
        if (wall)
        {
            for (int i = 0; i < maxwallhealth.Length; i++)
            {
                wallheath[i] = maxwallhealth[i];
                Debug.Log(wallheath[i]);
            }
        }

        if (camp) { currentwalltype = 1; }
        
        globaldata = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GlobalData>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Button.GetComponent<Image>().sprite = PressedButton;
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            Button.GetComponent<Image>().sprite = UnpressedButton;
        }
        if (Input.GetKeyUp(KeyCode.K) && playerInRange)
        {
            WallTakeDamage(100);
        }
        if (Input.GetKeyUp(KeyCode.C) && playerInRange)
        {
            wallheath[currentwalltype] -= 5;
        }

        CheckWallHealth();

        if (SpearBench || HammerBench || FarmingBench)
        {
            if (ToolAmount == Tools.Length - 1)
            {
                interactCanvas.SetActive(false);
                interact.Active = false;
            }
        }

        if (HasMultipleUpgrades && (costDisplayNumber > costDisplays.Length))
        {
            interactCanvas.SetActive(false);
            interact.Active = false;
        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
            buildersFree = 0;
            

            if (((Tree || wall) || camp) && !UnderConstruction && globaldata.foodAmount > 0)
            {
                GameObject[] builders = GameObject.FindGameObjectsWithTag("Builder");
                foreach (GameObject builder in builders)
                {
                    TribeAI builderscript = builder.GetComponent<TribeAI>();
                    if (!builderscript.MoveToConstruct)
                    {
                        buildersFree += 1;
                    }

                }

                Debug.Log("camp camp");
                
                if (buildersFree > 0)
                {
                    if ((wall && currentwalltype < newSprite.Length - 1) && ((wallheath[currentwalltype] > 0) || (currentwalltype == 0)) && globaldata.maxwallupgrade > currentwalltype)
                    {
                        if (transform.position.x < globaldata.forestcoords[3] && transform.position.x > globaldata.forestcoords[1])
                        {
                            interactCanvas.SetActive(true);
                            interact.Active = true;
                        }

                    }
                    else if (Tree)
                    {
                        interactCanvas.SetActive(true);
                        interact.Active = true;
                    }
                    else if ((wall && wallheath[currentwalltype] <= 0) && (currentwalltype != 0))
                    {
                        interactCanvas.SetActive(true);
                        interact.Active = true;

                        repairWallCostDisplay.SetActive(true);

                        if (costDisplays[costDisplayNumber] != null)
                        {
                            costDisplays[costDisplayNumber].SetActive(false);
                        }
                    }
                    else if (camp && currentwalltype < 4)
                    {
                        interactCanvas.SetActive(true);
                        interact.Active = true;
                    }
                }
            }
            
            if (SpearBench || HammerBench || FarmingBench)
            {
                if (ToolAmount < Tools.Length - 1)
                {
                    interactCanvas.SetActive(true);
                    interact.Active = true;
                }
            }


            if (well)
            {
                if (transform.position.x < globaldata.villagecoords[1] && transform.position.x > globaldata.villagecoords[0])
                {
                    interactCanvas.SetActive(true);
                    interact.Active = true;
                }
            }
            

        }
    }

    // ||
    void MeatMove()
    {
        if (HasMultipleUpgrades && ((wallheath[currentwalltype] > 0) || !wall))
        {
            Debug.Log("meat tsukki ");
            Destroy(costDisplays[costDisplayNumber].gameObject);
            costDisplayNumber += 1;
            if (costDisplayNumber < costDisplays.Length)
            {
                costDisplays[costDisplayNumber].SetActive(true);
            }
            
        }
        else if (wall && wallheath[currentwalltype] <= 0)
        {
            wallheath[currentwalltype] = maxwallhealth[currentwalltype];
            gameObject.layer = 10;
            UIMoneyMoveTo uI = repairWallCostDisplay.gameObject.GetComponent<UIMoneyMoveTo>();
            uI.ClearUI();
            changeGlobalDataWallBuild(true);
            repairWallCostDisplay.SetActive(false);

            if (costDisplayNumber < costDisplays.Length)
            {
                costDisplays[costDisplayNumber].SetActive(true);
            }
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
            interactCanvas.SetActive(false);
            interact.Active = false;
        }
    }

    public void upgradewall()
    {
        if (wallheath[currentwalltype] > 0 || currentwalltype == 0)
        {
            if (transform.position.x > 0)
            {
                if (transform.position.x > globaldata.villagecoords[1]) { globaldata.villagecoords[1] = transform.position.x; }
            }
            if (transform.position.x < 0)
            {
                if (transform.position.x < globaldata.villagecoords[0]) { globaldata.villagecoords[0] = transform.position.x; }
            }

            currentwalltype += 1;

        }


        changeGlobalDataWallBuild(true);
        gameObject.layer = 10;
        updateEnemyList();
        spriteRenderer.sprite = newSprite[currentwalltype];
        Debug.Log(currentwalltype);
        UnderConstruction = false;
        MeatMove();

        if (decorations != null)
        {
            decorations.SetActive(true);
        }
    }

    public void upgradeCamp()
    {
        currentwalltype += 1;
        globaldata.maxwallupgrade = currentwalltype;
        stars[starNumber].SetActive(true);
        starNumber += 1;
        MeatMove();
        UnderConstruction = false;
    }

    void changeGlobalDataWallBuild(bool updatedBool)
    {
        if (InsideWalls)
        {
            if (transform.position.x > 0) { globaldata.rightSideBuiltOn = updatedBool; }
            if (transform.position.x < 0) { globaldata.leftSideBuiltOn = updatedBool; }
        }
    }

    public void CutTree()
    {
        Debug.Log("Builder");
        GameObject[] builders = GameObject.FindGameObjectsWithTag("Builder");
        float ShortestDistance = Mathf.Infinity;
        GameObject nearestBuilder = null;
        foreach (GameObject builder in builders)
        {
            TribeAI builderscript = builder.GetComponent<TribeAI>();
            if (!builderscript.MoveToConstruct)
            {
                float DistanceToEnemy = Vector3.Distance(transform.position, builder.transform.position);
                if (DistanceToEnemy < ShortestDistance)
                {
                    ShortestDistance = DistanceToEnemy;
                    nearestBuilder = builder;
                }
            }
            
        }

        Debug.Log(nearestBuilder);
        TribeAI tribe = nearestBuilder.GetComponent<TribeAI>();
        tribe.GetTreeTransform(this.gameObject.transform);
    }

    public void PurchaseTool()
    {
        ToolAmount += 1;
        Tools[ToolAmount].gameObject.SetActive(true);
    }

    public void PurchaseWell()
    {
        farmStakes.SetActive(true);
        Instantiate(wellsprite, transform.position + offset, transform.rotation);
        Destroy(gameObject);
    }

    public void PurchaseFarm()
    {
        Instantiate(farmsprite, transform.position + offset, transform.rotation);
        Destroy(gameObject);
    }

    void CheckWallHealth()
    {
        if (wall)
        {
            if ((wallheath[currentwalltype] < (maxwallhealth[currentwalltype] / 3 * 2)) && currentwalltype != 0)
            {
                if (wallheath[currentwalltype] < (maxwallhealth[currentwalltype] / 3))
                {
                    spriteRenderer.sprite = brokenWalls[currentwalltype];
                }
                else { spriteRenderer.sprite = chippedWalls[currentwalltype]; }
            }

            if (wallheath[currentwalltype] <= 0)
            {
                if ((currentwalltype == 1) || (currentwalltype == 2))
                {
                    spriteRenderer.sprite = rubbleSprites[0];
                }
                if ((currentwalltype == 3) || (currentwalltype == 4))
                {
                    spriteRenderer.sprite = rubbleSprites[1];
                }

                gameObject.layer = 0;
                changeGlobalDataWallBuild(false);
            }
        }
        
    }

    public void WallTakeDamage(int damage)
    {
        wallheath[currentwalltype] -= damage;

        if (wallheath[currentwalltype] <= 0)
        {
            globaldata.UpdateVillageCoords();
        }
    }

    void updateEnemyList()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject obj in enemies)
        {
            EnemyScript enemyscript = obj.GetComponent<EnemyScript>();

            if (enemyscript.Attackables.Contains(gameObject)) { Debug.Log("whydoesittakesolong"); }
            else { enemyscript.Attackables.Add(gameObject); }
            
        }
    }

    
}
