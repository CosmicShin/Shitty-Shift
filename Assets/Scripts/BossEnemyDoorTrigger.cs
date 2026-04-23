using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BossEnemyDoorTrigger : MonoBehaviour
{
    [Header("Door")]
    [Tooltip("The door to open. Auto-detected from parent Door if left empty.")]
    public Door door;

    [Header("Settings")]
    [Tooltip("If true, the door only opens once and the trigger disables itself after.")]
    public bool openOnce = false;

    private void Awake()
    {
        if (door == null)
            door = GetComponentInParent<Door>();

        if (door == null)
            Debug.LogWarning($"BossEnemyDoorTrigger on {name} could not find a Door component.", this);

        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (door == null)
            return;

        if (other.GetComponent<BossEnemyPatrol>() == null)
            return;

        if (!door.IsOpen)
            door.Interact(null);

        if (openOnce)
            enabled = false;
    }
}