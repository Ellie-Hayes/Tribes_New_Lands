using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject archer;
    public GameObject target;

    public float speed;
    public int damage;

    private float archerX;
    private float targetX;
    private float archerY;
    private float targetY;

    private float distance;
    private float nextX;
    private float baseY;
    private float height;

    public bool OnGround;
    private float enemyWidth;
    private float enemyHeight;
    //private Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        //anim = GetComponent<Animator>();
        //target = GameObject.FindGameObjectWithTag("Enemy");

        archerX = archer.transform.position.x;
        targetX = target.transform.position.x;
        archerY = archer.transform.position.y;
        targetY = target.transform.position.y;

        enemyWidth = target.GetComponent<BoxCollider2D>().size.x / 2;
        enemyHeight = target.GetComponent<BoxCollider2D>().size.y / 2;

        Invoke("DestroySpear", 10.0f);
    }

   
    void Update()
    {
        archerX = archer.transform.position.x;
        targetX = target.transform.position.x;
        distance = targetX - archerX;
        nextX = Mathf.MoveTowards(transform.position.x, targetX, speed * Time.deltaTime);
        baseY = Mathf.Lerp(archerY, targetY, (nextX - archerX) / distance);
        height = 2 * (nextX - archerX) * (nextX - targetX) / (-0.25f * distance * distance);

        Vector3 movePosition = new Vector3(nextX, baseY + height, transform.position.z);
        transform.rotation = LookAtTarget(movePosition - transform.position);
        transform.position = movePosition;

        if (target == null)
        {
            Destroy(gameObject);
        }

        if ((Vector2.Distance(new Vector2(transform.position.x, 0), new Vector2(target.transform.position.x, 0)) < enemyWidth) && !OnGround && target != null)
        {
            if ((Vector2.Distance(new Vector2(0, transform.position.y), new Vector2(0, target.transform.position.y)) < enemyHeight) && !OnGround)
            {
                if (target.tag != "Animal")
                {
                    EnemyScript enemy = target.GetComponentInParent<EnemyScript>();
                    enemy.TakeDamage(damage);
                    Destroy(gameObject);
                }
                else
                {
                    AnimalRoam animalscript = target.GetComponentInParent<AnimalRoam>();
                    animalscript.TakeDamage(damage);
                    Destroy(gameObject);
                }
                
            }
        }

        
        //  if ((Vector2.Distance(new Vector2(transform.position.x, enemyHeight / 2), new Vector2(target.transform.position.x, enemyHeight / 2)) < enemyWidth) && !OnGround)
    }

    public static Quaternion LookAtTarget(Vector2 rotation)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) { OnGround = true; }
    }

    public void DestroySpear()
    {
        Destroy(gameObject);
    }

    void discard()
    {
        if ((Vector2.Distance(transform.position, target.transform.position) < enemyWidth) && !OnGround)
        {
            Debug.Log(transform.position.y);
            Destroy(target.gameObject);
            Destroy(gameObject);
        }
    }

    public void GetArcher(GameObject archerget)
    {
        archer = archerget;
    }

    public void GetTarget(GameObject targetGet)
    {
        target = targetGet;
    }

}
