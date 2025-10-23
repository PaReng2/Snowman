using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;

    private PlayerController playerController;
    private float lifeTime;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        lifeTime = 1f;
    }
    private void Update()
    {
        Destroy(gameObject, lifeTime);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "Ground")
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "enemy")
        {
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject, lifeTime);
        }

    }
    
}