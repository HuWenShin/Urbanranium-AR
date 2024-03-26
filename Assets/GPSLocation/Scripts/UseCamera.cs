using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Make sure this namespace is included

public class UseCamera : MonoBehaviour
{
    //WebCamTexture webcam;
    //public RawImage rawImage; // Use RawImage instead of GameObject

    //void Start()
    //{
    //    webcam = new WebCamTexture();
    //    rawImage.texture = webcam;
    //    webcam.Play();

    //    // Correct for the camera's rotation
    //    rawImage.rectTransform.localEulerAngles = new Vector3(0, 0, -webcam.videoRotationAngle);
    //}

    private bool camAvailable;
    private WebCamTexture backCam;
    private Texture defaultBackground;

    public RawImage background;
    public AspectRatioFitter fit;

    private void Start()
    {
        defaultBackground = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices;

        if(devices.Length == 0)
        {
            Debug.Log("No camera");
            camAvailable = false;
            return;
        }

        for (int i = 0; i< devices.Length; i++){
            if (!devices[i].isFrontFacing)
            {
                backCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
            }

        }

        if (backCam == null)
        {
            Debug.Log("No back camera");
            return;
        }

        backCam.Play();
        background.texture = backCam;

        camAvailable = true;
    }

    private void Update()
    {
        if (!camAvailable)
        {
            return;
        }

        float ratio = (float)backCam.width / (float)backCam.height;
        fit.aspectRatio = ratio;

        float scaleY = backCam.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        int orient = -backCam.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
    }

}
