using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaySystem : MonoBehaviour
{
    private GlobalData globaldata;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        globaldata = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GlobalData>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("IsNight", globaldata.IsNight);
    }
}
