using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private float snapDistance = 5f;

    private void Update()
    {
        if (target != null)
        {
            Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

            if ((desiredPosition - transform.position).sqrMagnitude > snapDistance)
            {
                transform.position = desiredPosition;
                return;
            }

            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
