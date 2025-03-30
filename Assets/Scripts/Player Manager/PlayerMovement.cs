using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerControls
{
    public KeyCode up;
    public KeyCode down;
    public KeyCode left;
    public KeyCode right;
    public KeyCode jump;

    public KeyCode toggle;

    public PlayerControls(KeyCode up, KeyCode down, KeyCode left, KeyCode right, KeyCode jump, KeyCode toggle)
    {
        this.up = up;
        this.down = down;
        this.left = left;
        this.right = right;
        this.jump = jump;
        this.toggle = toggle;
    }
}

public class PlayerMovement : MonoBehaviour
{

    public Tilemap tilemap;

    public int magnetismPower;

    private Rigidbody2D rb;
    public float acceleration = 1f;
    public float maxSpeed = 10f;
    public float maxFallSpeed = 10f;
    public float deceleration = 0.6f;

    public float accelerationAir;
    public float decelerationAir;


    public float jumpHangTimeThreshold;
    public float jumpHangAccelerationMult;
    public float jumpHangMaxSpeedMult;


    public float jumpForce = 10f;
    private float coyoteTime = 0.15f;
    public float coyoteTimeCounter = 0f;
    public bool isGrounded = false;
    private PlayerControls controls;
    private PhysicsMaterial2D physicMaterial;

    public float speedDif;
    public float movement;

    public bool chargeState = false;
    public bool playerType;
    public bool hasHeadphones;
    public bool playerEffectingEachother;

    private string platformState;

    private LineRenderer lineRenderer;
    public Gradient colorGradient;

    Color GetDesaturatedColor(Color originalColor, float saturation)
    {
        Color.RGBToHSV(originalColor, out float h, out float s, out float v);
        return Color.HSVToRGB(h, saturation, v);
    }

    Color baseColor;
    Color newColor;


    public void ApplyLineRenderer(GameObject obj)
    {

        LineRenderer lr = obj.GetComponent<LineRenderer>();
        if (lr == null)
        {
            lr = obj.AddComponent<LineRenderer>();
        }
        lr.startColor = Color.red;
        lr.endColor = Color.blue;
        lr.startWidth = 0.2f;
        lr.endWidth = 0.2f;

        lineRenderer = lr;
    }

    public void ConnectToP2(bool tem)
    {
        if (tem)
        {
            Color sarrowColor = lineRenderer.startColor;
            Color earrowColor = lineRenderer.endColor;
            sarrowColor.a = 0;
            earrowColor.a = 0;
            lineRenderer.startColor = sarrowColor;
            lineRenderer.endColor = earrowColor;
            playerEffectingEachother = false;
        }
        GameObject target = GameObject.FindWithTag("P2");
        if (target != null && lineRenderer != null)
        {
            if (target.GetComponent<PlayerMovement>().chargeState)
            {
                playerEffectingEachother = true;

                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, target.transform.position);
                float distance = Vector3.Distance(transform.position, target.transform.position);

                float t = Mathf.Clamp01(1 - (distance / 10f));

                Color sarrowColor = lineRenderer.startColor;
                Color earrowColor = lineRenderer.endColor;
                sarrowColor.a = t - 0.3f;
                earrowColor.a = t - 0.3f;

                lineRenderer.startColor = sarrowColor;
                lineRenderer.endColor = earrowColor;
            }
        }
        else
        {
            Debug.LogError("P2 not found or LineRenderer is missing!");
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (gameObject.tag == "P1")
        {
            ApplyLineRenderer(rb.gameObject);
            playerType = true;
            baseColor = Color.red;
            controls = new PlayerControls(KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D, KeyCode.Space, KeyCode.E);
        }
        else if (gameObject.tag == "P2")
        {
            playerType = false;
            baseColor = Color.blue;
            controls = new PlayerControls(KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.RightControl, KeyCode.L);
        }
        else
        {
            Debug.LogError("GameObject is connected to an object that is not a player");
        }
        newColor = GetDesaturatedColor(baseColor, 0.5f);

        physicMaterial = rb.sharedMaterial;
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("QUIT");
            Application.Quit();
        }

        if (isGrounded)// 
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        rb.AddForce(CalculateMovement() * Vector2.right, 0); // Run Left and Right

        if (Input.GetKeyDown(controls.jump) && coyoteTimeCounter > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            coyoteTimeCounter = 0;
        }

        if (Input.GetKeyDown(controls.toggle))
        {
            chargeState = !chargeState;
            //gameObject.GetComponent<Renderer>().material.saturation *= -1;
        }

        if (!chargeState)
        {
            gameObject.GetComponent<Renderer>().material.color = newColor;
        }
        else
        {
            gameObject.GetComponent<Renderer>().material.color = baseColor;
        }

        // if (chargeState)
        // {
        //     ConnectToP2(false);
        // }
        // if (playerType && !chargeState)
        // {
        //     ConnectToP2(true);
        // }

        // if (playerEffectingEachother)
        // {
        //     ApplyPlayerForces((gameObject.tag == "P1" ? GameObject.FindWithTag("P2") : GameObject.FindWithTag("P1")));
        // }
    }

    private float CalculateMovement()
    {
        int moveInput;
        if (Input.GetKey(controls.left))
        {
            moveInput = -1;
        }
        else if (Input.GetKey(controls.right))
        {
            moveInput = 1;
        }
        else
        {
            moveInput = 0;
        }

        float targetSpeed = moveInput * maxSpeed; // Credit to @DawnosaurDev in youtube
        //We can reduce are control using Lerp() this smooths changes to are direction and speed
        targetSpeed = Mathf.Lerp(rb.linearVelocity.x, targetSpeed, 1);

        float accelRate;

        //Gets an acceleration value based on if we are accelerating (includes turning) 
        //or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
        if (isGrounded)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration * accelerationAir : deceleration * decelerationAir;

        //Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural


        //We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
        if (Mathf.Abs(rb.linearVelocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rb.linearVelocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && isGrounded)
        {
            //Prevent any deceleration from happening, or in other words conserve are current momentum
            //You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
            accelRate = 0;
        }

        if ((!isGrounded) && Mathf.Abs(rb.linearVelocity.y) < jumpHangTimeThreshold)
        {
            accelRate *= jumpHangAccelerationMult;
            targetSpeed *= jumpHangMaxSpeedMult;
        }

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - rb.linearVelocity.x;
        //Calculate force along x-axis to apply to thr player

        return (speedDif * accelRate);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || playerType && collision.gameObject.CompareTag("P1") || playerType && collision.gameObject.CompareTag("P2"))
        {
            isGrounded = false;
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || playerType && collision.gameObject.CompareTag("P1") || playerType && collision.gameObject.CompareTag("P2"))
        {
            isGrounded = true;
        }

        if (chargeState && collision.gameObject.CompareTag("Magnet"))
        {
            ApplyPolarityForces(collision.gameObject);
            //DrawArrow(collision);
        }
    }

    private void ApplyPolarityForces(GameObject collision)
    {
        Vector3 worldPos = collision.transform.position;
        int force = collision.GetComponent<MagnetScript>().Polarity * (playerType ? 1 : -1);
        Vector3 direction = (worldPos - transform.position).normalized;

        rb.AddForce(direction * (force * magnetismPower / (Vector3.Distance(worldPos, transform.position) + 0.4f)));
    }

    private void ApplyPlayerForces(GameObject collision)
    {
        Vector3 worldPos = collision.transform.position;
        Vector3 direction = (worldPos - transform.position).normalized;

        rb.AddForce(direction * (magnetismPower / (Vector3.Distance(worldPos, transform.position) + 0.4f)));
    }
}
