using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Attributes")]
    public float startHealth = 100;
    public GameObject deathEffect;
    public Image healthBar;

    private float health;

    void Start()
    {
        health = startHealth;
    }

    public void TakeDamage(int damage)
    {
        //cheking if enemy is taking damage
        health -= damage;

        healthBar.fillAmount = health / startHealth;

        if (health <= 0)
        {
            Die();//killing the enemy
        }

    }

    void Die()
    {
        GameObject effect = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
        Destroy(effect, 2f);
    }
}
