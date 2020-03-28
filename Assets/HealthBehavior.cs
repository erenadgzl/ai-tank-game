using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBehavior : MonoBehaviour
{
    public Text healthText;
    public Image healthBar;
    float health = 100;

    public void TakeDamage(float amount)
    {
        health -= amount;
        healthText.text = string.Format("%{0}", health);
        healthBar.fillAmount = health / 100f;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
