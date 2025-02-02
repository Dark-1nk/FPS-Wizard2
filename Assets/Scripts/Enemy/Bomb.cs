using UnityEngine;

public class Bomb : MonoBehaviour
{
    private float explosionRadius;
    private int damage;
    private Vector3 targetPosition;
    public AudioClips sfx;
    private Animator animator;

    public float speed = 10f; // Speed of the bomb

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void Initialize(float radius, int damageAmount, Vector3 target)
    {
        explosionRadius = radius;
        damage = damageAmount;
        targetPosition = target;
    }

    private void Update()
    {
        // Move the bomb toward the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Check if the bomb has reached the target
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            Explode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Collider>())
        {
            Explode();
        }
    }

    private void Explode()
    {
        // Detect objects within the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (var collider in colliders)
        {
            PlayerMove player = collider.GetComponent<PlayerMove>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }

        animator.SetTrigger("Explode");
        sfx.PlayOneShot("Bomb");
        // Destroy the bomb
        Destroy(gameObject);
    }
}
