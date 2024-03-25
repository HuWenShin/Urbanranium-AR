using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class SceneChange : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangetoGPSScene() {
        SceneManager.LoadScene("GPSLocation");
    }

    public void ChangetoIntroScene()
    {
        SceneManager.LoadScene("intro");
    }
}
