using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeDataHolder : MonoBehaviour
{
    public static PipeDataHolder Instance { get; private set; }

    public float Diameter;
    public string YearOfInstallation;
    public string Material;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}