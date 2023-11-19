using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tribeforestcamps : MonoBehaviour
{
    public GameObject tree1;
    public GameObject tree2;
    public GameObject campHolder;
    public bool GlobalDataNotSpawn;
    public GameObject[] currenttribe;
    public GameObject forestTribePrefab;
    GlobalData globalData;

    private void Start()
    {
        globalData = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GlobalData>();
    }

    void Update()
    {
        if (tree1 == null && tree2 == null)
        {
            //play fade out animation
            DestroyCamps();
        }

        if (globalData.minutes == 23.00)
        {
            for (int i = 0; i < currenttribe.Length; i++) { if (currenttribe[i] != null) { return; } }
            GlobalDataNotSpawn = false;
        }
    }

    public void DestroyCamps() => Destroy(campHolder.gameObject);

    public void spawnPeople(int spawnAmount)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject tribeMember = Instantiate(forestTribePrefab, transform.position, transform.rotation);
            currenttribe[i] = tribeMember;

            TribeAI tribescript = tribeMember.GetComponent<TribeAI>();
            tribescript.campCenterpoint = this.gameObject;
            
        }

        GlobalDataNotSpawn = true;
    }
}
