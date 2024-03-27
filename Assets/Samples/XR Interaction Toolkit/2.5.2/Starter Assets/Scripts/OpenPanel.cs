using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPanel : MonoBehaviour
{
    public GameObject panelA;
    public GameObject panelB;
    public GameObject panelC;

    void Start()
    {
        // Ensure this GameObject persists between scene changes
        DontDestroyOnLoad(gameObject);

        string panelToOpen = PlayerPrefs.GetString("PanelToOpen", "");
        PlayerPrefs.DeleteKey("PanelToOpen"); // Clear the flag

        // Open the panel based on the flag
        switch (panelToOpen)
        {
            case "PanelA":
                panelA.SetActive(true);
                break;
            case "PanelB":
                panelB.SetActive(true);
                break;
            case "PanelC":
                panelC.SetActive(true);
                break;
        }
    }
}
