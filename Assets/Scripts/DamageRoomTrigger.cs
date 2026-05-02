using UnityEngine;

public class DamageRoomTrigger : MonoBehaviour
{
    [Header("Damage Settings")]
    public float startingDamagePerSecond = 2f;
    public float maxDamagePerSecond = 25f;

    [Header("Damage Scaling")]
    public float damageIncreaseRate = 3f;
    public float scalerMultiplier = 1.25f;

    [Header("Reset")]
    public bool resetWhenPlayerLeaves = true;

    private PlayerMovement player;
    private float currentDamagePerSecond;
    private float timeInside;
    private bool playerInside;

    private void Start()
    {
        currentDamagePerSecond = startingDamagePerSecond;
    }

    private void Update()
    {
        if (!playerInside || player == null)
            return;

        timeInside += Time.deltaTime;

        currentDamagePerSecond = startingDamagePerSecond + 
            (timeInside * damageIncreaseRate * scalerMultiplier);

        currentDamagePerSecond = Mathf.Clamp(
            currentDamagePerSecond,
            startingDamagePerSecond,
            maxDamagePerSecond
        );

        player.TakeDamage(currentDamagePerSecond * Time.deltaTime);

        Debug.Log("Damage Room: " + currentDamagePerSecond.ToString("F1") + " DPS");
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement detectedPlayer = other.GetComponent<PlayerMovement>();

        if (detectedPlayer == null)
            detectedPlayer = other.GetComponentInParent<PlayerMovement>();

        if (detectedPlayer == null)
            return;

        player = detectedPlayer;
        playerInside = true;

        if (resetWhenPlayerLeaves)
        {
            timeInside = 0f;
            currentDamagePerSecond = startingDamagePerSecond;
        }

        Debug.Log("Player entered damage room.");
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerMovement detectedPlayer = other.GetComponent<PlayerMovement>();

        if (detectedPlayer == null)
            detectedPlayer = other.GetComponentInParent<PlayerMovement>();

        if (detectedPlayer != player)
            return;

        playerInside = false;
        player = null;

        if (resetWhenPlayerLeaves)
        {
            timeInside = 0f;
            currentDamagePerSecond = startingDamagePerSecond;
        }

        Debug.Log("Player left damage room.");
    }
}