using System.Collections;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public GameObject projectilePrefab;
    public GameObject floatingTextPrefab;
    public float fireInterval = 2f;
    public float baseDamage = 1f;
    public int diceSides = 6;

    public enum PassiveAbility { None, FasterFire, BiggerProjectile, DoubleShot }
    public PassiveAbility passive = PassiveAbility.None;

    private Coroutine fireRoutine;

    void OnEnable()
    {
        GameEvents.OnCombatStarted += StartFiring;
        GameEvents.OnCombatEnded += StopFiring;

        if (GameManager.Instance != null && GameManager.Instance.IsCombatActive)
            StartFiring();
    }

    void OnDisable()
    {
        GameEvents.OnCombatStarted -= StartFiring;
        GameEvents.OnCombatEnded -= StopFiring;

        StopFiring();
    }

    void ApplyPassive()
    {
        if (passive == PassiveAbility.FasterFire)
            fireInterval *= 0.5f;
    }

    void Start()
    {
        ApplyPassive();
    }

    void StartFiring()
    {
        if (fireRoutine == null)
            fireRoutine = StartCoroutine(FireLoop());
    }

    void StopFiring()
    {
        if (fireRoutine != null)
        {
            StopCoroutine(fireRoutine);
            fireRoutine = null;
        }
    }

    IEnumerator FireLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireInterval);
            FireProjectile();
        }
    }

    void FireProjectile()
    {
        if (projectilePrefab == null) return;

        int rolledValue = Random.Range(1, diceSides + 1);
        float damage = baseDamage * rolledValue;

        Vector3 spawnPos = transform.position;
        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        Projectile homing = proj.GetComponent<Projectile>();
        if (homing != null)
        {
            homing.damage = damage;
        }

        if (floatingTextPrefab != null)
        {
            GameObject textGO = Instantiate(floatingTextPrefab, spawnPos + Vector3.up * 0.5f, Quaternion.identity);
            FloatingText textScript = textGO.GetComponent<FloatingText>();
            if (textScript != null)
                textScript.Show(rolledValue);
        }
    }
}
