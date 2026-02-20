using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(EnemyAnimationController))]
[RequireComponent(typeof(TagCombat))]
public class EnemyFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform player;          // Assign player here
    public float detectionRadius = 5f;
    public float moveSpeed = 2f;

    [Header("Optional")]
    public bool facePlayer = true;

    [Header("Attack Settings")]
    public float attackRange = 1f;
    public float attackCooldown = 2f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private EnemyAnimationController animController;
    private TagCombat combatScript;

    private float lastAttackTime = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animController = GetComponent<EnemyAnimationController>();
        combatScript = GetComponent<TagCombat>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRadius)
        {
            FollowPlayer();

            // Attack if in range
            if (distance <= attackRange && Time.time > lastAttackTime + attackCooldown)
            {
                Attack();
            }
        }
        else
        {
            StopMoving();
        }
    }

    void FollowPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        // Only horizontal movement
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        // Flip sprite
        if (facePlayer)
            sr.flipX = direction.x < 0;

        // Walking animation
        animController.SetWalking(true);
    }

    void StopMoving()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animController.SetWalking(false);
    }

    void Attack()
    {
        lastAttackTime = Time.time;
        animController.TriggerAttack();
        combatScript.Attack();
    }

    // Draw detection radius
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
