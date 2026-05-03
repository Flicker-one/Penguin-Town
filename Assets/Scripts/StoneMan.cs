using UnityEngine;

public class EnemyVisionChaser : MonoBehaviour
{
    [Header("player & movement")]
    public Transform player;
    public float moveSpeed = 2.2f;

    [Header("visibility")]
    public float viewRadius = 6f;
    [Range(0, 360)] public float viewAngle = 90f;
    public LayerMask collisionLayer;
    public LayerMask playerLayer;
    
    public float chaseAfterLostTime = 2f;

    [Header("attack")]
    public float attackRange = 1.5f;
    public float attackInterval = 1.5f;
    public float damage = 40f;

    [Header("drop items")] 
    public GameObject dropItemPrefab;

    public AudioClip deathSFX;
    public event System.Action onDeath;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private Vector2 lastSawPlayerPos;
    private float lostVisionTimer;
    private float attackTimer;
    private bool canSeePlayer;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        lostVisionTimer = chaseAfterLostTime;
    }

    void Update()
    {
        if (PauseController.IsGamePaused || FrozenSpell.IsFreezed)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("isWalking", false);
            return;
        }
        if (player == null) return;
        attackTimer -= Time.deltaTime;
        CheckVision();
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= attackRange)
        {
            rb.velocity = Vector2.zero;
            if (attackTimer <= 0)
            {
                Attack();
                attackTimer = attackInterval;
            }
            return;
        }

        // 行为逻辑
        if (canSeePlayer)
        {
            MoveTowards(player.position);
            lostVisionTimer = chaseAfterLostTime;
        }
        else if (lostVisionTimer > 0)
        {
            MoveTowards(lastSawPlayerPos);
            lostVisionTimer -= Time.deltaTime;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    void CheckVision()
    {
        canSeePlayer = false;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, viewRadius, playerLayer);

        foreach (var hit in hits)
        {
            Transform target = hit.transform;
            Vector2 dirToTarget = (target.position - transform.position).normalized;
            if (Vector2.Angle(transform.right, dirToTarget) < viewAngle / 2)
            {
                if (!Physics2D.Raycast(transform.position, dirToTarget, viewRadius, collisionLayer))
                {
                    canSeePlayer = true;
                    lastSawPlayerPos = target.position;
                }
            }
        }
    }

    void MoveTowards(Vector2 targetPos)
    {
        Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
        rb.velocity = dir * moveSpeed;
        animator.SetFloat("vx", dir.x);
        animator.SetFloat("vy", dir.y);
    }

    void Attack()
    {
        Debug.Log("Enemy attack");
        PlayerStatus.Instance.ModifyHealth(-damage);
        // reduce player's hp
    }

    // 视野 Gizmos 调试
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector2 viewDirA = DirFromAngle(viewAngle / 2);
        Vector2 viewDirB = DirFromAngle(-viewAngle / 2);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + viewDirA * viewRadius);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + viewDirB * viewRadius);
    }

    public Vector2 DirFromAngle(float angle)
    {
        angle += transform.eulerAngles.z;
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
        {
            DieAndDrop();
            Destroy(collision.gameObject);
        }
    }

    public void DieAndDrop()
    {
        onDeath?.Invoke();
        AudioSource.PlayClipAtPoint(deathSFX, transform.position);
        if (dropItemPrefab != null)
        {
            Instantiate(dropItemPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}