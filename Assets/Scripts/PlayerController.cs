using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Variables for speed - 'public' makes them visible in Unity's Inspector
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 150.0f;

    // Start is called before the first frame update
    void Start()
    {
        // We can leave Start empty for now
    }

    // Update is called once per frame
    void Update()
    {
        // --- Input ---
        // Get horizontal input (A/D keys or Left/Right Arrows by default)
        float horizontalInput = Input.GetAxis("Horizontal");
        // Get vertical input (W/S keys or Up/Down Arrows by default)
        float verticalInput = Input.GetAxis("Vertical");

        // --- Movement ---
        // Move the object forward/backward based on vertical input
        // Vector3.forward is shorthand for (0, 0, 1)
        // Time.deltaTime makes movement frame-rate independent
        transform.Translate(Vector3.forward * verticalInput * moveSpeed * Time.deltaTime);

        // --- Rotation ---
        // Rotate the object left/right based on horizontal input
        // Vector3.up is shorthand for (0, 1, 0) - rotates around the Y-axis
        transform.Rotate(Vector3.up * horizontalInput * rotateSpeed * Time.deltaTime);
    }
}