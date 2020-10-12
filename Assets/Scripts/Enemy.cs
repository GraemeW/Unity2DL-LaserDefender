using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Tunables
    [Header("Ship Detail")]
    [SerializeField] float health = 100;
    [SerializeField] float shipPadding = 1.0f;
    [SerializeField] float scorePoints = 50.0f;

    [Header("Ship Damage")]
    [SerializeField] GameObject explosionEffect = null;
    [SerializeField] float deathAfterExplosion = 1.0f;
    [SerializeField] AudioClip deathSound = null;
    [SerializeField] [Range(0, 1)] float deathSoundVolume = 0.1f;

    [Header("Ship Weapons")]
    [SerializeField] GameObject laser = null;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3.0f;
    [SerializeField] float shipProjectileForce = 50.0f;

    // State
    float shotCounter = 0.0f;

    // Cached References
    Rigidbody2D shipRigidbody2D = null;
    Level level = null;

    // Start is called before the first frame update
    void Start()
    {
        InitializeShotCounter();
        shipRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        level = FindObjectOfType<Level>();
    }

    // Update is called once per frame
    void Update()
    {
        CountDownAndShoot();
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

    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;
        if (shotCounter < 0)
        {
            GameObject laserObject = Instantiate(laser, shipRigidbody2D.position + Vector2.down * shipPadding, Quaternion.identity);
            Laser laserShot = laserObject.GetComponent<Laser>();
            laserShot.Launch(Vector2.down, shipProjectileForce); // upward force applied since ship fixed face up
            AudioClip laserSound = laserShot.GetLaserSound();
            if (laserSound != null)
            {
                AudioSource.PlayClipAtPoint(laserShot.GetLaserSound(), Camera.main.transform.position, laserShot.GetLaserVolume());
            }
            InitializeShotCounter();
        }
    }

    private void InitializeShotCounter()
    {
        shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        if (health <= 0)
        {
            EnemyPathing enemyPathing = gameObject.GetComponent<EnemyPathing>();
            enemyPathing.KillPathingUpdates();
            KillShip();
        }
        if (!damageDealer.IsShip())
        {
            damageDealer.Hit();
        }
    }

    public void KillShip()
    {
        // Increment score
        level.AddToScore(scorePoints);

        // Get rid of stuff ++ play FX
        Destroy(gameObject);
        AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position, deathSoundVolume);
        GameObject explosion = Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(explosion, deathAfterExplosion);
    }

    public float GetShipPadding()
    {
        return shipPadding;
    }
}
