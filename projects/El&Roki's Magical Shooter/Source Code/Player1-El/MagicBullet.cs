using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class MagicBullet : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private bool isUpMagic;
    [SerializeField] private bool isDownMagic;
    private Vector3 targetPosition;

    private void Start()
    {
        if(isUpMagic)
        {
            targetPosition = transform.position + new Vector3(0, -15f, 0);
        }
        else if(isDownMagic)
        {
            targetPosition = transform.position + new Vector3(0, 15f, 0);
        }
        else
        {
            targetPosition = transform.position + new Vector3(15f, 0, 0);
        }
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        
        if (transform.position == targetPosition)
        {
            Destroy(gameObject);
        }
    }

    public void ChangeSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }
}
