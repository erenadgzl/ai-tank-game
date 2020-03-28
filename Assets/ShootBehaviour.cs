using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBehaviour : MonoBehaviour
{
    public GameObject bulletprefab;
    public Transform bulletSpawn;
    float forceAmount = 1000f;
    float lastShoot;

    public void Shoot(float shootFreq)
    {
        if ((lastShoot += Time.deltaTime) >= 1f / shootFreq)
        {
            InstantiateBullet();
            lastShoot = 0;
        }

    }
    public void Shoot()
    {
        InstantiateBullet();
    }

    private void InstantiateBullet()
    {
        GameObject go = Instantiate(bulletprefab, bulletSpawn.position, Quaternion.identity);
        go.gameObject.GetComponent<Rigidbody>().AddForce(forceAmount * transform.forward);
    }
}
