using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    public Health playerHealth;
    // You can set the absolute max health cap here in the Inspector
    public int maxTotalHealthCap = 200;

    void OnEnable()
    {
        SubscribeToEnemies();
    }

    void SubscribeToEnemies()
    {
        Health[] enemies = FindObjectsOfType<Health>();
        foreach (var enemy in enemies)
        {
            if (enemy != playerHealth)
                enemy.OnDeath += OnEnemyKilled;
        }
    }

    void OnEnemyKilled(GameObject deadEnemy)
    {
        if (playerHealth == null) return;

        Debug.Log("Player killed: " + deadEnemy.name + ". Healing and increasing max HP.");

        // --- NEW LOGIC IS ALL HERE ---

        // 1. Increase the maximum health, but don't go over the cap
        playerHealth.maxHealth += 25;
        if (playerHealth.maxHealth > maxTotalHealthCap)
        {
            playerHealth.maxHealth = maxTotalHealthCap;
        }

        // 2. Add 25 to the current health
        playerHealth.currentHealth += 25;

        // 3. Make sure current health doesn't go over the new max health
        if (playerHealth.currentHealth > playerHealth.maxHealth)
        {
            playerHealth.currentHealth = playerHealth.maxHealth;
        }
    }

    void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        Health[] enemies = FindObjectsOfType<Health>();
        foreach (var enemy in enemies)
        {
            if (enemy != playerHealth)
                enemy.OnDeath -= OnEnemyKilled;
        }
    }
}