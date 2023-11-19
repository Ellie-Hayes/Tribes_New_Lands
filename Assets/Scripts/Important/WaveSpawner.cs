using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public Vector3 Offset;
    public enum SpawnState { Spawning, Waiting, Counting };
    public GameObject GameOverPanel;

    [System.Serializable] 
    public class Wave 
    {
        public string WaveName;
        public Transform enemy; 
        public int count; 
        public float rate; 
    }

    [Header("Attributes")]
    public waves[] wave; 
    public Transform[] spawnPoints; 
    private int nextWave = 0; 
    public float timeBetweenWaves;  
    public float WaveCountdown;
    private float searchCountdown = 1f;
     
    public GlobalData SceneData; 

    #region 
    [System.Serializable] //Doing code comments in region names the region lol but regions basically allow classes within classes, here we have the wave class and the sequence class within
    public class waves
    {
        [System.Serializable] //allows us to change the values in the inspector
        public class Sequence
        {
            public string SequenceName; //To name your enemy to keep things clear but not necessary 
            public float spawnRate = 1f; //Explanitory 
            public GameObject enemyPrefab; //The enemy being spawned
            [Space]//Dunno 12 year old just started writing these
            public int count; //How many of the enemies being spawned 
        }

        public Sequence[] waveSequences;
    }


    //Always have to end a region with #endregion its like a semi colon for regions 
    #endregion

    private SpawnState state = SpawnState.Counting;

    void Start()
    {
        WaveCountdown = timeBetweenWaves;
        SceneData = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GlobalData>(); 
        Offset = new Vector3(0f, 0f, 200f);
    }

    // Update is called once per frame
    void Update()
    {

        if (state == SpawnState.Waiting)
        {
            if (!EnemyIsAlive()) 
            {
                beginNewRound(); 
                return;
                
            }
            else
            {
                return;  
            }
        }


        if (WaveCountdown <= 0) 
        {
            if (state != SpawnState.Spawning && SceneData.minutes == 19)  
            {
                StartCoroutine(spawnEnemies());
            }
        }
        else
        {
            WaveCountdown -= Time.deltaTime; 
        }
    }

    private IEnumerator spawnEnemies()
    {
        state = SpawnState.Spawning;
        int randomNode = Random.Range(0, 2);
        for (int i = 0; i < wave[nextWave].waveSequences.Length; i++) 
        {
            yield return new WaitForSeconds(WaveCountdown);

            for (int x = 0; x < wave[nextWave].waveSequences[i].count; x++)  
            {
                Instantiate(wave[nextWave].waveSequences[i].enemyPrefab, SceneData.PathNodes[randomNode].transform.position + Offset, transform.rotation); 
                yield return new WaitForSeconds(1 / wave[nextWave].waveSequences[i].spawnRate); 
               
            }
        }

        state = SpawnState.Waiting;
        yield break; 
    }

    bool EnemyIsAlive() 
    {
        searchCountdown -= Time.deltaTime;  

        if (searchCountdown <= 0f)
        {
            searchCountdown = 1f;
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0) 
            {
                return false;
            }
        }

        return true;  
    }

    void beginNewRound()
    {

        state = SpawnState.Counting;
        WaveCountdown = timeBetweenWaves;

        if (nextWave + 1 > wave.Length - 1)  
        {

            Debug.Log("Completed all waves");
            GameOverPanel.gameObject.SetActive(true);
            Time.timeScale = 0;
            return;
        }
        else
        {
            nextWave++; 
        }
    }
}
