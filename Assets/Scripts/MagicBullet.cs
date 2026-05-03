using UnityEngine;

public class MagicBullet : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float distanceTraveled;
    public float maxDistance = 8f;
    public LayerMask collisionLayer;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(Vector2 dir, float spd)
    {
        direction = dir.normalized;
        speed = spd;
        rb.velocity = direction * speed;
    }

    void FixedUpdate()
    {
        distanceTraveled += speed * Time.fixedDeltaTime;
        if (distanceTraveled >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // var enemy = collision.gameObject.GetComponent<EnemyVisionChaser>();
            // if (enemy != null)
            // {
            //     enemy.DieAndDrop();
            // }
            Destroy(gameObject);
            return;
        }
        bool isInCollisionLayer = (collisionLayer.value & (1 << collision.gameObject.layer)) != 0;
        if (isInCollisionLayer)
        {
            Destroy(gameObject);
        }
    }
}