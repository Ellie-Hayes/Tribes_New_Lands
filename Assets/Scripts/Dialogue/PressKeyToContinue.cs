using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressKeyToContinue : MonoBehaviour
{
    public bool PromptPlayer;
    private Animator anim;
    GlobalData globalData;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        globalData = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GlobalData>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("PromptPlayer", PromptPlayer);
    }

    public void turnOffPromt()
    {
        PromptPlayer = false;
        globalData.InTutorial = false;
    }
}
