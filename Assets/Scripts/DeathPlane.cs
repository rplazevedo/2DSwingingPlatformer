using Assets.Scripts.UnityEnums;
using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Tags.Player.ToString()))
        {
            PlayerDeath();
        }
    }

    void PlayerDeath()
    {
        //TODO Raised task under 2023-12-09 to decide what we do here
        Debug.Log("The player died!");
    }
}
