// Author: Juan Pablo Camporeale
// File: Player.cs
// Date: 11/12/2021

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Support Classes

    #endregion

    [SerializeField] private PlayAudioEvent m_hitReactionSFXEvent;
    [SerializeField] private AudioKey m_attackSFX;
    [SerializeField] private AudioKey m_chargeAttackSFX;
    [SerializeField] private AudioKey m_highkickAttackSFX;
    [SerializeField] private AudioKey m_hitReactionSFX;
    [SerializeField] private AudioKey m_pickupReactionSFX;


    public int health;                            // Player health
    public int damage;                            // Player damage
    public int facing = 1;                        // Player facing direction, 1 for right and -1 for left
    public float speed;                           // Player speed
    public float slideSpeed;                      // Player slide speed
    public float slideTime;                       // Slide time
    public float attackTime;                      // Attack time
    public float invencibilityTime;               // Invencibility time
    public float chargeTime = 0f;                 // Charge time
    public Vector2 knockbackForce;                // Knockback force
    public int knockbackAmount;                   // Knockback amount
    public bool isDead = false;                   // Is player dead?
    public bool isCharging;                       // Is player charging?
    public bool isInvulnerable;                   // Is player invulnerable?

    private float initialHealth;                  // Player initial health
    private float movement;                       // Player movement detector
    private float iTime;                          // Invulnerability timer
    private Vector2 initialPos;                   // Initial posotion
    private bool iTilt;                           // Check for tilt color
    private bool isSliding;                       // Is player sliding?
    private bool isPunching;                      // Is player punching?


    public GameObject hurtParticles;
    public GameObject chargeParticles;
    public GameObject slideParticles;

    Rigidbody2D body;
    Animator animator;
    BoxCollider2D playerHitBox;
    SpriteRenderer sprite;

    void Start()
    {
        // Set the variables
        initialHealth = (float)health;
        initialPos = transform.position;
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerHitBox = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (iTime > 0)
        {
            iTime -= Time.deltaTime;
            playerHitBox.enabled = false;
            if (GameManager.Instance.pauseMenu.activeSelf == false)
            {
                if (iTilt)
                    sprite.color = Color.clear;
                else
                    sprite.color = Color.white;
                iTilt = !iTilt;
            }
            if (iTime > 1)
                if (facing == -1)
                    Instantiate(slideParticles, transform.position + new Vector3(2, -5, 0), transform.rotation);
                else
                    Instantiate(slideParticles, transform.position + new Vector3(-2, -5, 0), transform.rotation);

            if (!isCharging && iTime > 0.5)
                animator.SetBool("TriggerCheck", true);
        }
        else if (isInvulnerable)
        {
            isInvulnerable = false;
            playerHitBox.enabled = true;
            sprite.color = Color.white;
        }

        if (!isSliding && !isPunching && !isDead)
        {
            movement = Input.GetAxisRaw("Horizontal");

            if (Input.GetButtonDown("Punch"))
            {
                animator.SetBool("TriggerCheck", false);
                movement = 0f;
                animator.SetTrigger("Charge");
                isCharging = true;
                Instantiate(chargeParticles, transform.position, transform.rotation);
            }

            if (Input.GetButton("Punch"))                 // Check key input for 'J' and 'Z' (Attack)
            {
                movement = 0f;
                chargeTime += Time.deltaTime;
                if (chargeTime >= 3)
                {
                    animator.SetFloat("Charging", 1f);
                }
                else
                {
                    animator.SetFloat("Charging", 0f);
                }
            }

            if (Input.GetButtonUp("Punch"))
            {
                if (chargeTime >= 3)
                {
                    PlaySFX(m_chargeAttackSFX);
                    StartCoroutine(Attack(0.17f));
                    animator.SetTrigger("SuperP");
                    knockbackForce.x = 0;
                }
                else
                {
                    PlaySFX(m_attackSFX);
                    StartCoroutine(Attack(0f));
                    animator.SetTrigger("Punch");
                    animator.SetFloat("Attack", 0.5f);
                }
                chargeTime = 0;
                isCharging = false;
                //animator.SetBool("TriggerCheck", true);
            }

            if (Input.GetButtonDown("High Kick") && !isSliding && !isPunching && !isCharging)             // Check key input for 'K' and 'X' (Attack)
            {
                movement = 0f;
                PlaySFX(m_highkickAttackSFX);
                StartCoroutine(Attack(0.23f));
                animator.SetTrigger("High Kick");
                animator.SetFloat("Attack", 1f);
                animator.SetBool("TriggerCheck", true);
            }

            if (Input.GetButtonDown("Low Kick") && !isSliding && !isPunching && !isCharging)              // Check key input for 'L' and 'C' (Attack)
            {
                movement = 0f;
                PlaySFX(m_attackSFX);
                StartCoroutine(Attack(-0.25f));
                animator.SetTrigger("Low Kick");
                animator.SetFloat("Attack", 0f);
                animator.SetBool("TriggerCheck", true);
            }

            if (Input.GetButtonDown("Slide") && !isSliding && !isPunching && !isCharging)                 // Check key input for "left shift" (Slide)
            {
                chargeTime = 0;
                StartCoroutine(Slide());
                animator.SetTrigger("Slide");
                animator.SetBool("TriggerCheck", true);
            }
        }
    }

    void FixedUpdate()
    {
        // Dead check
        if (!isDead)
        {
            if (!isCharging)
            {
                knockbackForce = Vector2.Lerp(knockbackForce, Vector2.zero, 8 * Time.deltaTime);
                body.velocity = new Vector2((movement + knockbackForce.x) * speed, 0f);
            }

            // Sliding and Punching checks 
            if (!isSliding && !isPunching)
            {
                if (Input.GetAxisRaw("Horizontal") < -0.1f)         // Check key input for left side (Movement)
                {
                    // Flip the sprite to the proper direction
                    transform.eulerAngles = new Vector2(0, 180);
                    facing = -1;
                }
                else if (Input.GetAxisRaw("Horizontal") > 0.1f)     // Check key input for right side (Movement)
                {
                    // Flip the sprite to the proper direction
                    transform.eulerAngles = new Vector2(0, 0);
                    facing = 1;
                }
                animator.SetFloat("Speed", Mathf.Abs(movement));
            }
            else if (!isPunching)
                body.velocity = new Vector2(movement * slideSpeed, 0f);

            if (isSliding && movement != 0)
            {
                Instantiate(slideParticles, transform.position + new Vector3(0, -5, 0), transform.rotation);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isInvulnerable)
        {
            health -= damage;

            GameManager.Instance.HPBar.value = health / initialHealth;

            if (!isCharging)
            {
                // Condition to trigger the animation
                animator.SetTrigger("Hurt");
                animator.SetBool("TriggerCheck", true);
            }
            PlaySFX(m_hitReactionSFX);
            Instantiate(hurtParticles, transform.position, transform.rotation);


            // Health check
            if (health <= 0)
            {
                isDead = true;
                animator.SetBool("Dead", true);

                // Display the Dead Menu
                GameManager.Instance.TurnOnDeadMenu();
            }
            else
            {
                iTime = 1.5f;
                isInvulnerable = true;
            }
        }
    }

    IEnumerator Attack(float extraTime)
    {
        movement = 0f;
        isPunching = true;
        yield return new WaitForSeconds(attackTime + extraTime);
        isPunching = false;
    }


    IEnumerator Slide()
    {
        isSliding = true;
        body.gravityScale = 0;
        playerHitBox.enabled = false;
        yield return new WaitForSeconds(slideTime);
        isSliding = false;
        playerHitBox.enabled = true;
        body.gravityScale = 10;
    }

    public void RestartPlayer()
    {
        health = (int)initialHealth;
        transform.position = initialPos;
        animator.SetBool("Dead", false);
        isDead = false;
        isSliding = false;
        isPunching = false;
        isCharging = false;
        chargeTime = 0;
        GameManager.Instance.HPBar.value = health / initialHealth;
        movement = 0f;
        knockbackForce.x = 0;
    }

    private void PlaySFX(AudioKey _key)
    {
        PlayAudioEvent.Data data = new PlayAudioEvent.Data()
        {
            AudioKey = _key
        };
        m_hitReactionSFXEvent.Raise(data);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Pickup pickup = other.GetComponent<Pickup>();
        if (pickup != null)
        {
            ApplyHealth(pickup.OnPickedUp());
            Destroy(pickup.gameObject);
        }
    }

    private void ApplyHealth(int _amount)
    {
        health += _amount;
        health = (int)Mathf.Clamp(health, 0, initialHealth);

        PlayAudioEvent.Data d = new PlayAudioEvent.Data()
        {
            AudioKey = m_pickupReactionSFX
        };
        m_hitReactionSFXEvent.Raise(d);
    }
}