using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyAnimationController))]
[RequireComponent(typeof(TagCombat))]
[RequireComponent(typeof(Health))]
public class EnemyBrain : MonoBehaviour
{
    [Header("AI Settings")]
    public float detectionRadius = 8f;
    public float moveSpeed = 2.5f;
    public string playerTag = "Player";

    [Header("Combat Settings")]
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;

    // --- Private References ---
    private Rigidbody2D rb;
    private EnemyAnimationController animController;
    private TagCombat combatScript;
    private Health health;
    private Transform playerTransform;
    private float lastAttackTime;

    void Awake()
    {
        // Get references to all the other scripts on this object
        rb = GetComponent<Rigidbody2D>();
        animController = GetComponent<EnemyAnimationController>();
        combatScript = GetComponent<TagCombat>();
        health = GetComponent<Health>();
    }

    void Start()
    {
        // Find the player at the start of the game
        GameObject playerObj = GameObject.FindWithTag(playerTag);
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogError("EnemyBrain could not find GameObject with tag: " + playerTag);
        }
    }

    void Update()
    {
        // If this enemy is dead or can't find the player, do nothing.
        if (health.currentHealth <= 0 || playerTransform == null)
        {
            // Ensure movement and animations are stopped
            if (rb.linearVelocity.x != 0)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                animController.SetWalking(false);
            }
            return;
        }

        // --- Main AI Logic ---
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
        {
            // If in attack range and cooldown is over, ATTACK
            Attack();
        }
        else if (distanceToPlayer <= detectionRadius && distanceToPlayer > attackRange)
        {
            // If in detection range but not attack range, CHASE
            Chase();
        }
        else
        {
            // If player is out of range, go IDLE
            Idle();
        }
    }

    void Chase()
    {
        // Tell the animation controller to play the walking animation
        animController.SetWalking(true);

        // Calculate direction and move towards the player
        float moveDirection = Mathf.Sign(playerTransform.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);
        Flip(moveDirection);
    }

    void Attack()
    {
        lastAttackTime = Time.time;
        StopMoving(); // Stop before attacking

        // Tell the other scripts to perform their attack actions
        animController.TriggerAttack();
        combatScript.Attack();
    }

    void Idle()
    {
        StopMoving();
    }

    void StopMoving()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animController.SetWalking(false);
    }

    void Flip(float direction)
    {
        if (direction > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (direction < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}