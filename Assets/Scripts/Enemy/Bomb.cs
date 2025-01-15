using UnityEngine;

public class Bomb : MonoBehaviour
{
    private float explosionRadius;
    private int damage;
    private Vector3 targetPosition;

    public float speed = 10f; // Speed of the bomb

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

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
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

        // Destroy the bomb
        Destroy(gameObject);
    }
}
