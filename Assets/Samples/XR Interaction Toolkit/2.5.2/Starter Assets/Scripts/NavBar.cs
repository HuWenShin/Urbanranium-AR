using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavBar : MonoBehaviour
{
    [SerializeField] GameObject[] panels;

    public void deactivatePanel(GameObject activePanel) {
        for (int i = 0; i < panels.Length; i ++) {
            panels[i].SetActive(false);
        }
        activePanel.SetActive(true);
    } 
}
