using UnityEngine;

public class LaserGun : MonoBehaviour
{
    public GameObject laserPrefab;

    private GameObject laser;

    private void Awake()
    {
        laser = Instantiate(laserPrefab, transform.position, transform.rotation);
        laser.SetActive(false);
    }

    void Start()
    {
        FireLaser();
    }

    private void FireLaser()
    {
        laser.SetActive(true);
    }
}
