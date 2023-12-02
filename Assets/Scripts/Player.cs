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

    [SerializeField]
    private Collider2D playerCollider;

    [SerializeField]
    private Rigidbody2D body;

    void Update()
    {
        //TODO Refactoring?
        var xSpeed = Input.GetAxis(Inputs.Horizontal.ToString()) * speed;
        body.velocity = new Vector2(xSpeed, body.velocity.y);

        if (ShouldJump())
        {
            Jump();
        }
    }

    private bool ShouldJump()
    {
        var isGrounded = Physics2D.IsTouchingLayers(playerCollider, groundLayer);
        return Input.GetButtonDown(Inputs.Jump.ToString()) && isGrounded;
    }

    private void Jump()
    {
        body.AddForce(Vector3.up * 5, ForceMode2D.Impulse);
    }
}
