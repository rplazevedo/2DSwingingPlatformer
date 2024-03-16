using Assets.Scripts.UnityEnums;
using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    [SerializeField] private Player player;

    private void Start()
    {
        player = Player.instance;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Tags.Player))
        {
            PlayerDeath();
        }
    }

    void PlayerDeath()
    {
        player.Reset();
    }
}
