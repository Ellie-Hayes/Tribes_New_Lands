using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    GlobalData globalData;
    public int tutorialNumber;
    bool CampTutorialCanDisplay;
    bool playedTutorial;

    

    private void Start()
    {
        globalData = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GlobalData>();
        
    }

    private void Update()
    {
        if (CampTutorialCanDisplay && !globalData.InTutorial && globalData.SettingTutorial && !playedTutorial)
        {
            globalData.DisplayTutorial(tutorialNumber);
            playedTutorial = true;                       
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!playedTutorial && globalData.SettingTutorial)
            {
                Debug.Log("tutorial");
                if (tutorialNumber == 0)
                {
                    for (int i = 0; i < globalData.playedTutorials.Length; i++) { if (i != 0) { if (globalData.playedTutorials[i] != true) { return; } } }
                    CampTutorialCanDisplay = true;
                }
                else
                {
                    if (!globalData.InTutorial)
                    {
                        globalData.DisplayTutorial(tutorialNumber);
                        playedTutorial = true;
                    }
                }
            }
           
        }
    }

    
}
