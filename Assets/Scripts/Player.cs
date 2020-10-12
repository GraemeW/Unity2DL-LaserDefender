using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Player : MonoBehaviour
{
    // Tunables
    [Header("Ship Detail")]
    [SerializeField] float shipPadding = 1.0f;
    public float shipSpeedFactor = 10.0f;
    float xMin = 0.0f;
    float xMax = 0.0f;
    float yMin = 0.0f;
    float yMax = 0.0f;
    float horizontal = 0.0f;
    float vertical = 0.0f;
    [SerializeField] float health = 500.0f;
    [SerializeField] float maxHealth = 500.0f;
    [SerializeField] float regen = 10.0f;

    [Header("Ship Damage")]
    [SerializeField] float hitDamageColorDuration = 0.05f;
    [SerializeField] AudioClip damageSound = null;
    [SerializeField] float damageVolume = 0.025f;
    [SerializeField] GameObject explosionEffect = null;
    [SerializeField] float deathAfterExplosion = 1.0f;
    [SerializeField] AudioClip deathSound = null;
    [SerializeField] [Range(0,1)] float deathSoundVolume = 0.3f;

    [Header("Ship Weapons")]
    public float shipProjectileForce = 100.0f;
    public float shipFiringPeriod = 0.1f;
    [SerializeField] GameObject laser = null;

    // State
    [SerializeField] float xScreenUnits = 5.625f;
    bool shipDead = false;

    // Cached References
    Rigidbody2D shipRigidbody2D = null;
    Coroutine firingCoroutine = null;
    AudioSource audioSource = null;
    SpriteRenderer spriteRenderer = null;
    Level level = null;

    // Start is called before the first frame update
    void Start()
    {
        SetMoveBoundaries();
        shipRigidbody2D = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        level = FindObjectOfType<Level>();
        StartCoroutine(RegenerateHealth());
    }

    // Update is called once per frame
    void Update()
    {
        // Get user input
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        // Fire weapons
        Fire();
    }

    private void FixedUpdate()
    {
        // Apply user input to move
        Move();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (damageDealer != null)
        {
            ProcessHit(damageDealer);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (damageDealer != null)
        {
            ProcessHit(damageDealer);
        }
    }

    private void Move()
    {
        // Update ship position
        float deltaX = horizontal * Time.deltaTime * shipSpeedFactor;
        float deltaY = vertical * Time.deltaTime * shipSpeedFactor;
        float newXPosition = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        float newYPosition = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);
        Vector2 newPosition = new Vector2(newXPosition, newYPosition);
        shipRigidbody2D.MovePosition(newPosition);
    }

    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            firingCoroutine = StartCoroutine(FireContinuously());
        }
        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
        }
    }

    IEnumerator FireContinuously()
    {
        while (true)
        {
            GameObject laserObject = Instantiate(laser, shipRigidbody2D.position + Vector2.up * shipPadding, Quaternion.identity);
            Laser laserShot = laserObject.GetComponent<Laser>();
            laserShot.Launch(Vector2.up, shipProjectileForce); // upward force applied since ship fixed face up
            AudioClip laserSound = laserShot.GetLaserSound();
            if (laserSound != null)
            {
                audioSource.PlayOneShot(laserSound);
            }
            yield return new WaitForSeconds(shipFiringPeriod);
        }
    }

    IEnumerator RegenerateHealth()
    {
        while (true)
        {
            health = Mathf.Clamp(health + regen, health, maxHealth);
            yield return new WaitForSeconds(1.0f);
        }
    }
    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        StartCoroutine(HitColorBlip());
        AudioSource.PlayClipAtPoint(damageSound, Camera.main.transform.position, damageVolume);
        if (!damageDealer.IsShip())
        {
            damageDealer.Hit();
        }
        if (health <= 0)
        {
            KillShip();
        }
    }

    IEnumerator HitColorBlip()
    {
        for(int colorState = 0; colorState < 2; colorState++)
        {
            if (colorState == 0) { spriteRenderer.color = new Color(255f, 0f, 0f, 255f); }
            else { spriteRenderer.color = new Color(255f, 255f, 255f, 255f); }
            yield return new WaitForSeconds(hitDamageColorDuration);
        }
    }

    private void KillShip()
    {
        // Increment score
        level.SetFinalWave(level.GetWave());

        // Get rid of stuff ++ play FX
        Destroy(gameObject);
        AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position, deathSoundVolume);
        GameObject explosion = Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(explosion, deathAfterExplosion);

        shipDead = true;
    }

    public bool IsShipDead()
    {
        return shipDead;
    }

    public float GetHealth()
    {
        return Mathf.Clamp(health, 0.0f, health);
    }

    private void SetMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        Vector3 cornerBottomLeft = new Vector3(0, 0, 0);
        Vector3 cornerTopRight = new Vector3(1, 1, 0);
        //xMin = gameCamera.ViewportToWorldPoint(cornerBottomLeft).x + shipPadding;
        xMin = -xScreenUnits + shipPadding; // full screen breaks bounding, so temporary hack
        yMin = gameCamera.ViewportToWorldPoint(cornerBottomLeft).y + shipPadding;
        //xMax = gameCamera.ViewportToWorldPoint(cornerTopRight).x - shipPadding;
        xMax = xScreenUnits - shipPadding; // full screen breaks bounding, so temporary hack
        yMax = gameCamera.ViewportToWorldPoint(cornerTopRight).y - shipPadding;
    }
}
