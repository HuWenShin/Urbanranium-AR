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

    public void ChangetoAppHomeScene()
    {
        SceneManager.LoadScene("AppHome");
    }

    public void ChangetoM2_PreVideo1Scene()
    {
        SceneManager.LoadScene("M2_PreVideo1");
    }

    public void ChangetoM2_Video1Scene()
    {
        SceneManager.LoadScene("M2_Video1");
    }


    public void ChangetoM2_PreGPSScene()
    {
        SceneManager.LoadScene("M2_PreGPS");
    }

    public void ChangetoM2_GPSScene()
    {
        SceneManager.LoadScene("M2_GPSLocation");
    }


    public void ChangetoM2_SeePipeInCameraScene()
    {
        SceneManager.LoadScene("M2_SeePipeInCamera");
    }


}
