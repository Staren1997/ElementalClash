using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // --- Movement Variables ---
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 150.0f;

    // --- Shooting Variables ---
    public GameObject fireballPrefab; // Reference to the Fireball prefab
    public Transform firePoint; // Point where the fireball will spawn

    // Start is called before the first frame update
    void Start()
    {
        // We can leave Start empty for now
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleShooting(); // Call the new shooting method
    }

    void HandleMovement()
    {
        // --- Input ---
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // --- Movement ---
        transform.Translate(Vector3.forward * verticalInput * moveSpeed * Time.deltaTime);

        // --- Rotation ---
        transform.Rotate(Vector3.up * horizontalInput * rotateSpeed * Time.deltaTime);
    }

    // New method for handling shooting
    void HandleShooting()
    {
        // Check if the "Fire1" button (Left Mouse Button by default) is pressed down
        if (Input.GetButtonDown("Fire1"))
        {
            // Check if the fireballPrefab and firePoint have been assigned in the Inspector
            if (fireballPrefab != null && firePoint != null)
            {
                // Create an instance of the fireballPrefab at the firePoint's position and rotation
                Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);
            }
            else
            {
                // Log an error if references are missing (helpful for debugging)
                Debug.LogError("Fireball Prefab or Fire Point not assigned in the Inspector!");
            }
        }
    }
}