using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        HealthBehavior healthBehavior = collision.gameObject.GetComponent<HealthBehavior>();
        if (healthBehavior != null)
        {
            healthBehavior.TakeDamage(20);
        }
        Destroy(gameObject, 1);
    }


    private void Start()
    {

        Destroy(gameObject, 5);
    }
}
