using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5;
    public float jumpForce = 10;

    [SerializeField]
    private Rigidbody2D body;

    void Update()
    {
        //TODO Refactoring?
        //TODO Delta? Investigate
        var xSpeed = Input.GetAxis("Horizontal") * speed;
        body.velocity = new Vector2(xSpeed, body.velocity.y); ;

        //TODO No more air jumping qwq
        if (Input.GetButtonDown("Jump"))
        {
            body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
