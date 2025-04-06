using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    public float speed = 15.0f; // How fast the fireball travels
    public float lifetime = 2.0f; // How long the fireball exists in seconds

    // Start is called before the first frame update
    void Start()
    {
        // Destroy the fireball GameObject after 'lifetime' seconds
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        // Move the fireball forward constantly
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    // This function is called automatically when this trigger collider overlaps with another collider
    void OnTriggerEnter(Collider other)
    {
        // Try to get the Target component from the object we hit
        Target target = other.GetComponent<Target>();

        // Check if the object we hit actually has the Target script attached
        if (target != null)
        {
            // We hit a target! Call its TakeDamage function
            Debug.Log("Fireball hit TARGET: " + other.gameObject.name);
            target.TakeDamage(10f); // Deal 10 damage (arbitrary amount for now)
            target.ApplyBurn(); // Tell the target to start burning
        }
        else
        {
            // We hit something that wasn't a target (like maybe just a wall)
            Debug.Log("Fireball hit something else: " + other.gameObject.name);
        }

        // Destroy the fireball immediately upon hitting anything
        Destroy(gameObject);
    }
}