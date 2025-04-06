using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    public float speed = 15.0f;
    public float lifetime = 2.0f;
    public float damage = 10f; // Damage this specific fireball will deal

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Target target = other.GetComponent<Target>();
        if (target != null)
        {
            Debug.Log("Fireball hit TARGET: " + other.gameObject.name + " dealing " + damage + " damage."); // Log the damage dealt
            target.TakeDamage(damage); // Use the damage variable
            target.ApplyBurn();
        }
        else
        {
            Debug.Log("Fireball hit something else: " + other.gameObject.name);
        }
        Destroy(gameObject);
    }
}