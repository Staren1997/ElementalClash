using UnityEngine;

public class Target : MonoBehaviour
{
    public float maxHealth = 100f; // Set the maximum health
    private float currentHealth; // Track the current health

    void Start()
    {
        // Initialize health when the target is created
        currentHealth = maxHealth;
    }

    // This function can be called by other scripts (like our Fireball)
    public void TakeDamage(float amount)
    {
        // Subtract the damage amount from current health
        currentHealth -= amount;

        Debug.Log(gameObject.name + " took " + amount + " damage! Current Health: " + currentHealth);

        // Check if health has dropped to zero or below
        if (currentHealth <= 0f)
        {
            Die(); // Call the Die function if health is depleted
        }
    }

    // Function to handle what happens when the target dies
    void Die()
    {
        Debug.Log(gameObject.name + " has been destroyed!");

        // Destroy the target GameObject
        Destroy(gameObject);

        // Later, we could add explosion effects, score updates, etc. here
    }
}