using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyManager enemyManager;
    public float enemyHealth = 2f;
    public GameObject gunHitEffect;
    public AudioClips sfx;


    // Update is called once per frame
    void Update()
    {
        if (enemyHealth <= 0)
        {
            sfx.PlayOneShot("EnemyDeath");
            enemyManager.RemoveEnemy(this);
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        sfx.PlayOneShot("EnemyDamage");
        Instantiate(gunHitEffect, transform.position, Quaternion.identity);
        enemyHealth -= damage;
    }
}