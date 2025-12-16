using UnityEngine;

public class GhostFloatWander : MonoBehaviour
{
    public Transform player;

    [Header("Movement")]
    public float moveSpeed = 1.5f;
    public float changeTargetTime = 3f;
    public float wanderRadius = 5f;

    [Header("Floating")]
    public float floatSpeed = 2f;
    public float floatHeight = 0.5f;
    public float rotationSpeed = 30f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private float timer;

    void Start()
    {
        startPos = transform.position;
        PickNewTarget();
    }

    void Update()
    {
    
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance < 2f)
            {
                targetPos = transform.position +
                            (transform.position - player.position).normalized * 3f;
            }
        }

        timer += Time.deltaTime;
        if (timer >= changeTargetTime)
        {
            PickNewTarget();
            timer = 0;
        }

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(
            transform.position.x,
            startPos.y + yOffset,
            transform.position.z
        );

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    void PickNewTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        targetPos = startPos + new Vector3(randomCircle.x, 0, randomCircle.y);
    }
}
