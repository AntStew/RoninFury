using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Samuri : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    
    [Header("Ground Check Settings")]
    public LayerMask groundLayer;
    
    [Header("Attack Settings")]
    public Transform attackPoint;       // Child transform representing the attack hitbox position.
    public float attackRadius = 0.5f;     // Radius of the attack hitbox.
    public LayerMask enemyLayer;        // Layer(s) containing enemies.
    
    [Header("Player Settings")]
    public int lives = 3;               // Player starts with 3 lives.
    
    [Header("Fade Settings")]
    public CanvasGroup fadeCanvasGroup; // CanvasGroup on a full-screen black Image for fading.

    [Header("Block Settings")]
    public float blockDuration = 1f;    // How long the block lasts.
    
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sprite;
    private Collider2D coll;
    
    // For flipping the attackPoint along with the sprite.
    private Vector3 initialAttackPointLocalPos;
    
    // Tracks if a double jump is available.
    private bool canDoubleJump = false;

    // Tracks whether the player is blocking (invincible).
    private bool isBlocking = false;
    
    // Tracks whether the character is dead.
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<Collider2D>();  // Get the player's collider.
        
        // Store the initial local position of the attackPoint.
        if (attackPoint != null)
        {
            initialAttackPointLocalPos = attackPoint.localPosition;
        }
    }

    void FixedUpdate()
    {
        // Movement code remains the same.
        float horizontal = 0f;
        if (Input.GetKey(KeyCode.A))
        {
            horizontal = -1f;
            sprite.flipX = true;  // Face left.
        }
        else if (Input.GetKey(KeyCode.D))
        {
            horizontal = 1f;
            sprite.flipX = false; // Face right.
        }
        rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
    }

    void Update()
    {
        // Update the attackPoint's local position based on the sprite's flip.
        if (attackPoint != null)
        {
            if (sprite.flipX)
            {
                attackPoint.localPosition = new Vector3(-Mathf.Abs(initialAttackPointLocalPos.x),
                                                          initialAttackPointLocalPos.y,
                                                          initialAttackPointLocalPos.z);
            }
            else
            {
                attackPoint.localPosition = new Vector3(Mathf.Abs(initialAttackPointLocalPos.x),
                                                          initialAttackPointLocalPos.y,
                                                          initialAttackPointLocalPos.z);
            }
        }
        
        // Check if the player's collider is touching the ground layer.
        bool isGrounded = coll.IsTouchingLayers(groundLayer);
        if (isGrounded)
        {
            canDoubleJump = true;
        }
        
        // Jump input: Press W.
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                animator.SetTrigger("Jump");
            }
            else if (canDoubleJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                animator.SetTrigger("Jump");
                canDoubleJump = false;
            }
        }
        
        // Attack input: Press Space.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Randomly select one of three attack animations.
            int attackChoice = Random.Range(0, 3);
            if (attackChoice == 0)
                animator.SetTrigger("Attack1");
            else if (attackChoice == 1)
                animator.SetTrigger("Attack2");
            else
                animator.SetTrigger("Attack3");

            // Check for enemies in the attack hitbox.
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayer);
            foreach (Collider2D enemy in hitEnemies)
            {
                EnemyController enemyController = enemy.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    enemyController.Die();
                    // Add 100 points to the score for each enemy killed.
                    if (ScoreManager.instance != null)
                    {
                        ScoreManager.instance.AddScore(100);
                    }
                }
            }
        }

        // Block input: Press Shift.
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            // Only allow blocking if not already blocking.
            if (!isBlocking)
            {
                StartCoroutine(Block());
            }
        }
        
        float currentSpeed = Mathf.Abs(rb.velocity.x);
        animator.SetFloat("Speed", currentSpeed);
        
        // Flip the sprite based on movement.
        if (rb.velocity.x < 0)
            sprite.flipX = true;
        else if (rb.velocity.x > 0)
            sprite.flipX = false;
    }
    
    // This method is called by enemy attacks.
    public void ReceiveHit()
    {
        // If the player is blocking, ignore the hit.
        if (isBlocking)
        {
            Debug.Log("Blocked!");
            return;
        }
        
        lives--;
        animator.SetTrigger("Hit");
        Debug.Log("Player hit! Lives remaining: " + lives);
        if (lives <= 0)
        {
            // Switch the animator to unscaled time so death animation plays.
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            animator.SetTrigger("Die");
            Time.timeScale = 0;
            StartCoroutine(ZoomCameraFadeAndGameOver());
        }
    }
    
    private IEnumerator Block()
    {
        isBlocking = true;
        // Trigger the block animation.
        animator.SetTrigger("Block");
        // Optionally, you can add logic here to prevent movement or change speed if needed.
        Debug.Log("Blocking activated! Character is invincible from the front.");

        // Wait for the block duration.
        yield return new WaitForSeconds(blockDuration);

        isBlocking = false;
        Debug.Log("Blocking ended.");
    }
    
    private IEnumerator ZoomCameraFadeAndGameOver()
    {
        // Get the main camera.
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogError("Main camera not found!");
            yield break;
        }
        
        // Smoothly zoom the camera to the player's position.
        float initialSize = mainCam.orthographicSize;
        float targetSize = initialSize / 2f;
        float duration = 3f;
        float elapsed = 0f;
        Vector3 initialCamPos = mainCam.transform.position;
        Vector3 targetCamPos = transform.position;
        targetCamPos.z = initialCamPos.z;
        
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            mainCam.orthographicSize = Mathf.Lerp(initialSize, targetSize, t);
            mainCam.transform.position = Vector3.Lerp(initialCamPos, targetCamPos, t);
            yield return null;
        }
        
        // Fade to black using the fadeCanvasGroup.
        if (fadeCanvasGroup != null)
        {
            float fadeDuration = 3f;
            float fadeElapsed = 0f;
            fadeCanvasGroup.alpha = 0f;
            while (fadeElapsed < fadeDuration)
            {
                fadeElapsed += Time.unscaledDeltaTime;
                fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, fadeElapsed / fadeDuration);
                yield return null;
            }
        }
        SceneManager.LoadScene("GameOver");
    }
    
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
            
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
