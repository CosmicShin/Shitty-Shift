using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class CombinationLockDoor : Door
{
    [Header("Combination Lock")]
    [Tooltip("Four digit combination to unlock the door.")]
    public string combination = "1234";

    [Header("UI")]
    public GameObject combinationPanel;
    public TextMeshProUGUI[] digitDisplays = new TextMeshProUGUI[4];
    public TextMeshProUGUI statusText;

    private const int CombinationLength = 4;
    private static readonly Key[] digitKeys = new[]
    {
        Key.Digit0, Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4,
        Key.Digit5, Key.Digit6, Key.Digit7, Key.Digit8, Key.Digit9
    };
    private static readonly Key[] numpadKeys = new[]
    {
        Key.Numpad0, Key.Numpad1, Key.Numpad2, Key.Numpad3, Key.Numpad4,
        Key.Numpad5, Key.Numpad6, Key.Numpad7, Key.Numpad8, Key.Numpad9
    };

    private char[] enteredDigits = new char[CombinationLength] { '0', '0', '0', '0' };
    private int currentSlotIndex;
    private bool panelOpen;
    private bool isUnlocked;
    private PlayerMovement currentPlayer;

    private void OnValidate()
    {
        if (combination == null)
            combination = "0000";

        combination = new string(combination.Where(char.IsDigit).ToArray());

        if (combination.Length < 4)
            combination = combination.PadRight(4, '0');
        else if (combination.Length > 4)
            combination = combination.Substring(0, 4);
    }

    private void Start()
    {
        ResetEnteredDigits();
        HidePanel();
    }

    private void Update()
    {
        if (!panelOpen || currentPlayer == null)
            return;

        ReadDigitInput();
    }

    public override string GetInteractionPrompt()
    {
        if (panelOpen)
            return "Press F to Close";

        if (isUnlocked)
            return base.GetInteractionPrompt();

        return "Press F to Enter Code";
    }

    public override void Interact(PlayerMovement player)
    {
        if (isUnlocked)
        {
            base.Interact(player);
            return;
        }

        if (panelOpen)
        {
            ClosePanel();
            return;
        }

        OpenPanel(player);
    }

    private void ReadDigitInput()
    {
        if (Keyboard.current == null)
            return;

        if (TryReadDigitKey(out char digit))
        {
            if (currentSlotIndex < enteredDigits.Length)
            {
                enteredDigits[currentSlotIndex] = digit;
                currentSlotIndex++;
                UpdateDigitDisplays();
            }

            if (currentSlotIndex >= enteredDigits.Length)
                ValidateCombination();
        }
        else if (Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            if (currentSlotIndex > 0)
            {
                currentSlotIndex--;
                enteredDigits[currentSlotIndex] = '0';
                UpdateDigitDisplays();
                UpdateStatusText("Backspace");
            }
        }
        else if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ClosePanel();
        }
    }

    private bool TryReadDigitKey(out char digit)
    {
        digit = default;

        if (Keyboard.current == null)
            return false;

        for (int i = 0; i < digitKeys.Length; i++)
        {
            if (Keyboard.current[digitKeys[i]].wasPressedThisFrame || Keyboard.current[numpadKeys[i]].wasPressedThisFrame)
            {
                digit = (char)('0' + i);
                return true;
            }
        }

        return false;
    }

    private void ValidateCombination()
    {
        string entered = new string(enteredDigits);
        PlayerMovement player = currentPlayer;

        if (entered == combination)
        {
            isUnlocked = true;
            UpdateStatusText("Unlocked!");
            ClosePanel();
            base.Interact(player);
        }
        else
        {
            UpdateStatusText("Wrong code. Try again.");
            ResetEnteredDigits();
            UpdateDigitDisplays();
        }
    }

    private void OpenPanel(PlayerMovement player)
    {
        currentPlayer = player;
        panelOpen = true;

        if (combinationPanel != null)
            combinationPanel.SetActive(true);

        ResetEnteredDigits();
        UpdateDigitDisplays();
        UpdateStatusText("Enter 4 digits. Backspace to delete.");

        if (currentPlayer != null)
            currentPlayer.SetReadingState(true);
    }

    private void ClosePanel()
    {
        panelOpen = false;

        if (combinationPanel != null)
            combinationPanel.SetActive(false);

        if (currentPlayer != null)
            currentPlayer.SetReadingState(false);

        currentPlayer = null;
    }

    private void OnDisable()
    {
        if (panelOpen)
            ClosePanel();
    }

    private void HidePanel()
    {
        if (combinationPanel != null)
            combinationPanel.SetActive(false);
    }

    private void ResetEnteredDigits()
    {
        System.Array.Fill(enteredDigits, '0');
        currentSlotIndex = 0;
    }

    private void UpdateDigitDisplays()
    {
        if (digitDisplays == null)
            return;

        for (int i = 0; i < digitDisplays.Length; i++)
        {
            if (digitDisplays[i] != null)
                digitDisplays[i].text = enteredDigits[i].ToString();
        }
    }

    private void UpdateStatusText(string text)
    {
        if (statusText != null)
            statusText.text = text;
    }
}