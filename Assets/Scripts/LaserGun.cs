using UnityEngine;

public class LaserGun : MonoBehaviour
{
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private float laserCooldown = 2f;

    private GameObject laser;
    private float lastLaserStateChangeTime;

    private void Awake()
    {
        laser = Instantiate(laserPrefab, transform.position, transform.rotation);
        laser.SetActive(false);
        lastLaserStateChangeTime = Time.time;   
    }


    private void Update()
    {
        if (Time.time > lastLaserStateChangeTime + laserCooldown)
        {
            lastLaserStateChangeTime = Time.time;
            ToggleLaser();
        }
    }

    private void ToggleLaser()
    {
        laser.SetActive(!laser.activeSelf);
    }
}
