using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // --- Movement Variables ---
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 150.0f;

    // --- Energy ---
    public float maxEnergy = 10f;
    public float energyRegenRate = 0.2f; // Energy per second (1 / 5 seconds = 0.2)
    private float currentEnergy;

    // --- Shooting (Fireball) Variables ---
    public GameObject fireballPrefab;
    public Transform firePoint;
    public float fireballBaseDamage = 10f;
    public float fireballEnergyCost = 2f; // Energy cost for Fireball

    // --- Flame Wave Variables ---
    public GameObject flameWaveVFXPrefab; // <<< RE-ADDED THIS VARIABLE
    public float flameWaveDamage = 25f;
    public float flameWaveRange = 6f;
    [Range(0, 360)]
    public float flameWaveAngle = 120f;
    public float flameWaveCooldown = 8f;
    public float flameWaveEnergyCost = 4f; // Energy cost for Flame Wave (implement later)
    private float flameWaveCooldownTimer = 0f;

    // --- Phoenix Form (Ultimate) Variables ---
    public float phoenixFormDuration = 6f;
    public float phoenixFormCooldown = 18f;
    public float phoenixFormEnergyCost = 6f; // Energy cost for Ultimate
    public float phoenixFormSpeedBonus = 3f;
    public float phoenixFormDamageBonus = 10f; // Bonus damage for BOTH abilities during ultimate
    private float phoenixFormCooldownTimer = 0f;
    private bool isInPhoenixForm = false;
    private Coroutine phoenixFormActiveCoroutine;

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

        if (currentEnergy < maxEnergy)
        {
            currentEnergy += energyRegenRate * Time.deltaTime;
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
        }
        // Debug.Log("Current Energy: " + currentEnergy);

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

    void HandleShooting()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            bool canAfford = currentEnergy >= fireballEnergyCost;
            if (isInPhoenixForm)
            {
                Debug.Log("Phoenix Form: Free Fireball!");
                SpawnFireball();
            }
            else if (canAfford)
            {
                currentEnergy -= fireballEnergyCost;
                Debug.Log("Used " + fireballEnergyCost + " energy. Remaining: " + currentEnergy);
                SpawnFireball();
            }
            else
            {
                Debug.Log("Not enough energy for Fireball! Need: " + fireballEnergyCost + ", Have: " + currentEnergy);
            }
        }
    }

    void SpawnFireball()
    {
        if (fireballPrefab != null && firePoint != null)
        {
            GameObject spawnedFireball = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);
            FireballController fbController = spawnedFireball.GetComponent<FireballController>();
            if (fbController != null)
            {
                float currentFireballDamage = fireballBaseDamage;
                if (isInPhoenixForm) { currentFireballDamage += phoenixFormDamageBonus; }
                fbController.damage = currentFireballDamage;
            }
            else { Debug.LogError("Spawned Fireball is missing FireballController script!"); }
        }
        else { Debug.LogError("Fireball Prefab or Fire Point not assigned!"); }
    }


    void HandleFlameWaveInput()
    {
        // Check if enough energy for Flame Wave
        bool canAfford = currentEnergy >= flameWaveEnergyCost;

        // Check if "Fire2" (Right Mouse Button by default) is pressed AND cooldown is ready AND enough energy
        if (Input.GetButtonDown("Fire2") && flameWaveCooldownTimer <= 0f && canAfford)
        {
            CastFlameWave(); // Call the function to perform the attack
            currentEnergy -= flameWaveEnergyCost; // Deduct energy cost
            flameWaveCooldownTimer = flameWaveCooldown; // Reset the cooldown timer
            Debug.Log("Used " + flameWaveEnergyCost + " energy for Flame Wave. Remaining: " + currentEnergy);
        }
        // Optional: Add feedback if trying to cast without enough energy
        else if (Input.GetButtonDown("Fire2") && flameWaveCooldownTimer <= 0f && !canAfford)
        {
            Debug.Log("Not enough energy for Flame Wave! Need: " + flameWaveEnergyCost + ", Have: " + currentEnergy);
        }
    }

    void HandleUltimateInput()
    {
        bool canAffordUltimate = currentEnergy >= phoenixFormEnergyCost;
        if (Input.GetKeyDown(KeyCode.R) && !isInPhoenixForm && phoenixFormCooldownTimer <= 0f && canAffordUltimate)
        {
            StartPhoenixForm();
        }
        else if (Input.GetKeyDown(KeyCode.R) && !isInPhoenixForm && phoenixFormCooldownTimer <= 0f && !canAffordUltimate)
        {
            Debug.Log("Not enough energy for Phoenix Form! Need: " + phoenixFormEnergyCost + ", Have: " + currentEnergy);
        }
    }

    // CORRECTED CastFlameWave
    void CastFlameWave()
    {
        // Instantiate the VFX at the fire point <<< RE-ADDED THIS BLOCK
        if (flameWaveVFXPrefab != null && firePoint != null)
        {
            Instantiate(flameWaveVFXPrefab, firePoint.position, firePoint.rotation);
        } // <<< END RE-ADDED BLOCK

        float currentFlameWaveDamage = flameWaveDamage;
        if (isInPhoenixForm)
        {
            currentFlameWaveDamage += phoenixFormDamageBonus;
            Debug.Log("Phoenix Form: Enhanced Flame Wave!");
        }
        Debug.Log("Casting Flame Wave!"); // Moved this log after VFX spawn
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
                    Debug.Log("Flame Wave hit TARGET: " + hitCollider.gameObject.name + " dealing " + currentFlameWaveDamage + " damage.");
                    target.TakeDamage(currentFlameWaveDamage);
                    target.ApplyBurn();
                }
            }
        }
    }

    void StartPhoenixForm()
    {
        if (isInPhoenixForm) return;
        currentEnergy -= phoenixFormEnergyCost;
        isInPhoenixForm = true;
        phoenixFormCooldownTimer = phoenixFormCooldown;
        Debug.Log("PHOENIX FORM ACTIVATED! Energy Used: " + phoenixFormEnergyCost + ", Remaining: " + currentEnergy);
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