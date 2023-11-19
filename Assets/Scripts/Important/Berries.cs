using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Berries : MonoBehaviour
{
    public bool HasBerries = true;
    public GameObject BerrySprite;
    public GameObject InteractButton;
    public bool PlayerWithinRange;
    public Vector3 offset;
    public GameObject berriesOnBushSprite;
    public float BerrySpawnChance;
    GlobalData globaldata;

    private void Start()
    {
        globaldata = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GlobalData>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && PlayerWithinRange && HasBerries)
        {
            Debug.Log("hi");
            Instantiate(BerrySprite, transform.position + offset, transform.rotation);
            HasBerries = false;
            berriesOnBushSprite.SetActive(false);
        }

        if (!HasBerries) { InteractButton.SetActive(false); }
        if ((transform.position.x < globaldata.villagecoords[1]) && (transform.position.x > globaldata.villagecoords[0])) { Destroy(gameObject); }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && HasBerries) { PlayerWithinRange = true; }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) { PlayerWithinRange = false; }
    }

    public void randomSpawn()
    {
        StartCoroutine("SpawnBerries");
    }

    IEnumerator SpawnBerries()
    {
        float randomNum = Random.Range(0f, 100f);

        if (randomNum <= BerrySpawnChance)
        {
            //Instantiate(BerrySprite, transform.position + offset, transform.rotation);
            berriesOnBushSprite.SetActive(true);
            HasBerries = true;
        }
        
        yield break;
    }
}
