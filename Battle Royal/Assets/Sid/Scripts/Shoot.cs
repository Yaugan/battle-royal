using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shoot : MonoBehaviour
{
    /* public Transform firePoint;
     public GameObject bulletPrefab;

     // Update is called once per frame
     void Update()
     {
         if (Input.GetButtonDown("Fire1"))
         {
             ShootBullet();
         }
     }

     void ShootBullet()
     {
         Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
     }*/


    public bool isFiring;
    public Bullet bullet;
    public float bulletSpeed;
    public float timeBetweenShots;
    public Transform firePoint;

    public Button fireButton;

    private float shotCounter;

    public void FireButton()
    {
        if (fireButton.interactable == true)
        {
            if (isFiring)
            {
                shotCounter -= Time.deltaTime;

                if (shotCounter <= 0)
                {
                    shotCounter = timeBetweenShots;
                    Bullet newBullet = Instantiate(bullet, firePoint.position, firePoint.rotation) as Bullet;
                    newBullet.speed = bulletSpeed;
                }
            }

            else
            {

                isFiring = false;
                shotCounter = 0;

            }

        }
    }
}
