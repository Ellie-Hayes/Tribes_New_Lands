using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class InteractUI : MonoBehaviour
{
    [SerializeField] public float indicatorTimer = 1.0f;
    [SerializeField] public float maxIndicatorTimer = 1.0f;
    [SerializeField] private Image radialIndicatorUI = null;
    [SerializeField] private KeyCode selectKey = KeyCode.E;
    [SerializeField] private UnityEvent buyEvent = null;
    public bool Active;
    private bool shouldUpdate = false;
    [SerializeField] public GameObject treeObject;
    private Buy buyscript;
    [SerializeField] public GlobalData globalData;
    PlayerController playercont;

    private void Start()
    {
        buyscript = treeObject.GetComponent<Buy>();
        
    }

    private void Awake()
    {
        playercont = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        globalData = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GlobalData>();
    }

    void Update()
    {
        if (Active && globalData.foodAmount > 0)
        {
           
            if (Input.GetKey(selectKey))
            {
                
                shouldUpdate = false;
                indicatorTimer -= Time.deltaTime;
                radialIndicatorUI.enabled = true;
                radialIndicatorUI.fillAmount = indicatorTimer / maxIndicatorTimer;
               

                if (indicatorTimer <= 0)
                {
                    indicatorTimer = maxIndicatorTimer;
                    radialIndicatorUI.fillAmount = maxIndicatorTimer;
                    radialIndicatorUI.enabled = false;
                    buyEvent.Invoke();
                    playercont.InTransaction = false;
                }

                
            }
            else
            {
                if (shouldUpdate)
                {
                    indicatorTimer += Time.deltaTime;
                    radialIndicatorUI.fillAmount = indicatorTimer;
                    //playercont.InTransaction = false;

                    if (indicatorTimer >= maxIndicatorTimer)
                    {
                        indicatorTimer = maxIndicatorTimer;
                        radialIndicatorUI.fillAmount = maxIndicatorTimer;
                        radialIndicatorUI.enabled = false;
                        shouldUpdate = false;
                    }
                }
            }

            if (Input.GetKey(selectKey))
            {
                shouldUpdate = true;
            }
        }

    }

    public void buy()
    {
        //playercont.InTransaction = false; 
        this.gameObject.SetActive(false);
       
        Active = false;
    }

    public void StartCutTree()
    {
        //playercont.InTransaction = false;
        buyscript.CutTree();
        this.gameObject.SetActive(false);
       
        Active = false;
    }

    public void buyTool()
    {
        //playercont.InTransaction = false;
        buyscript.PurchaseTool();
       
    }
    
}
