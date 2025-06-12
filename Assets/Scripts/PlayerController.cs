using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Movement")]
    public float moveSpeed = 7f;
    public float jumpForce = 15f;
    private float moveInput;
    private bool isFacingRight = true;
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

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleInput();
        HandleFlipping();
        HandleAnimations();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleJumping();
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
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    private void HandleJumping()
    {
        if (jumpRequested && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
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
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
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
        currentProjectile.GetComponent<Rigidbody2D>().linearVelocity = shootDirection * projectileSpeed;
    }

    private void Teleport()
    {
        transform.position = currentProjectile.transform.position;
        Destroy(currentProjectile);
    }
}