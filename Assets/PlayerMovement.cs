using UnityEngine;

public class PlayerControls
{
    public KeyCode up;
    public KeyCode down;
    public KeyCode left;
    public KeyCode right;
    public KeyCode jump;

    public PlayerControls(KeyCode up, KeyCode down, KeyCode left, KeyCode right, KeyCode jump)
    {
        this.up = up;
        this.down = down;
        this.left = left;
        this.right = right;
        this.jump = jump;
    }
}
public class PlayerMovement : MonoBehaviour
{


    private Rigidbody2D rb;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    private PlayerControls controls;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (gameObject.tag == "P1")
        {
            controls = new PlayerControls(KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D, KeyCode.Space);
        }
        else if (gameObject.tag == "P2")
        {
            controls = new PlayerControls(KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.RightControl);
        }
        else
        {
            Debug.LogError("GameObject is connected to an object that is not a player");
        }
    }

    // Update is called once per frame
    void Update()
     {
        // Get input
        float moveX = 0f;
        if (Input.GetKey(controls.left))
            moveX = -1f;
        if (Input.GetKey(controls.right))
            moveX = 1f;

        // Apply movement
        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);

        // Jump
        if (Input.GetKeyDown(controls.jump))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }
}
