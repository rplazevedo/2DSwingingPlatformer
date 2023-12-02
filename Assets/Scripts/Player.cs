using Assets.Scripts.UnityEnums;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5;
    public float jumpForce = 10;

    public float raycastLength = 0.1f;
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    private bool isGrounded;

    [SerializeField]
    private Collider2D playerCollider;

    [SerializeField]
    private Rigidbody2D body;

    void Update()
    {
        isGrounded = Physics2D.IsTouchingLayers(playerCollider, groundLayer);//.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        //TODO Refactoring?
        var xSpeed = Input.GetAxis(Inputs.Horizontal.ToString()) * speed;
        body.velocity = new Vector2(xSpeed, body.velocity.y); ;

        //TODO No more air jumping qwq


        if (Input.GetButtonDown(Inputs.Jump.ToString()) && isGrounded)
        {

                body.AddForce(Vector3.up * 5, ForceMode2D.Impulse);
        }
    }
    bool IsGrounded()
    {
        // Cast a ray straight down to check if the player is grounded
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, raycastLength);

        // If the ray hits something, the player is grounded
        return hit.collider != null;
    }

}
