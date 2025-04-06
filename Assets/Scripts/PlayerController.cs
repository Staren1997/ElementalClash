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
    public float fireballBaseDamage = 10f; // Base damage for regular Fireball
    public GameObject flameWaveVFXPrefab; // Reference to the Flame Wave VFX

    // --- Flame Wave Variables ---
    public float flameWaveDamage = 25f;
    public float flameWaveRange = 6f;
    [Range(0, 360)]
    public float flameWaveAngle = 120f;
    public float flameWaveCooldown = 8f;
    private float flameWaveCooldownTimer = 0f;

    // --- Phoenix Form (Ultimate) Variables ---
    public float phoenixFormDuration = 6f;
    public float phoenixFormCooldown = 18f;
    public float phoenixFormEnergyCost = 6f;
    public float phoenixFormSpeedBonus = 3f;
    public float phoenixFormDamageBonus = 10f; // Bonus damage for BOTH abilities during ultimate
    private float phoenixFormCooldownTimer = 0f;
    private bool isInPhoenixForm = false;
    private Coroutine phoenixFormActiveCoroutine;

    // --- Energy (Placeholder) ---
    public float maxEnergy = 10f;
    private float currentEnergy;

    void Start()
    {
        flameWaveCooldownTimer = 0f;
        phoenixFormCooldownTimer = 0f;
        isInPhoenixForm = false;
        currentEnergy = maxEnergy;
    }

    void Update()
    {
        if (flameWaveCooldownTimer > 0f) { flameWaveCooldownTimer -= Time.deltaTime; }
        if (phoenixFormCooldownTimer > 0f) { phoenixFormCooldownTimer -= Time.deltaTime; }

        HandleMovement();
        HandleShooting();
        HandleFlameWaveInput();
        HandleUltimateInput();
    }

    void HandleMovement()
    {
        float currentMoveSpeed = moveSpeed;
        if (isInPhoenixForm) { currentMoveSpeed += phoenixFormSpeedBonus; }
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        transform.Translate(Vector3.forward * verticalInput * currentMoveSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * horizontalInput * rotateSpeed * Time.deltaTime);
    }

    // MODIFIED HandleShooting
    void HandleShooting()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (fireballPrefab != null && firePoint != null)
            {
                // Create the fireball instance
                GameObject spawnedFireball = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);
                // Try to get its controller script
                FireballController fbController = spawnedFireball.GetComponent<FireballController>();

                if (fbController != null)
                {
                    // Calculate damage based on state
                    float currentFireballDamage = fireballBaseDamage;
                    if (isInPhoenixForm)
                    {
                        Debug.Log("Phoenix Form: Enhanced Fireball!");
                        currentFireballDamage += phoenixFormDamageBonus;
                        // Add logic later to bypass energy cost if implemented
                    }
                    // Set the damage on the spawned fireball instance
                    fbController.damage = currentFireballDamage;
                }
                else
                {
                    Debug.LogError("Spawned Fireball is missing FireballController script!");
                }
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
        if (Input.GetKeyDown(KeyCode.R) && !isInPhoenixForm && phoenixFormCooldownTimer <= 0f && currentEnergy >= phoenixFormEnergyCost)
        {
            StartPhoenixForm();
        }
    }

    void CastFlameWave()
    {
        float currentFlameWaveDamage = flameWaveDamage;
        if (isInPhoenixForm)
        {
            currentFlameWaveDamage += phoenixFormDamageBonus;
            Debug.Log("Phoenix Form: Enhanced Flame Wave!");
        }
        Debug.Log("Casting Flame Wave!");
        // Instantiate the VFX at the fire point
        if (flameWaveVFXPrefab != null && firePoint != null)
        {
            Instantiate(flameWaveVFXPrefab, firePoint.position, firePoint.rotation);
        }
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
                    Debug.Log("Flame Wave hit TARGET: " + hitCollider.gameObject.name + " dealing " + currentFlameWaveDamage + " damage."); // Log damage
                    target.TakeDamage(currentFlameWaveDamage);
                    target.ApplyBurn();
                }
            }
        }
    }

    void StartPhoenixForm()
    {
        if (isInPhoenixForm) return;
        isInPhoenixForm = true;
        currentEnergy -= phoenixFormEnergyCost;
        phoenixFormCooldownTimer = phoenixFormCooldown;
        Debug.Log("PHOENIX FORM ACTIVATED! Duration: " + phoenixFormDuration + "s");
        if (phoenixFormActiveCoroutine != null) { StopCoroutine(phoenixFormActiveCoroutine); }
        phoenixFormActiveCoroutine = StartCoroutine(PhoenixFormTimer());
    }

    IEnumerator PhoenixFormTimer()
    {
        yield return new WaitForSeconds(phoenixFormDuration);
        EndPhoenixForm();
    }

    void EndPhoenixForm()
    {
        if (!isInPhoenixForm) return;
        isInPhoenixForm = false;
        phoenixFormActiveCoroutine = null;
        Debug.Log("Phoenix Form deactivated.");
    }

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