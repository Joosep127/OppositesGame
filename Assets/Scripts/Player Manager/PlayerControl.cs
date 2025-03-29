using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D detectionRadius;

    public int chargeState = 0; // 0 = Uncharged, 1 = Positive, -1 = Negative
    public KeyCode chargeKey; // Assigned in Unity Inspector (E for P1, O for P2)

    private float chargeForce = 10f;
    private float bounceForce = 12f; // Adjust as needed
    private float fallSpeedBoost = 5f; // Extra gravity force when falling after release
    private bool isStuck = false; // Track if player is stuck to a surface

    private Transform stuckSurface; // Store reference to the surface when sticking
    private float radius;

    private bool canJumpWhileStuck = true; // Allow jumping while stuck if true
    
    void Start()
    {
        if (detectionRadius == null)
        {
            detectionRadius = GetComponent<Collider2D>();
        }

        if (detectionRadius != null)
        {
            detectionRadius.isTrigger = true; // Ensure it's a trigger
            UpdateDetectionRadius();
        }
    }

    void Update()
    {
        ToggleCharge();
    }

    void FixedUpdate()
    {
        ApplyChargeForces();
       
        if (isStuck && stuckSurface != null)
        {
            float distance = Vector2.Distance(transform.position, stuckSurface.position);
            if (distance > radius) 
            {
                ReleaseFromSurface();
            }
        }
    }

    void ToggleCharge()
    {
        if (Input.GetKeyDown(chargeKey))
        {
            chargeState = (chargeState == 0) ? (chargeKey == KeyCode.E ? 1 : -1) : 0;

            if (chargeState == 0 && isStuck) // If player uncharges, release them
            {
                ReleaseFromSurface();
            }
        }
    }

    void ApplyChargeForces()
    {
        if (chargeState == 0) return;

        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D obj in nearbyObjects)
        {
            if (obj.CompareTag("Ground"))
            {
                ChargedPlatform platform = obj.GetComponent<ChargedPlatform>();
                if (platform != null && platform.chargeType != 0) // Ignore uncharged platforms
                {
                    Vector2 direction = obj.transform.position - transform.position;
                    float force = (platform.chargeType == chargeState) ? -chargeForce : chargeForce;
                    rb.AddForce(direction.normalized * force, ForceMode2D.Force);
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            ChargedPlatform platform = collision.gameObject.GetComponent<ChargedPlatform>();

            if (platform != null && platform.chargeType != 0)
            {
                if (chargeState == platform.chargeType) // Similar charges -> Bounce
                {
                    Bounce();
                }
                else if (chargeState != 0 && chargeState != platform.chargeType) // Opposite charges -> Stick
                {
                    StickToSurface(collision.transform);
                }
            }
        }
    }

    void Bounce()
    {
        if (isStuck)
        {
            ReleaseFromSurface();
        }
        rb.velocity = new Vector2(rb.velocity.x, bounceForce); // Use velocity for better control
    }

    void StickToSurface(Transform surface)
    {
        if (isStuck) return; // Prevent redundant sticking

        isStuck = true;
        stuckSurface = surface;

        //rb.linearVelocity = Vector2.zero; // Stop the player's movement
        rb.gravityScale = 0f;
        transform.parent = surface; // Attach player to platform
    }

    void ReleaseFromSurface()
    {
        if (!isStuck) return; // Prevent unnecessary releases if not stuck

        isStuck = false;
        rb.gravityScale = 1f; // Reset gravity to normal
        rb.velocity = new Vector2(rb.velocity.x, -fallSpeedBoost); // Fall quickly after release
        transform.parent = null; // Detach from platform
    }
    
    void UpdateDetectionRadius()
    {
        if (detectionRadius is CircleCollider2D circle)
        {
            radius = circle.radius * Mathf.Max(transform.localScale.x, transform.localScale.y);
        }
        else if (detectionRadius is BoxCollider2D box)
        {
            radius = Mathf.Max(box.size.x, box.size.y) * 0.5f * Mathf.Max(transform.localScale.x, transform.localScale.y);
        }
    }
}
