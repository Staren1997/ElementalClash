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
        // 'other' is the collider that we hit

        // Print the name of the object we hit to the console for testing
        Debug.Log("Fireball hit: " + other.gameObject.name);

        // Destroy the fireball immediately upon hitting something
        Destroy(gameObject);

        // Later, we'll add code here to check *what* we hit (e.g., an enemy)
        // and apply damage or effects.
    }
}