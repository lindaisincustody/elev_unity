using UnityEngine;

public class GunPositioner : MonoBehaviour
{
    public Transform player;
    public float rotationSpeed = 10f;
    public float offsetDistance = 1f;

    void Update()
    {
        if (player == null)
        {
            Debug.LogError("Player reference not set in GunController script!");
            return;
        }

        Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        transform.position = player.position + (rotation * Vector3.right * offsetDistance);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

        ImageFlipCheck(direction.x);
    }

    private void ImageFlipCheck(float x)
    {
        if (x < 0)
        {
            transform.localScale = new Vector3(1, -1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
