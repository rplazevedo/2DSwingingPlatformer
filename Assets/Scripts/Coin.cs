using Assets.Scripts.UnityEnums;
using System;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private float floatStrength = 0.5f;
    [SerializeField] private int scoreToAdd = 1;

    private float originalYPosition;

    void Start()
    {
        originalYPosition = transform.position.y;
    }

    private void Update()
    {
        var yOffset = (float) Math.Sin(Time.time) * floatStrength;
        transform.position = new Vector2(transform.position.x, originalYPosition + yOffset);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.Player))
        {
            Destroy(gameObject);
            GameManager.instance.AddScore(scoreToAdd);
        }
    }
}
