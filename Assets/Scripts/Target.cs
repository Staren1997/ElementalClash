using System.Collections; // Needed for Coroutines
using UnityEngine;

public class Target : MonoBehaviour
{
    // Health Variables
    public float maxHealth = 100f;
    private float currentHealth;

    // Burn Status Variables
    public float burnDamagePerTick = 5f; // Damage dealt per burn tick
    public float burnTickInterval = 1f;  // How often burn damage is applied (in seconds)
    public float burnDuration = 3f;    // How long the burn lasts (in seconds)
    private bool isBurning = false;     // Is the target currently burning?
    private Coroutine burnCoroutine;    // Reference to the active burn coroutine

    void Start()
    {
        currentHealth = maxHealth;
        isBurning = false; // Ensure not burning initially
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log(gameObject.name + " took " + amount + " damage! Current Health: " + currentHealth);

        if (currentHealth <= 0f && !isBurning) // Check if dead AND not already burning (to avoid double Die() calls)
        {
            Die();
        }
        else if (currentHealth <= 0f && isBurning)
        {
            // If burning caused death, coroutine handles the Die() call after last tick
            // We could potentially stop the coroutine early here too if needed
        }
    }

    // Called by the Fireball to start the burn effect
    public void ApplyBurn()
    {
        if (!isBurning) // Start burning only if not already burning
        {
            isBurning = true;
            // Start the coroutine and store a reference to it
            burnCoroutine = StartCoroutine(BurnDamageOverTime());
            Debug.Log(gameObject.name + " started burning!");
        }
        // Optional: If already burning, maybe refresh the duration? For now, we just ignore subsequent ApplyBurn calls.
    }

    // Coroutine to handle Damage over Time
    IEnumerator BurnDamageOverTime()
    {
        float timer = burnDuration; // Initialize timer for this burn instance

        while (timer > 0)
        {
            // Wait for the specified interval
            yield return new WaitForSeconds(burnTickInterval);

            // Apply burn damage ONLY if still alive
            if (currentHealth > 0)
            {
                Debug.Log(gameObject.name + " taking burn damage!");
                TakeDamage(burnDamagePerTick);
            }

            // Decrease the timer
            timer -= burnTickInterval;

            // If health dropped below zero during this tick, stop burning and Die
            if (currentHealth <= 0f)
            {
                timer = 0; // Force loop exit
                Die();     // Call Die immediately after fatal burn tick
            }
        }

        // Burn duration finished
        isBurning = false;
        Debug.Log(gameObject.name + " stopped burning.");
        burnCoroutine = null; // Clear the reference
    }


    void Die()
    {
        // Prevent multiple deaths / stop burn if Die() is called early by direct damage
        if (isBurning && burnCoroutine != null)
        {
            StopCoroutine(burnCoroutine);
            isBurning = false;
            burnCoroutine = null;
            Debug.Log(gameObject.name + " burning stopped due to death.");
        }

        if (gameObject != null) // Check if not already destroyed
        {
            Debug.Log(gameObject.name + " has been destroyed!");
            Destroy(gameObject);
        }
    }
}