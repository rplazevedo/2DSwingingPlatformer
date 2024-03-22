using UnityEngine;

public class LaserGun : MonoBehaviour
{
    public GameObject laserPrefab;

    private GameObject laser;
    private float lastLaserStateChangeTime;
    private float laserCooldown = 5f;

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
