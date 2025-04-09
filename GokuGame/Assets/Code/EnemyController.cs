using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Transform spawnLocation;  // Optional spawn location.
    public Transform player;         // Reference to the player. If not set, will try to find by tag "Player".

    [Header("Movement Settings")]
    public float runSpeed = 3f;
    public float jumpForce = 5f;
    public float randomJumpIntervalMin = 2f;
    public float randomJumpIntervalMax = 5f;

    [Header("Attack Settings")]
    public float attackRange = 0.1f;
    public float attackCooldown = 1f;

    [Header("Health Settings")]
    public int health = 100;

    [Header("Ground Settings")]
    public LayerMask groundLayer;    // Set this to your groundLayer in the Inspector.

    // Components
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private Collider2D coll;         // To help with ground checking.

    // Internal timers and state flags.
    private float nextAttackTime = 0f;
    private float nextJumpTime = 0f;
    private bool isDead = false;
    private bool isAttacking = false; // Flag indicating an attack is in progress.

    void Start()
    {
        if (spawnLocation != null)
        {
            transform.position = spawnLocation.position;
        }
        
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
        
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<Collider2D>();  // Get enemy's collider for ground checking.
    
        ScheduleNextJump();
    }

    void Update()
    {
        if (isDead || player == null)
            return;
    
        Vector2 toPlayer = player.position - transform.position;
        float distance = toPlayer.magnitude;
    
        // If within attack range, not attacking, and off cooldown, attack.
        if (distance <= attackRange && !isAttacking && Time.time >= nextAttackTime)
        {
            StartCoroutine(Attack());
            nextAttackTime = Time.time + attackCooldown;
        }
        else
        {
            // Move toward the player.
            float moveDir = Mathf.Sign(toPlayer.x);
            rb.velocity = new Vector2(moveDir * runSpeed, rb.velocity.y);
            anim.SetBool("isRunning", true);
            sprite.flipX = (moveDir < 0);
        }
    
        if (Mathf.Abs(rb.velocity.x) < 0.1f)
        {
            anim.SetBool("isRunning", false);
        }
    }

    void FixedUpdate()
    {
        if (isDead)
            return;
    
        // Handle random jumping.
        if (Time.time >= nextJumpTime && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            anim.SetTrigger("Jump");
            ScheduleNextJump();
        }
    }
    
    IEnumerator Attack()
    {
        isAttacking = true;
        anim.SetTrigger("Attack");
    
        float originalSpeed = runSpeed;
        runSpeed = 0f;
    
        // Wait for the attack animation (adjust delay as needed).
        yield return new WaitForSeconds(0.7f);
    
        // Check if the player is still in range.
        float distanceToPlayer = Vector2.Distance(player.position, transform.position);
        if (distanceToPlayer <= attackRange && isDead!=true)
        {
            Samuri playerScript = player.GetComponent<Samuri>();
            if (playerScript != null)
            {
                playerScript.ReceiveHit();
            }
        }
    
        runSpeed = originalSpeed;
        isAttacking = false;
    }
    
    bool IsGrounded()
    {
        if (coll == null)
            return false;
            
        Vector2 origin = new Vector2(coll.bounds.center.x, coll.bounds.min.y);
        float extraHeight = 0.1f;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, extraHeight, groundLayer);
        return hit.collider != null;
    }
    
    void ScheduleNextJump()
    {
        nextJumpTime = Time.time + Random.Range(randomJumpIntervalMin, randomJumpIntervalMax);
    }
    
    public void TakeDamage(int damage)
    {
        if (isDead)
            return;
    
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }
    
    public void Die()
    {
        if (isDead) return; // Prevent this method from running if already dead.

        isDead = true;
        rb.velocity = Vector2.zero;
        anim.SetTrigger("Die");
        Destroy(gameObject, 1f);
    }
}
