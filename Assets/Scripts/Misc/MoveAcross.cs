using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAcross : MonoBehaviour
{
    private Vector3 startPos;
    private float midPoint;
    public Vector2 direction;
    public float speed;

    void Start()
    {
        startPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        midPoint = GetComponent<BoxCollider2D>().size.x / 2;
    }

    
    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        if (transform.position.x < startPos.x - midPoint * 2)
        {
            transform.position = new Vector3(startPos.x, transform.position.y, transform.position.z);
        }
    }

    
}
