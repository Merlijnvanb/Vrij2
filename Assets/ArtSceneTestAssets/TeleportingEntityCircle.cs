using UnityEngine;
using System.Collections;

public class AutoTeleportingEntity : MonoBehaviour
{
    [Header("Arena Settings")]
    public Vector3 arenaCenter = Vector3.zero;
    public float arenaRadius = 10f;
    public float fixedY = 0f;  // The height the item stays at (e.g., floor level)

    [Header("Teleport Settings")]
    public float teleportInterval = 5f;       // Time between teleports
    public bool hideBetweenTeleports = false; // Optional: disappear before moving

    private Renderer entityRenderer;
    private Collider entityCollider;

    private void Awake()
    {
        entityRenderer = GetComponent<Renderer>();
        entityCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        StartCoroutine(TeleportRoutine());
    }

    private IEnumerator TeleportRoutine()
    {
        while (true)
        {
            if (hideBetweenTeleports)
                SetActiveState(false);

            yield return new WaitForSeconds(teleportInterval / 2);

            TeleportToRandomLocation();

            if (hideBetweenTeleports)
                SetActiveState(true);

            yield return new WaitForSeconds(teleportInterval / 2);
        }
    }

    private void TeleportToRandomLocation()
    {
        Vector2 randomPoint = Random.insideUnitCircle * arenaRadius;
        Vector3 newPosition = new Vector3(arenaCenter.x + randomPoint.x, fixedY, arenaCenter.z + randomPoint.y);
        transform.position = newPosition;
    }

    private void SetActiveState(bool state)
    {
        if (entityRenderer != null) entityRenderer.enabled = state;
        if (entityCollider != null) entityCollider.enabled = state;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Vector3 center = new Vector3(arenaCenter.x, fixedY, arenaCenter.z);
        Gizmos.DrawWireSphere(center, arenaRadius);
    }
}
