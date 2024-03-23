using System.Net;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class LaserGun : MonoBehaviour
{
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private float laserCooldown = 2f;
    public float maxLaserDistance = 100f;
    public LayerMask groundLayer;

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
        if(laser.activeSelf)
        {
            laser.SetActive(false);
            return;
        }


        laser.SetActive(true);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.right, maxLaserDistance, groundLayer);

        Vector3 endPoint;
        if (hit.collider != null)
        {
            endPoint = new Vector3(hit.point.x, hit.point.y) + transform.right * GetComponent<SpriteRenderer>().bounds.size.x / 2f;
        }
        else
        {
            endPoint = transform.position  - transform.right * maxLaserDistance;
        }
        Vector3 gunEdgePosition = transform.position - transform.right * GetComponent<SpriteRenderer>().bounds.size.x;

        laser.transform.position = (gunEdgePosition + endPoint) / 2f;


        //Vector3 direction = endPoint - transform.position;
        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //laser.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        float distance = Vector3.Distance(transform.position, endPoint);
        laser.transform.localScale = new Vector3(distance, laser.transform.localScale.y, laser.transform.localScale.y);
    }
}
