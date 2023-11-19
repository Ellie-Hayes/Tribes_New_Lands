using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GlobalData : MonoBehaviour
{
    [Header("Currency")]
    public TextMeshProUGUI foodAmountText;
    public int foodAmount;

    [Header("Day system")]
    public TextMeshProUGUI TimeText;
    private int DayNumber = 1;
    private float time = 0.1f;
    public bool IsNight;
    public bool FallBack;

    [Header("UI")]
    public TextMeshProUGUI DayText;

    [Header("Biome check")]
    public string[] biomeNames;
    public float[] forestcoords;
    public float[] villagecoords;
    public GameObject centerPoint;
    public bool runforestcheck;

    [Header("Animals")]
    public GameObject deer;
    public GameObject rabbit;
    private int currentRabbits;
    private int currentDeer;
    private int MaxDeer;
    private int MaxRabbit;
    private float xvalue;
    private float xvalueright;
    public GameObject[] PathNodes;

    float grasslandsArea;
    float forestArea;
    bool spawnedAnimals;
    bool startGame;

    [Header("Misc")]
    public bool paused;
    public GameObject globalLight;
    public bool leftSideBuiltOn;
    public bool rightSideBuiltOn;

    public bool SettingTutorial;

    [Header("Camp")]
    public int maxwallupgrade = 1;

    [Header("Tutorials")]
    public bool InTutorial;
    public string[] tutorialTextStrings;
    public bool[] playedTutorials;
    public TextMeshProUGUI tutorialTextGO;

    [Header("Music")]
    public AudioClip[] audios;
    public AudioSource audioSource;
    public AudioSource NightAudio;
    int audioToPlay;
    bool musicStarted;

    [Header("Archer stations")]
    public GameObject[] archerStations;

    public int minutes;
    // ||

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        UpdateForestCoords();
        globalLight.SetActive(false);
        time = 360;
        //time = 1000;
        SettingTutorial = true;
    }

    void Update()
    {
        time += Time.deltaTime * 5;
        DisplayTime();
        DayText.text = "Day: " + DayNumber.ToString();
        foodAmountText.text = "x" + foodAmount.ToString();

        if (runforestcheck)
        {
            UpdateForestCoords();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            UpdateVillageCoords();
        }
    }

    void DisplayTime()
    {
        minutes = Mathf.FloorToInt(time / 60.0f);
        int seconds = Mathf.FloorToInt(time - minutes * 60);

        if (seconds % 10 == 0) { TimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds); }
        if (minutes % 4 == 0) { spawnAnimals(); }
        else { spawnedAnimals = false; }
        
        if (minutes == 17.00) { FallBack = true; }
        if (minutes == 19.00) { IsNight = true; }
        if (minutes == 7.00)
        {
            IsNight = false;
            FallBack = false;
        }
        if (minutes == 24.00)
        {
            time = 0.0f;
            DayNumber += 1;
            DailySpawning();
            
        }
        if (minutes == 4.00) { startGame = true; }
        if (minutes == 7.00 && DayNumber == 10)
        {
            SceneManager.LoadScene(3);
            Time.timeScale = 1;
        }
        
    }

    void DailySpawning()
    {
        GameObject[] BerryBushes = GameObject.FindGameObjectsWithTag("BerryBush");

        foreach (GameObject obj in BerryBushes)
        {
            obj.GetComponent<Berries>().randomSpawn();
        }

        GameObject[] campsites = GameObject.FindGameObjectsWithTag("ForestCampPoints");

        foreach (GameObject obj in campsites)
        {
            Tribeforestcamps tribescript = obj.GetComponent<Tribeforestcamps>();
            if (tribescript.campHolder != null && !tribescript.GlobalDataNotSpawn)
            {
                int amountoftribespawn = (int)Random.Range(1, 4);
                float chance = 25;

                float randomchance = Random.Range(0, 100);

                if (chance >= randomchance)
                {
                    tribescript.spawnPeople(amountoftribespawn);
                    
                }
            }
        }
    }

    void spawnTest()
    {
        // gets max deer and rabbits from area of forest / grasslands
        xvalue = villagecoords[0] - forestcoords[1];
        xvalueright = forestcoords[3] - villagecoords[1];
        grasslandsArea = xvalue + xvalueright;

        Debug.Log(grasslandsArea);
        MaxRabbit = Mathf.FloorToInt(grasslandsArea / 10);

        if (MaxRabbit == 0) { MaxRabbit = 1; }

        xvalue = forestcoords[1] - forestcoords[0];
        xvalueright = forestcoords[2] - forestcoords[3];
        forestArea = xvalue + xvalueright;

        Debug.Log(forestArea);
        MaxDeer = Mathf.FloorToInt(forestArea / 20);

        if (MaxDeer == 0) { MaxDeer = 1; }
       
    }

    public void UpdateForestCoords()
    {
        
        Debug.Log("ran");
        GameObject[] Trees = GameObject.FindGameObjectsWithTag("Tree");

        float ShortestDistanceLeft = Mathf.Infinity;
        float ShortestDistanceRight = Mathf.Infinity;

        foreach (GameObject tree in Trees)
        {
            float DistanceTotree = Vector3.Distance(tree.transform.position, centerPoint.transform.position);
            if (tree.transform.position.x <= 0)
            {
                if (DistanceTotree < ShortestDistanceLeft)
                {
                    ShortestDistanceLeft = DistanceTotree;
                    forestcoords[1] = tree.transform.position.x;
                    Debug.Log("Updated forest left");
                }
            }
            if (tree.transform.position.x > 0)
            {
                if (DistanceTotree < ShortestDistanceRight)
                {
                    ShortestDistanceRight = DistanceTotree;
                    forestcoords[3] = tree.transform.position.x;
                    Debug.Log("Updated forest right");
                }
            }

        }

        spawnTest();
        runforestcheck = false;
    }

    public void UpdateVillageCoords()
    {
        Debug.Log("VillagerB");
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");

        float ShortestDistanceLeft = Mathf.Infinity;
        float ShortestDistanceRight = Mathf.Infinity;

        foreach (GameObject wall in walls)
        {
            Buy buyscript = wall.GetComponentInChildren<Buy>();
            Debug.Log(buyscript);
            if (buyscript.wallheath[buyscript.currentwalltype] <= 0)
            {
                Debug.Log("returning");
                
            }
            else
            {
                float DistanceToWall = Vector3.Distance(wall.transform.position, centerPoint.transform.position);
                if (wall.transform.position.x < 0)
                {
                    if (DistanceToWall < ShortestDistanceLeft)
                    {
                        ShortestDistanceLeft = DistanceToWall;
                        villagecoords[0] = wall.transform.position.x;
                        archerStations[0].transform.position = new Vector2(wall.transform.position.x - 5, wall.transform.position.y);
                        Debug.Log("Updated wall left");
                    }
                }
                if (wall.transform.position.x > 0)
                {
                    if (DistanceToWall < ShortestDistanceRight)
                    {
                        ShortestDistanceRight = DistanceToWall;
                        villagecoords[1] = wall.transform.position.x;
                        archerStations[1].transform.position = new Vector2(wall.transform.position.x + 5, wall.transform.position.y);
                        Debug.Log("Updated wall right");
                    }
                }
            }
            

        }
    }

    void spawnAnimals()
    {
        if (!spawnedAnimals)
        {
            spawnedAnimals = true;
            Debug.Log("ANIMALS R HERE");
            if (currentRabbits <= MaxRabbit - 1)
            {
                float rabbitSpawnPercentage = (grasslandsArea / MaxRabbit) / 2;
                
                int maxSpawnableRabbits = MaxRabbit - currentRabbits;

                for (int i = 0; i < maxSpawnableRabbits; i++)
                {
                    float randomNum = Random.Range(1, 100);

                    if (rabbitSpawnPercentage > randomNum)
                    {
                        Debug.Log("spawned");
                        int side = (int)Random.Range(1, 3);
                        Debug.Log(side);
                        if (side == 1)
                        {
                            xvalue = Random.Range(villagecoords[0], forestcoords[1]);
                            Vector2 spawnPosition = new Vector2(xvalue, -1f);
                            Debug.Log(xvalue);
                            Instantiate(rabbit, spawnPosition, transform.rotation);
                            currentRabbits += 1;
                        }
                        if (side == 2)
                        {
                            xvalueright = Random.Range(villagecoords[1], forestcoords[3]);
                            Vector2 spawnPosition = new Vector2(xvalueright, -1f);
                            Debug.Log(xvalueright);
                            Instantiate(rabbit, spawnPosition, transform.rotation);
                            currentRabbits += 1;
                        }
                    }
                }

            }

            if (currentDeer <= MaxDeer - 1)
            {
                float deerSpawnPercentage = (forestArea / MaxDeer) / 2;
                Debug.Log(deerSpawnPercentage);
                int maxSpawnableDeer = MaxDeer - currentDeer;

                for (int i = 0; i < maxSpawnableDeer; i++)
                {
                    float randomNum = Random.Range(1, 100);
                    Debug.Log(randomNum);

                    if (deerSpawnPercentage > randomNum)
                    {
                        Debug.Log("spawned");
                        int side = (int)Random.Range(1, 3);
                        Debug.Log(side);
                        if (side == 1)
                        {
                            xvalue = Random.Range(forestcoords[0], forestcoords[1]);
                            Vector2 spawnPosition = new Vector2(xvalue, 0f);
                            Instantiate(deer, spawnPosition, transform.rotation);
                            currentDeer += 1;
                        }
                        if (side == 2)
                        {
                            xvalueright = Random.Range(forestcoords[2], forestcoords[3]);
                            Vector2 spawnPosition = new Vector2(xvalueright, 0f);
                            Instantiate(deer, spawnPosition, transform.rotation);
                            currentDeer += 1;
                        }
                    }
                }

            }
        }
        
    }

    public void DisplayTutorial(int tutorialNumber)
    {
        InTutorial = true;
        tutorialTextGO.text = tutorialTextStrings[tutorialNumber];
        playedTutorials[tutorialNumber] = true;

        PressKeyToContinue press = tutorialTextGO.GetComponent<PressKeyToContinue>();
        press.PromptPlayer = true;
    }
    


    public void ChangeTutorialSettings()
    {
        if (SettingTutorial) { SettingTutorial = false; }
        else if (!SettingTutorial) { SettingTutorial = true; }

        Debug.Log(SettingTutorial);
    }

    
       
    IEnumerator musicWait()
    {
        
        if (musicStarted)
        {
            StartCoroutine(FadeAudioSource.StartFade(audioSource, 2f, 0f));
            yield return new WaitForSeconds(2);
        }
        musicStarted = true;
        StartCoroutine(FadeAudioSource.StartFade(audioSource, 2f, 1f));
        audioSource.clip = audios[audioToPlay];
        audioSource.Play();
        
        StartCoroutine(FadeAudioSource.FadeOut(audioSource, 2f));
        audioSource.Stop();
        yield return new WaitForSeconds(0.5f);
        audioSource.clip = audios[audioToPlay];
        audioSource.Play();
        StartCoroutine(FadeAudioSource.FadeIn(audioSource, 2f));

    }

    void hatemusicWait()
    {
        //StartCoroutine(FadeAudioSource.FadeOut(audioSource, 2f));
        audioSource.clip = audios[audioToPlay];
        audioSource.Play();
        StartCoroutine(FadeAudioSource.FadeIn(audioSource, 2f));
    }
    
}
