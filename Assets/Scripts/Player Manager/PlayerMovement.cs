using UnityEngine;

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

    private string platformState;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (gameObject.tag == "P1")
        {
            playerType = true;
            controls = new PlayerControls(KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D, KeyCode.Space, KeyCode.E);
        }
        else if (gameObject.tag == "P2")
        {
            playerType = false;
            controls = new PlayerControls(KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.RightControl, KeyCode.L);
        }
        else
        {
            Debug.LogError("GameObject is connected to an object that is not a player");
        }

        physicMaterial = rb.sharedMaterial;

    }

    // Update is called once per frame
    void Update()
    {
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
        }

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
    public void OnTriggerEnter2D(Collider2D other)
    {
        
    }


    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6 && chargeState)
        {

            platformState = collision.gameObject.tag;


            Vector2 direction = collision.gameObject.transform.position - rb.transform.position;
            int force = ((platformState == "Minus" ? true : false) == chargeState) ? -1 : 1;
            rb.AddForce(direction.normalized * force);

        }
    }


}
