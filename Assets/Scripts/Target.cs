using UnityEngine;

public class Target : MonoBehaviour
{
    // This function can be called by other scripts (like our Fireball)
    public void TakeDamage(float amount)
    {
        Debug.Log(gameObject.name + " took " + amount + " damage!");

        // For this simple test, we'll just destroy the target immediately when hit.
        // Later, this would reduce health, check for death, etc.
        Destroy(gameObject);
    }
}