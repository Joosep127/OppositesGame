using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Tilemap chargedTilemap; // Reference to the Tilemap containing charged tiles

    public int chargeState = 0; // 0 = Uncharged, 1 = Positive, -1 = Negative
    public KeyCode chargeKey; // Assigned in Unity Inspector (E for P1, O for P2)

    private float chargeForce = 10f;
    private float detectionRadius = 1f;
    private float bounceForce = 12f; // Adjust as needed
    private bool isStuck = false; // Track if player is stuck to a surface

    private Transform stuckSurface; // Store reference to the surface when sticking

    void Update()
    {
        ToggleCharge();
    }

    void FixedUpdate()
    {
        ApplyChargeForces();
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

        Vector3Int playerCell = chargedTilemap.WorldToCell(transform.position); // Get player's current tile position

        for (int x = -2; x <= 2; x++) // Scan tiles in a small area around the player
        {
            for (int y = -2; y <= 2; y++)
            {
                Vector3Int tilePosition = new Vector3Int(playerCell.x + x, playerCell.y + y, 0);
                TileBase tile = chargedTilemap.GetTile(tilePosition);

                if (tile is Temp chargedTile) // Check if tile has charge
                {
                    float force = (chargedTile.Polarity == chargeState) ? -chargeForce : chargeForce;
                    Vector2 direction = (chargedTilemap.GetCellCenterWorld(tilePosition) - transform.position).normalized;
                    
                    rb.AddForce(direction * force, ForceMode2D.Force);
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector3Int tilePosition = chargedTilemap.WorldToCell(transform.position);
        TileBase tile = chargedTilemap.GetTile(tilePosition);

        if (tile is Temp chargedTile)
        {
            if (chargeState == chargedTile.Polarity) // Similar charges -> Bounce
            {
                Bounce();
            }
            else if (chargeState != 0 && chargeState != chargedTile.Polarity) // Opposite charges -> Stick
            {
                StickToSurface(collision.transform);
            }
        }
    }

    void Bounce()
    {
        if (isStuck)
        {
            ReleaseFromSurface();
        }
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce); // Use velocity for better control
    }

    void StickToSurface(Transform surface)
    {
        if (isStuck) return; // Prevent redundant sticking

        isStuck = true;
        stuckSurface = surface;

        rb.linearVelocity = Vector2.zero; // Stop the player's movement
        rb.gravityScale = 0f;
        //transform.parent = surface; // Attach player to platform
    }

    void ReleaseFromSurface()
    {
        if (!isStuck) return; // Prevent unnecessary releases if not stuck

        isStuck = false;
        rb.gravityScale = 1f; // Reset gravity to normal
        //transform.parent = null; // Detach from platform
    }
}
