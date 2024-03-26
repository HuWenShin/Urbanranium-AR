using TMPro; // Ensure to include the TMPro namespace
using UnityEngine;
using UnityEngine.UI; // This is the missing namespace

public class ChangeTextColor : MonoBehaviour
{
    public Button yourButton; // Assign this in the inspector

    void Start()
    {
        // Optional: Call ChangeButtonTextColor here or from anywhere you need
        ChangeButtonTextColor(Color.red); // Example to change color to red
    }

    public void ChangeButtonTextColor(Color newColor)
    {
        // Get the TextMeshProUGUI component from the button and change its color
        TextMeshProUGUI buttonText = yourButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.color = newColor;
        }
    }
}
