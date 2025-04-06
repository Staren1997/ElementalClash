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

    // --- Flame Wave Variables ---
    public float flameWaveDamage = 25f; // Base damage for Flame Wave
    public float flameWaveRange = 6f;   // How far the wave reaches
    [Range(0, 360)] // Attribute to make angle a slider in Inspector
    public float flameWaveAngle = 120f; // The angle of the cone (120 degrees total)
    public float flameWaveCooldown = 8f;  // Cooldown time in seconds
    private float flameWaveCooldownTimer = 0f; // Timer to track cooldown

    // Start is called before the first frame update
    void Start()
    {
        // Make sure cooldown timer starts at 0 so we can fire immediately
        flameWaveCooldownTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // Handle Cooldowns
        // Decrease cooldown timer if it's greater than 0
        if (flameWaveCooldownTimer > 0f)
        {
            flameWaveCooldownTimer -= Time.deltaTime;
        }

        // Handle Inputs and Actions
        HandleMovement();
        HandleShooting();
        HandleFlameWaveInput(); // Call the new input method
    }

    void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        transform.Translate(Vector3.forward * verticalInput * moveSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * horizontalInput * rotateSpeed * Time.deltaTime);
    }

    void HandleShooting()
    {
        if (Input.GetButtonDown("Fire1")) // Fire1 for Fireball
        {
            if (fireballPrefab != null && firePoint != null)
            {
                Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);
            }
            else
            {
                Debug.LogError("Fireball Prefab or Fire Point not assigned in the Inspector!");
            }
        }
    }

    // Checks for Flame Wave input
    void HandleFlameWaveInput()
    {
        // Check if "Fire2" (Right Mouse Button by default) is pressed AND cooldown is ready
        if (Input.GetButtonDown("Fire2") && flameWaveCooldownTimer <= 0f)
        {
            CastFlameWave(); // Call the function to perform the attack
            flameWaveCooldownTimer = flameWaveCooldown; // Reset the cooldown timer
        }
    }

    // Performs the Flame Wave cone attack logic
    void CastFlameWave()
    {
        Debug.Log("Casting Flame Wave!");

        // --- Cone Detection Logic ---
        // Find all colliders within the flameWaveRange around the firePoint
        Collider[] hitColliders = Physics.OverlapSphere(firePoint.position, flameWaveRange);

        // Loop through all the colliders found
        foreach (var hitCollider in hitColliders)
        {
            // Ignore hitting the player itself
            if (hitCollider.transform == transform)
            {
                continue; // Skip to the next collider in the loop
            }

            // Calculate direction from the fire point (or player center) to the hit collider
            Vector3 directionToTarget = (hitCollider.transform.position - firePoint.position).normalized;

            // Calculate the angle between the player's forward direction and the direction to the target
            float angleToTarget = Vector3.Angle(firePoint.forward, directionToTarget);

            // Check if the angle is within half of our specified cone angle
            if (angleToTarget <= flameWaveAngle / 2)
            {
                // The collider is within the cone! Now check if it's a target
                Target target = hitCollider.GetComponent<Target>();
                if (target != null)
                {
                    // It's a target! Apply damage and burn
                    Debug.Log("Flame Wave hit TARGET: " + hitCollider.gameObject.name);
                    target.TakeDamage(flameWaveDamage);
                    target.ApplyBurn();
                }
                // Optional: Add logic here later to affect non-target objects if needed
                // Optional: Add logic here later for destroying projectiles hit by the wave
            }
        }
        // --- End Cone Detection Logic ---
    }
}