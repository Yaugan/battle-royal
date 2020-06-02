using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{   
     [Header("Common")]
     public GameObject impactEffect;

     [Header("Player")]
     public int damage = 10;
     public float speedPlayer = 20f;
     public Rigidbody rb;
     public float speed;

     public float lifeTime;
     
     [Header("Checker")]
     public bool isPlayer = true;

     
     [Header("Enemy")]
     public float speedEnemy;
     private Transform player;
     private Vector3 target;
     
    //Start is called before the first frame update
    void Start()
    {
        if (isPlayer)
        {
            rb.velocity = transform.forward * speedPlayer;
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        
            target = new Vector3(player.position.x, 0,player.position.z);
        }
    }

    void OnTriggerEnter(Collider hitInfo)
    {
        if (isPlayer)
        {
            Enemy enemy = hitInfo.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            GameObject effect = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);

            Destroy(gameObject,5f);
            Destroy(effect, 1f);
        }
        else
        {
            if (hitInfo.CompareTag("Player"))
            {
                Player player = hitInfo.GetComponent<Player>();
                if (player != null)
                {
                    player.TakeDamage(damage);
                }

                GameObject effect = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);

                DestroyProjectile();
                Destroy(effect, 1f);
            }
        }
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speedEnemy * Time.deltaTime);

        if (transform.position.x == target.x && transform.position.z == target.z)
        {
            DestroyProjectile();
        }
    }

    void DestroyProjectile()
    {
        Destroy(gameObject);
    }


    //public float speed;

    //public float lifeTime;

    //public int damageToGive;

    //// Update is called once per frame
    //void Update()
    //{
    //    transform.Translate(Vector3.forward * speed * Time.deltaTime);

    //    lifeTime -= Time.deltaTime;
    //    if (lifeTime <= 0)
    //    {
    //        Destroy(gameObject);
    //    }
    //}

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Enemy")
    //    {
    //        collision.gameObject.GetComponent<Enemy>().TakeDamage(damageToGive);
    //        Destroy(gameObject);
    //    }


    //}
}
