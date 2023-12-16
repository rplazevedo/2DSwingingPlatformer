using Assets.Scripts.UnityEnums;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.Player))
        {
            Destroy(gameObject);
        }
    }
}
