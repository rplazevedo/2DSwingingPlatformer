using Assets.Scripts.UnityEnums;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5;
    [SerializeField] private float jumpForce = 10;

    [Header("Collision")]
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private LayerMask groundLayer;

    void Update()
    {
        Move();
    }

    private void Move()
    {
        HorizontalMovement();

        if (ShouldJump())
        {
            Jump();
        }
    }

    private void HorizontalMovement()
    {
        var xSpeed = Input.GetAxis(Inputs.Horizontal.ToString()) * speed;
        body.velocity = new Vector2(xSpeed, body.velocity.y);
    }

    private bool ShouldJump()
    {

        var currentPosition = transform.position;
        var targetPoint = new Vector3(currentPosition.x, currentPosition.y - 0.15f, currentPosition.z); 
        var collisionHit = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0f, Vector2.down, 0.1f, groundLayer);



        var isGrounded = collisionHit.collider != null;
        //if(collisionHit.Length < 1)
        //{
        //    return false;
        //}

        //var ground = hits[0];

        //}

        return Input.GetButtonDown(Inputs.Jump.ToString()) && isGrounded;
    }

    private void Jump()
    {
        body.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
    }
}
