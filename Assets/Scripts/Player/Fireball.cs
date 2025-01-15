using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fireball : MonoBehaviour
{
    public float range = 20f;
    public float verticalRange = 20f;
    public float cooldown = 10f;
    public float damage = 5f;
    public Image fireSpellVisual; // Visual of the spell
    public Image cooldownIndicator; // Cooldown progress indicator (fill or overlay)
    public WandAnimator wand;

    public Color readyColor = Color.white; // Color when ready
    public Color cooldownColor = new(1f, 1f, 1f, 0.5f); // Color when on cooldown

    public AudioClips sfx;
    public LayerMask raycastLayerMask;

    private BoxCollider fireballTrigger;
    public PlayerMove caster;

    public EnemyManager enemyManager;
    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;

    void Start()
    {
        fireballTrigger = GetComponent<BoxCollider>();
        fireballTrigger.size = new Vector3(5, verticalRange, range);
        fireballTrigger.center = new Vector3(0, 0, range * 0.5f);

        // Ensure the cooldown indicator starts as empty (ready state)
        if (cooldownIndicator != null)
        {
            cooldownIndicator.fillAmount = 1f; // Fully filled when ready
        }
    }

    void Update()
    {
        if (caster == null || !caster.hasOrange || fireSpellVisual == null)
        {
            return;
        }

        fireSpellVisual.gameObject.SetActive(caster.hasOrange);

        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;

            // Update the cooldown progress indicator (reversed logic)
            if (cooldownIndicator != null)
            {
                cooldownIndicator.fillAmount = 1f - (cooldownTimer / cooldown);
            }

            if (cooldownTimer <= 0f)
            {
                EndCooldown();
            }
        }

        if (Input.GetMouseButtonDown(1) && !isOnCooldown)
        {
            sfx?.PlayOneShot("Fireball"); // Null-safe audio playback
            wand?.Fire(); // Null-safe animation trigger
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
                    enemy.TakeDamage(damage);
                }
            }
        }

        StartCooldown();
    }

    void StartCooldown()
    {
        isOnCooldown = true;
        cooldownTimer = cooldown;

        // Update UI to indicate cooldown
        if (fireSpellVisual != null)
        {
            fireSpellVisual.color = cooldownColor;
        }
        if (cooldownIndicator != null)
        {
            cooldownIndicator.fillAmount = 0f; // Start empty when cooldown begins
        }
    }

    void EndCooldown()
    {
        isOnCooldown = false;

        // Reset UI to indicate readiness
        if (fireSpellVisual != null)
        {
            fireSpellVisual.color = readyColor;
        }
        if (cooldownIndicator != null)
        {
            cooldownIndicator.fillAmount = 1f; // Fully filled when ready
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enemyManager == null)
        {
            return;
        }

        Enemy enemy = other.transform.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemyManager.AddEnemy(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (enemyManager == null)
        {
            return;
        }

        Enemy enemy = other.transform.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemyManager.RemoveEnemy(enemy);
        }
    }
}
