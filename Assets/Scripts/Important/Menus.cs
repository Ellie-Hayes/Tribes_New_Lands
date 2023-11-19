using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour
{
    public int SceneToGoTo;
    public bool Fadeout;
    private Animator anim;
    public bool pauseMenu;
    public bool paused;
    GlobalData globalData;
    public GameObject settingsPanel;
    public GameObject fadeOverlay;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        globalData = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GlobalData>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!pauseMenu)
        {
            anim.SetBool("startpress", Fadeout);
        }
        else
        {
            anim.SetBool("OpenMenu", paused);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                paused = false;
                globalData.paused = false;
                Time.timeScale = 1;
            }
            else
            {
                paused = true;
                globalData.paused = true;
                Time.timeScale = 0;
                settingsPanel.SetActive(false);
            }
            
        }
    }

    public void Unpause()
    {
        paused = false;
        globalData.paused = false;
        Time.timeScale = 1;
    }

    public void SettingsOpen()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    public void StartFadeOut()
    {
        Fadeout = true;
    }
    public void loadScene()
    {
        SceneManager.LoadScene(SceneToGoTo);
        Time.timeScale = 1;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void creditFadeOut()
    {
        Menus menu = fadeOverlay.GetComponent<Menus>();
        menu.Fadeout = true;
    }

    public void Credits()
    {
        SceneManager.LoadScene(3);
        Time.timeScale = 1;
    }
}
