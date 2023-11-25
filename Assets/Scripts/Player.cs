using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5;
    public float jumpForce = 10;

    public Rigidbody2D body;

    void Update()
    {
        //TODO Refactoring?
        //TODO Delta? Investigate
        var xSpeed = Input.GetAxis("Horizontal") * speed;
        body.velocity = new Vector2(xSpeed, body.velocity.y); ;

        if (Input.GetButtonDown("Jump"))
        {
            body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
