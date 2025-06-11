using UnityEngine;
using System.Collections;

public class AutoTeleportingEntity : MonoBehaviour
{
    [Header("Arena Settings")]
    public Vector3 arenaCenter = Vector3.zero;
    public float arenaRadius = 10f;
    public float floorLevel = 0f; 

    [Header("Teleport Settings")]
    public float teleportInterval = 5f;

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
            yield return new WaitForSeconds(teleportInterval);
            TeleportToRandomPositionInCircle();
        }
    }

    private void TeleportToRandomPositionInCircle()
    {
        Vector2 randomPos = Random.insideUnitCircle * arenaRadius;
        Vector3 newPos = new Vector3(randomPos.x + arenaCenter.x, floorLevel, randomPos.y + arenaCenter.z);
        transform.position = newPos;
    }

    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
       
        Vector3 floorCenter = new Vector3(arenaCenter.x, floorLevel, arenaCenter.z);
        Gizmos.DrawWireSphere(floorCenter, arenaRadius);

        Gizmos.color = Color.green;
        Gizmos.matrix = Matrix4x4.TRS(floorCenter, Quaternion.identity, new Vector3(arenaRadius * 2, 0.1f, arenaRadius * 2));
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = Matrix4x4.identity;
    }
}