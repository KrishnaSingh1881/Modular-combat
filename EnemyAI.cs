using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyAnimationController))]
[RequireComponent(typeof(TagCombat))]
public class EnemyAI : MonoBehaviour
{
    [Header("Detection & Movement")]
    public float detectionRadius = 5f;   // When player enters this radius, enemy starts moving
    public float moveSpeed = 2f;         // Horizontal movement speed
    public string targetTag = "Player";  // Dropdown in Inspector

    [Header("Attack")]
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;

    [Header("Facing Options")]
    public bool flipTowardsPlayer = true;  // Flip sprite to face player

    // --- Private ---
    private Transform player;
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
        FindPlayer();

        if (player == null)
        {
            // No player found, stop moving
            rb.linearVelocity = Vector2.zero;
            animController.SetWalking(false);
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRadius)
        {
            MoveTowardsPlayer();

            // Attack logic
            if (distance <= attackRange && Time.time > lastAttackTime + attackCooldown)
            {
                Attack();
            }
        }
        else
        {
            // Player out of range
            rb.linearVelocity = Vector2.zero;
            animController.SetWalking(false);
        }
    }

    void FindPlayer()
    {
        GameObject targetObj = GameObject.FindWithTag(targetTag);
        if (targetObj != null)
            player = targetObj.transform;
        else
            player = null;
    }

    void MoveTowardsPlayer()
    {
        float directionX = player.position.x - transform.position.x;
        Vector2 velocity = new Vector2(Mathf.Sign(directionX) * moveSpeed, 0f); // Only X movement
        rb.linearVelocity = velocity;

        // Walking animation
        animController.SetWalking(Mathf.Abs(directionX) > 0.01f);

        // Flip sprite
        if (flipTowardsPlayer)
            sr.flipX = (directionX < 0);
    }

    void Attack()
    {
        lastAttackTime = Time.time;
        animController.TriggerAttack();
        combatScript.Attack();
    }

    private void OnDrawGizmosSelected()
    {
        // Detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Attack radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
