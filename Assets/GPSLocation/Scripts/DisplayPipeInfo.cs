using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayPipeInfo : MonoBehaviour
{
    public TextMeshProUGUI pipeInfo;
    // Start is called before the first frame update
    void Start()
    {
        if (PipeDataHolder.Instance != null)
        {
            pipeInfo.text = "Diameter: " + PipeDataHolder.Instance.Diameter.ToString() + " mm\n"+"Year of Installation: " + PipeDataHolder.Instance.YearOfInstallation+ "\nMaterial: " + PipeDataHolder.Instance.Material;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
