using UnityEngine;

public class PoisonHintTrigger : MonoBehaviour
{
    [Header("Hint Text")]
    [TextArea]
    public string hintMessage = "The air feels toxic... I shouldn't stay here too long. Maybe I need to move to higher ground or the tables.";

    [Header("Settings")]
    public bool triggerOnce = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player == null)
            player = other.GetComponentInParent<PlayerMovement>();

        if (player == null)
            return;

        // If already triggered and set to once → do nothing
        if (triggerOnce && hasTriggered)
            return;

        hasTriggered = true;

        if (HintManager.Instance != null)
        {
            HintManager.Instance.ShowHint(hintMessage);
        }

        Debug.Log("Poison hint triggered.");
    }
}