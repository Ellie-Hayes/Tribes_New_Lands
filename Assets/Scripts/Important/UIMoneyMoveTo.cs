using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMoneyMoveTo : MonoBehaviour
{
    public int cost;
    public GameObject[] coinholders;
    public GameObject coincoin;
    public GameObject coincanvasholder;
    public float speed;
    public bool AttachedToWorkbench;
    public bool repairCanvas;

    private bool StartedPurchase;
    private GameObject Player;
    private GameObject Coin;
    public InteractUI interact;

    public int coinNum;
    List<GameObject> CoinList;
    bool bought;
    public float TimeTaken;
    private float TimeBetweenObjects;
    private float Distance;
    GlobalData globalData;
    PlayerController playercont;


    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        playercont = Player.GetComponent<PlayerController>();
        CoinList = new List<GameObject>();

        globalData = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GlobalData>();
    }

    // Update is called once per frame
    void Update()
    {
       
        if (Input.GetKey(KeyCode.E))
        {
            if (!StartedPurchase)
            {
                StartedPurchase = true;
                playercont.InTransaction = true; 
                Coin = Instantiate(coincoin, Player.transform.position, Quaternion.identity);
                Coin.transform.parent = coincanvasholder.gameObject.transform;
                CoinList.Add(Coin);
                globalData.foodAmount -= 1;

                for (int i = 0; i < coinholders.Length; i++)
                {
                    Distance = Vector3.Distance(Player.transform.position, coinholders[i].transform.position);
                    TimeBetweenObjects = Distance / speed;
                    TimeTaken += TimeBetweenObjects;
                    Debug.Log(TimeTaken);
                    interact.indicatorTimer = TimeTaken;
                    interact.maxIndicatorTimer = TimeTaken;

                }
                
            }

            if (coinNum < coinholders.Length)
            {
                Coin.transform.position = Vector2.MoveTowards(Coin.transform.position, coinholders[coinNum].transform.position, speed * Time.deltaTime);

                if (Vector2.Distance(Coin.transform.position, coinholders[coinNum].transform.position) == 0 && globalData.foodAmount >= 1)
                {
                    Coin = Instantiate(coincoin, Player.transform.position, Quaternion.identity);
                    Coin.transform.parent = coincanvasholder.gameObject.transform;
                    CoinList.Add(Coin);
                    coinNum += 1;
                    globalData.foodAmount -= 1;
                }
            }
           
            
        }

        if (Input.GetKeyUp(KeyCode.E) && !bought)
        {
            foreach (GameObject x in CoinList)
            {
                BerrySpawn berrySpawn = x.gameObject.GetComponent<BerrySpawn>();
                berrySpawn.transform.parent = null;
                berrySpawn.DropUI();
            }
            StartedPurchase = false;
            playercont.InTransaction = false;

            CoinList.Clear();
            coinNum = 0;
            TimeTaken = 0;
        }
    }

    public void ClearUI()
    {
        foreach (var x in CoinList)
        {
            Destroy(x.gameObject);
        }

        StartedPurchase = false;
        CoinList.Clear();
        coinNum = 0;
        TimeTaken = 0;
    }
}
