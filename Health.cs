using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Defend Settings")]
    [Range(0f, 1f)]
    public float damageReductionFactor = 0.5f;

    [Header("Effects")]
    // Drag your "DamagePopup_Prefab" here in the Inspector
    public GameObject damagePopupPrefab;

    [Header("UI (Optional)")]
    public Text healthText;

    // --- Private Component References ---
    private Animator animController;
    private HeroKnight heroKnight; // Reference to the player's main script

    // --- Events ---
    public delegate void OnDeathHandler(GameObject deadObject);
    public event OnDeathHandler OnDeath;
    public delegate void OnHealthChangedHandler(int current, int max);
    public event OnHealthChangedHandler OnHealthChanged;

    void Awake()
    {
        animController = GetComponent<Animator>();
        // Get the HeroKnight script only if it exists on this character
        heroKnight = GetComponent<HeroKnight>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthDisplay();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        bool isCurrentlyBlocking = false;
        // Check if this character is the player and if they are blocking
        if (heroKnight != null)
        {
            isCurrentlyBlocking = heroKnight.IsBlocking();
        }

        // Reduce damage if the character is blocking
        if (isCurrentlyBlocking)
        {
            damage = Mathf.CeilToInt(damage * damageReductionFactor);
        }

        // --- NEW ---
        // If a prefab is assigned, create the floating damage number
        if (damagePopupPrefab != null && damage > 0)
        {
            GameObject popup = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
            popup.GetComponent<DamagePopup>().Setup(damage);
        }
        // --- END NEW ---

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthDisplay();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log(transform.name + " took " + damage + " damage. Health: " + currentHealth);

        if (currentHealth > 0)
        {
            if (animController != null)
                animController.SetTrigger("Hurt");
        }
        else
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        UpdateHealthDisplay();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log(transform.name + " healed. Health: " + currentHealth);
    }

    private void UpdateHealthDisplay()
    {
        if (healthText != null)
            healthText.text = currentHealth.ToString();
    }

    void Die()
    {
        Debug.Log(transform.name + " died.");
        if (animController != null)
            animController.SetTrigger("Death");

        // Disable the main controller script if it exists
        if (heroKnight != null)
        {
            heroKnight.enabled = false;
        }

        // Disable the collider
        GetComponent<Collider2D>().enabled = false;
        // Disable this script
        this.enabled = false;

        OnDeath?.Invoke(gameObject);
        Destroy(gameObject, 2f);
    }
}