using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 3f;

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (EnemySpawner.activeEnemies.Contains(this))
            EnemySpawner.activeEnemies.Remove(this);

        FindObjectOfType<EnemySpawner>()?.NotifyEnemyDefeated();
        Destroy(gameObject);
    }


}
