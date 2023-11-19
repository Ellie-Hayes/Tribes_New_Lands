using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forest : MonoBehaviour
{
    public GameObject player;
    public bool displayForest;

    GlobalData globalData;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        globalData = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GlobalData>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("DisplayForest", displayForest);

        if (((player.transform.position.x >= globalData.forestcoords[0]) && (player.transform.position.x <= globalData.forestcoords[1])) || ((player.transform.position.x >= globalData.forestcoords[3]) && (player.transform.position.x <= globalData.forestcoords[2])))
        {
            Debug.Log("displayforest");
            displayForest = true;
        }
        else
        {
            displayForest = false;
        }

        //displayForest = true;
        
       
    }
}
