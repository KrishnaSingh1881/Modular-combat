using UnityEngine;

[RequireComponent(typeof(Health))]
public class AttackBooster : MonoBehaviour
{
    [Header("Attack Buff Settings")]
    public int attackBonus = 10;
    [Range(0f, 1f)]
    public float attackBuffChance = 0.2f;

    // --- NEW ---
    [Header("Full Heal Settings")]
    [Range(0f, 1f)]
    public float fullHealChance = 0.1f; // 10% chance for a full heal

    [Header("Player Reference")]
    // Drag the Player object here in the Inspector
    public TagCombat playerCombat;

    private Health characterHealth;

    void Awake()
    {
        characterHealth = GetComponent<Health>();
        characterHealth.OnDeath += OnCharacterDied;
    }

    private void OnCharacterDied(GameObject deadObject)
    {
        // Check for the Attack Buff
        if (Random.value <= attackBuffChance)
        {
            GrantAttackBuff();
        }

        // --- NEW ---
        // Separately, check for the Full Heal
        if (Random.value <= fullHealChance)
        {
            GrantFullHeal();
        }
    }

    private void GrantAttackBuff()
    {
        if (playerCombat != null)
        {
            playerCombat.attackDamage += attackBonus;
            Debug.Log("ATTACK BUFF! Player's new damage: " + playerCombat.attackDamage);
        }
        else
        {
            Debug.LogWarning("Attack Buff dropped, but no Player was assigned!", this);
        }
    }

    // --- NEW METHOD ---
    private void GrantFullHeal()
    {
        if (playerCombat != null)
        {
            // Get the Health script from the same player object that the combat script is on
            Health playerHealth = playerCombat.GetComponent<Health>();
            if (playerHealth != null)
            {
                // Call the existing Heal method with a very large number.
                // Your Heal method will automatically cap it at the player's maxHealth.
                playerHealth.Heal(9999);
                Debug.Log("FULL HEAL! Player's health was restored.");
            }
        }
        else
        {
            Debug.LogWarning("Full Heal dropped, but no Player was assigned!", this);
        }
    }

    private void OnDestroy()
    {
        if (characterHealth != null)
        {
            characterHealth.OnDeath -= OnCharacterDied;
        }
    }
}