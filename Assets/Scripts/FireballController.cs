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

    // We'll add collision detection later
}
