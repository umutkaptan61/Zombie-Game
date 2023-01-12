using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;

    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void GiveDamage(int amount)
    {
        currentHealth -= amount;
    }

    public void AddHealth(int amount)
    {
        currentHealth += amount;
    }
    
    void Update()
    {
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
