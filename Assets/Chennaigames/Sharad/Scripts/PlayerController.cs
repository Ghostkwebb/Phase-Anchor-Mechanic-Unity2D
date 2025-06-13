using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Movement")]
    public float moveSpeed = 7f;
    public float jumpForce = 15f;
    private float moveInput;
    public bool isFacingRight = true;
    private bool jumpRequested = false;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 15f;
    private GameObject currentProjectile;

    [Header("Respawn")]
    public float respawnDelay = 1.5f;
    private Vector3 respawnPoint;
    private bool isDead = false;

    [Header("UI")]
    public ProjectileTimerUI timerUI;

    [Header("Camera Stuff")]
    private CameraFollowObject cameraFollowObject;
    public GameObject cameraFollowObjectGO;
    private bool isInControl = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        respawnPoint = transform.position;
        cameraFollowObject = cameraFollowObjectGO.GetComponent<CameraFollowObject>();
    }

    private void Update()
    {
        if (!isInControl) return;
        if (isDead) return;
        HandleInput();
        HandleAnimations();
        HandleTimerUI();
    }

    private void FixedUpdate()
    {
        if (!isInControl) return;
        if (isDead) return;
        HandleMovement();
        HandleJumping();
        HandleFlipping();
    }

    private void HandleInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            jumpRequested = true;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (currentProjectile == null)
            {
                Shoot();
            }
            else
            {
                Teleport();
            }
        }
    }

    private void HandleMovement()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    private void HandleJumping()
    {
        if (jumpRequested && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            anim.SetTrigger("Jump");
            jumpRequested = false;
        }
    }

    private void HandleFlipping()
    {
        if ((moveInput < 0 && isFacingRight) || (moveInput > 0 && !isFacingRight))
        {
            Flip();
        }
    }

    private void HandleAnimations()
    {
        anim.SetFloat("Speed", Mathf.Abs(moveInput));
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        float targetYRotation = isFacingRight ? 0f : -180f;
        transform.rotation = Quaternion.Euler(0f, targetYRotation, 0f);
        if(isFacingRight)
        {
            cameraFollowObject.CallTurn();
        }
        else
        {
            cameraFollowObject.CallTurn();
        }
    }

    private void Shoot()
    {
        anim.SetTrigger("Shoot");
    }

    public void FireProjectile()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector2 shootDirection = (mousePosition - transform.position).normalized;
        currentProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        currentProjectile.GetComponent<Rigidbody2D>().velocity = shootDirection * projectileSpeed;

        if (timerUI != null)
        {
            float lifetime = currentProjectile.GetComponent<Projectile>().lifetime;
            timerUI.StartTimer(lifetime);
        }
    }

    private void Teleport()
    {
        transform.position = currentProjectile.transform.position;
        Destroy(currentProjectile);
        if (timerUI != null)
        {
            timerUI.Hide();
        }
    }

    public void StartRespawnSequence()
    {
        if (!isDead)
        {
            StartCoroutine(RespawnCoroutine());
        }
    }

    private IEnumerator RespawnCoroutine()
    {
        isDead = true;
        rb.simulated = false;
        GetComponent<SpriteRenderer>().enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        // Respawn and re-enable the player
        transform.position = respawnPoint;
        GetComponent<SpriteRenderer>().enabled = true;
        rb.simulated = true;
        rb.velocity = Vector2.zero;
        isDead = false;
    }

    public void SetRespawnPoint(Vector3 newPoint)
    {
        respawnPoint = newPoint;
    }

    public void Respawn()
    {
        transform.position = respawnPoint;
        rb.velocity = Vector2.zero; // Reset momentum
    }

    private void HandleTimerUI()
    {
        // Hides the UI if the projectile was destroyed by timeout or hitting the ground.
        if (currentProjectile == null && timerUI.IsVisible())
        {
            timerUI.Hide();
        }
    }

    public void StartLevelEntrySequence(float duration)
    {
        StartCoroutine(EntryWalkCoroutine(duration));
    }

    private IEnumerator EntryWalkCoroutine(float duration)
    {
        isInControl = false; // Take control away from the player

        // Ensure the player is facing right for the walk-in
        if (!isFacingRight)
        {
            Flip();
        }

        // Force the walk animation and movement
        anim.SetFloat("Speed", 1f);
        rb.velocity = new Vector2(moveSpeed, 0);

        // Wait for the specified duration
        yield return new WaitForSeconds(duration);

        // Stop the movement and animation
        rb.velocity = Vector2.zero;
        anim.SetFloat("Speed", 0f);

        isInControl = true; // Give control back to the player
    }
    

}