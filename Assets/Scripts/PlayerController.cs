using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // --- Movement Variables ---
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 150.0f;

    // --- Shooting Variables ---
    public GameObject fireballPrefab;
    public Transform firePoint;

    // --- Flame Wave Variables ---
    public float flameWaveDamage = 25f;
    public float flameWaveRange = 6f;
    [Range(0, 360)]
    public float flameWaveAngle = 120f;
    public float flameWaveCooldown = 8f;
    private float flameWaveCooldownTimer = 0f;

    // --- Phoenix Form (Ultimate) Variables ---
    public float phoenixFormDuration = 6f;    // How long the ultimate lasts
    public float phoenixFormCooldown = 18f;   // Cooldown for the ultimate
    public float phoenixFormEnergyCost = 6f;  // Energy required (placeholder)
    public float phoenixFormSpeedBonus = 3f;  // Added move speed during ultimate
    public float phoenixFormDamageBonus = 10f;// Added damage during ultimate
    private float phoenixFormCooldownTimer = 0f; // Timer for ultimate cooldown
    private bool isInPhoenixForm = false;       // State tracker
    private Coroutine phoenixFormActiveCoroutine; // Reference to the duration coroutine

    // --- Energy (Placeholder) ---
    public float maxEnergy = 10f;
    private float currentEnergy;

    void Start()
    {
        // Initialize timers and states
        flameWaveCooldownTimer = 0f;
        phoenixFormCooldownTimer = 0f;
        isInPhoenixForm = false;
        currentEnergy = maxEnergy; // Start with full energy (placeholder)
    }

    void Update()
    {
        // --- Handle Cooldowns ---
        if (flameWaveCooldownTimer > 0f)
        {
            flameWaveCooldownTimer -= Time.deltaTime;
        }
        if (phoenixFormCooldownTimer > 0f)
        {
            phoenixFormCooldownTimer -= Time.deltaTime;
        }

        // --- Handle Inputs and Actions ---
        // Don't allow regular actions if in ultimate (can be changed later)
        // if (!isInPhoenixForm) // Optional: disable basic move/shoot during ult?
        // {
        HandleMovement();
        HandleShooting();
        HandleFlameWaveInput();
        // } // End optional block
        HandleUltimateInput();
    }

    void HandleMovement()
    {
        // Apply speed bonus if in Phoenix Form
        float currentMoveSpeed = moveSpeed;
        if (isInPhoenixForm)
        {
            currentMoveSpeed += phoenixFormSpeedBonus;
            // We would add flight logic here later
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        transform.Translate(Vector3.forward * verticalInput * currentMoveSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * horizontalInput * rotateSpeed * Time.deltaTime);
    }

    void HandleShooting()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (isInPhoenixForm)
            {
                Debug.Log("Phoenix Form: Free Fireball!");
                // Add logic later to bypass energy cost if implemented
            }

            if (fireballPrefab != null && firePoint != null)
            {
                Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);
            }
            else
            {
                Debug.LogError("Fireball Prefab or Fire Point not assigned!");
            }
        }
    }

    void HandleFlameWaveInput()
    {
        if (Input.GetButtonDown("Fire2") && flameWaveCooldownTimer <= 0f)
        {
            CastFlameWave();
            flameWaveCooldownTimer = flameWaveCooldown;
        }
    }

    void HandleUltimateInput()
    {
        // Check for 'R' key press (can be changed), if not already active, off cooldown, and enough energy
        if (Input.GetKeyDown(KeyCode.R) && !isInPhoenixForm && phoenixFormCooldownTimer <= 0f && currentEnergy >= phoenixFormEnergyCost)
        {
            StartPhoenixForm();
        }
        // Optional: Allow early cancel?
        // else if (Input.GetKeyDown(KeyCode.R) && isInPhoenixForm) {
        //     EndPhoenixForm();
        // }
    }

    void CastFlameWave()
    {
        // Apply damage bonus if in Phoenix Form
        float currentFlameWaveDamage = flameWaveDamage;
        if (isInPhoenixForm)
        {
            currentFlameWaveDamage += phoenixFormDamageBonus;
            Debug.Log("Phoenix Form: Enhanced Flame Wave!");
        }

        Debug.Log("Casting Flame Wave!");
        Collider[] hitColliders = Physics.OverlapSphere(firePoint.position, flameWaveRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform == transform) continue;
            Vector3 directionToTarget = (hitCollider.transform.position - firePoint.position).normalized;
            float angleToTarget = Vector3.Angle(firePoint.forward, directionToTarget);

            if (angleToTarget <= flameWaveAngle / 2)
            {
                Target target = hitCollider.GetComponent<Target>();
                if (target != null)
                {
                    Debug.Log("Flame Wave hit TARGET: " + hitCollider.gameObject.name);
                    // Use the potentially enhanced damage
                    target.TakeDamage(currentFlameWaveDamage);
                    target.ApplyBurn();
                }
            }
        }
    }

    // --- Phoenix Form Methods ---
    void StartPhoenixForm()
    {
        if (isInPhoenixForm) return; // Prevent starting if already active

        isInPhoenixForm = true;
        currentEnergy -= phoenixFormEnergyCost; // Consume energy
        phoenixFormCooldownTimer = phoenixFormCooldown; // Start cooldown

        Debug.Log("PHOENIX FORM ACTIVATED! Duration: " + phoenixFormDuration + "s");
        // Add visual/audio activation cues here later

        // Stop previous timer coroutine if it exists (e.g., if canceled early)
        if (phoenixFormActiveCoroutine != null)
        {
            StopCoroutine(phoenixFormActiveCoroutine);
        }
        // Start the timer coroutine
        phoenixFormActiveCoroutine = StartCoroutine(PhoenixFormTimer());
    }

    IEnumerator PhoenixFormTimer()
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(phoenixFormDuration);
        // Duration ended, call EndPhoenixForm
        EndPhoenixForm();
    }

    void EndPhoenixForm()
    {
        // Check if it's actually active before ending
        if (!isInPhoenixForm) return;

        isInPhoenixForm = false;
        phoenixFormActiveCoroutine = null; // Clear coroutine reference
        Debug.Log("Phoenix Form deactivated.");
        // Add visual/audio deactivation cues here later
    }

    // --- Gizmos ---
    void OnDrawGizmosSelected()
    {
        if (firePoint == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(firePoint.position, flameWaveRange);
        Vector3 forward = firePoint.forward;
        Vector3 leftRayDirection = Quaternion.Euler(0, -flameWaveAngle / 2, 0) * forward;
        Vector3 rightRayDirection = Quaternion.Euler(0, flameWaveAngle / 2, 0) * forward;
        Gizmos.DrawRay(firePoint.position, leftRayDirection * flameWaveRange);
        Gizmos.DrawRay(firePoint.position, rightRayDirection * flameWaveRange);
    }
}