using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SparkBolt : MonoBehaviour
{
    public float range = 20f;
    public float verticalRange = 20f;
    public float fireRate = 1f;
    public float bigDamage = 2;
    public float smallDamage = 1;

    public WandAnimator wand;
    public AudioClips sfx;
    public LayerMask raycastLayerMask;

    private float nextTimeToFire;
    private BoxCollider gunTrigger;

    public EnemyManager enemyManager;

    void Start()
    {
        gunTrigger = GetComponent<BoxCollider>();
        gunTrigger.size = new Vector3(1, verticalRange, range);
        gunTrigger.center = new Vector3(0, 0, range * 0.5f);

        // Ensure enemyManager is assigned
        if (enemyManager == null)
        {
            Debug.LogWarning("EnemyManager is not assigned to SparkBolt.");
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time > nextTimeToFire)
        {
            sfx?.PlayOneShot("Spark"); // Null-safe audio playback
            wand?.Bolt(); // Null-safe animation trigger
            Fire();
        }
    }

    void Fire()
    {
        if (enemyManager == null || enemyManager.enemiesInTrigger == null)
        {
           
            return;
        }

        foreach (var enemy in enemyManager.enemiesInTrigger)
        {
            if (enemy == null)
            {
                
                continue;
            }

            var dir = enemy.transform.position - transform.position;

            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, range * 1.5f, raycastLayerMask))
            {
                if (hit.transform == enemy.transform)
                {
                    float dist = Vector3.Distance(enemy.transform.position, transform.position);

                    if (dist > range * 0.5f)
                    {
                        enemy.TakeDamage(smallDamage);
                    }
                    else
                    {
                        enemy.TakeDamage(bigDamage);
                    }
                }
            }
        }

        nextTimeToFire = Time.time + fireRate;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enemyManager == null)
        {
            
            return;
        }

        Enemy enemy = other.transform.GetComponent<Enemy>();
        PlayerMove player = other.transform.GetComponent<PlayerMove>();

        if (enemy != null)
        {
            enemyManager.AddEnemy(enemy);
        }
        if (player != null)
        {

        }
        else
        {
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (enemyManager == null)
        {
            Debug.LogWarning("EnemyManager is not assigned. Cannot remove enemies.");
            return;
        }

        Enemy enemy = other.transform.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemyManager.RemoveEnemy(enemy);
        }
        else
        {

        }
    }
}
