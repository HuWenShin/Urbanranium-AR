using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GPSLocation : MonoBehaviour
{
    public TextMeshProUGUI GPSStatus;
    public TextMeshProUGUI latitudeValue;
    public TextMeshProUGUI longitudeValue;
    public TextMeshProUGUI altitudeValue;
    public TextMeshProUGUI horizontalAccuracyValue;
    public TextMeshProUGUI timestampValue;

    // Variables of pipes
    public TextMeshProUGUI nearestPipeInfo; // Add this for displaying nearest pipe info
    public LoadVancouverSewers pipeLoader; // Reference to your PipeLoader script
    // Start is called before the first frame update

    // Pipe image
    public Image pipeImg;
    // Purple Municipal GIF
    public Image purpleWalkGIF;

    void Start()
    {
        //StartCoroutine(GPSloc());

        // Hide the image at the start
        if (pipeImg != null) pipeImg.enabled = false;
        // Hide GIF at the start
        if (purpleWalkGIF != null) purpleWalkGIF.enabled = false;
    }

    public void StartGPS()
    {
        StartCoroutine(GPSloc());
    }

    // Update gps data with button
    public IEnumerator GPSloc()
    {
        if (!Input.location.isEnabledByUser)
        {
            GPSStatus.text = "Location service not enabled";
            yield break;
        }

        Input.location.Start();

        // Wait until service initializes
        int maxWait = 10; // You can adjust the timeout duration as needed
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            GPSStatus.text = "Timed out";
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            GPSStatus.text = "Unable to determine device location";
            yield break;
        }
        else
        {
            // Access location data once here instead of starting continuous updates
            UpdateGPSData();
            //Input.location.Stop(); // Stop service if you no longer need it after fetching location
        }
    }
    private void UpdateStatus(string status)
    {
        if (GPSStatus != null)
        {
            GPSStatus.text = status;
        }
        else
        {
            Debug.LogError("GPSStatus TextMeshProUGUI component not set in the inspector.");
        }
    }

    private void UpdateGPSData()
    {
        // Ensure that Input.location.status is checked right after it's been initialized
        if (Input.location.status == LocationServiceStatus.Running)
        {
            UpdateTextFields(
                Input.location.lastData.latitude.ToString(),
                Input.location.lastData.longitude.ToString(),
                Input.location.lastData.altitude.ToString(),
                Input.location.lastData.horizontalAccuracy.ToString(),
                Input.location.lastData.timestamp.ToString()
            );
        }
        else
        {
            UpdateStatus("Location service stopped");
        }
    }

    private void UpdateTextFields(string latitude, string longitude, string altitude, string horizontalAccuracy, string timestamp)
    {
        if (latitudeValue != null) latitudeValue.text = latitude;
        else Debug.LogError("latitudeValue TextMeshProUGUI component not set in the inspector.");

        if (longitudeValue != null) longitudeValue.text = longitude;
        else Debug.LogError("longitudeValue TextMeshProUGUI component not set in the inspector.");

        if (altitudeValue != null) altitudeValue.text = altitude;
        else Debug.LogError("altitudeValue TextMeshProUGUI component not set in the inspector.");

        if (horizontalAccuracyValue != null) horizontalAccuracyValue.text = horizontalAccuracy;
        else Debug.LogError("horizontalAccuracyValue TextMeshProUGUI component not set in the inspector.");

        if (timestampValue != null) timestampValue.text = timestamp;
        else Debug.LogError("timestampValue TextMeshProUGUI component not set in the inspector.");
    }



    // Update is called once per frame
    void Update()
    {

    }


    // Find nearest pipe
    private LoadVancouverSewers.SewerPipe FindNearestPipe(Vector2 userLocation, List<LoadVancouverSewers.SewerPipe> pipes)
    {
        int counter = 0;
        //LoadVancouverSewers.SewerPipe nearestPipe = null;
        //float smallestDistance = float.MaxValue;

        //Debug.Log($"There are " + pipes.Count + " pipes in this area");

        //foreach (var pipe in pipes)
        //{
        //    // Assuming geo_point_2d contains the latitude and longitude
        //    Vector2 pipeLocation = new Vector2((float)pipe.geo_point_2d.lon, (float)pipe.geo_point_2d.lat);
        //    float distance = Vector2.Distance(userLocation, pipeLocation);

        //    if (distance < smallestDistance)
        //    {
        //        smallestDistance = distance;
        //        nearestPipe = pipe;
        //        Debug.Log($"Nearest pipe changed with smallest distance" + distance);
        //    }
        //}

        LoadVancouverSewers.SewerPipe nearestPipe = null;
        float smallestDifferenceSum = float.MaxValue; // Initialize with max value

        foreach (var pipe in pipes)
        {
            Vector2 pipeLocation = new Vector2((float)pipe.geo_point_2d.lon, (float)pipe.geo_point_2d.lat);
            // Calculate the sum of the absolute differences in lat and lon
            float differenceSum = Mathf.Abs(userLocation.x - pipeLocation.y) + Mathf.Abs(userLocation.y - pipeLocation.x);

            if (differenceSum < smallestDifferenceSum)
            {
                Debug.Log("User location: " + userLocation.x + " " + userLocation.y);
                Debug.Log("Pipe location: " + pipeLocation.x + " " + pipeLocation.y);

                smallestDifferenceSum = differenceSum;
                nearestPipe = pipe; // Update nearest pipe

                Debug.Log($"Nearest pipe updated to location " + nearestPipe.geo_point_2d + " with distance of " + smallestDifferenceSum);
            }
            counter += 1;
        }

        Debug.Log($"Looped " + counter + " times");

        return nearestPipe;
    }

    // Find target pipe
    // Target pipe location
    private Vector2 targetPipeLocation = new Vector2(49.266885f, -123.089863f);
    // The maximum allowed difference sum to consider the pipe "within range"
    private float maxDifferenceSum = 0.01f; // Adjust this value based on your needs
    private bool targetPipeFound = false;

    private bool findTargetPipe(Vector2 userLocation)
    {
        // Calculate the sum of absolute differences in latitude and longitude
        float differenceSum = Mathf.Abs(userLocation.x - targetPipeLocation.x) + Mathf.Abs(userLocation.y - targetPipeLocation.y);

        // Check if the difference sum is within the allowed maximum
        if (differenceSum <= maxDifferenceSum)
        {
            targetPipeFound = true;
        }
        return targetPipeFound;
    }



    // Button click method
    public void OnUpdateLocationClicked()
    {
        //StartCoroutine(UpdateLocationAndFindNearestPipe());
        StartCoroutine(UpdateLocationAndFindtargetPipe());
    }

    private IEnumerator UpdateLocationAndFindtargetPipe()
    {
        // Start updating location
        yield return StartCoroutine(GPSloc());

        // Proceed if location status is running and pipes are available
        if (Input.location.status == LocationServiceStatus.Running)
        {
            Vector2 userLocation = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
            LoadVancouverSewers.SewerPipe nearestPipe = FindNearestPipe(targetPipeLocation, pipeLoader.pipes);

            if (nearestPipe != null)
            {
                if (findTargetPipe(userLocation) == true)
                {

                    // Display the nearest pipe information
                    nearestPipeInfo.text = "Found target pipe:\n" + $"Target Pipe Location: {nearestPipe.geo_point_2d.lat}, {nearestPipe.geo_point_2d.lon}\n"
                        + $"Diameter: {nearestPipe.diameter_mm} mm, " +
                                           $"Material: {nearestPipe.material}, " +
                                           $"Installed: {nearestPipe.install_yr}";

                    // Show hidden pipe image
                    if (pipeImg != null) pipeImg.enabled = true;
                    // Show hidden GIF
                    if (purpleWalkGIF != null) purpleWalkGIF.enabled = true;
                    //Debug.Log($"Nearest Pipe Info: Diameter {nearestPipe.diameter_mm} mm, Material: {nearestPipe.material}, Installed: {nearestPipe.install_yr}");

                }
            }
            else
            {
                Debug.LogError("No nearest pipe found.");
            }
        }
        else
        {
            Debug.LogError("Location service is not running.");
        }
    }

    private IEnumerator UpdateLocationAndFindNearestPipe()
    {
        // Start updating location
        yield return StartCoroutine(GPSloc());

        // Check if the pipeLoader reference is null
        if (pipeLoader == null)
        {
            Debug.LogError("pipeLoader is null.");
            yield break; // Exit if pipeLoader is not assigned.
        }

        // Check if the pipes list is null
        if (pipeLoader.pipes == null)
        {
            Debug.LogError("pipeLoader.pipes is null.");
            yield break; // Exit if the pipes list is not initialized.
        }

        // Check if the pipes list is empty
        if (pipeLoader.pipes.Count == 0)
        {
            Debug.LogError("pipeLoader.pipes is empty.");
            // You can choose to yield break here as well if there's no point in continuing without pipes.
        }

        // Proceed if location status is running and pipes are available
        if (Input.location.status == LocationServiceStatus.Running)
        {
            Vector2 userLocation = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
            LoadVancouverSewers.SewerPipe nearestPipe = FindNearestPipe(userLocation, pipeLoader.pipes);

            if (nearestPipe != null)
            {
                // Display the nearest pipe information
                nearestPipeInfo.text = "Finding nearest pipe:\n" + $"Nearest Pipe Location: {nearestPipe.geo_point_2d.lat}, {nearestPipe.geo_point_2d.lon}"
                    + $"Diameter: {nearestPipe.diameter_mm} mm, " +
                                       $"Material: {nearestPipe.material}, " +
                                       $"Installed: {nearestPipe.install_yr}";

                // Show hidden pipe image
                if (pipeImg != null) pipeImg.enabled = true;
                // Show hidden GIF
                if (purpleWalkGIF != null) purpleWalkGIF.enabled = true;
                //Debug.Log($"Nearest Pipe Info: Diameter {nearestPipe.diameter_mm} mm, Material: {nearestPipe.material}, Installed: {nearestPipe.install_yr}");
            }
            else
            {
                Debug.LogError("No nearest pipe found.");
            }
        }
        else
        {
            Debug.LogError("Location service is not running.");
        }
    }






    // update gps data continously
    //IEnumerator GPSloc()
    //{
    //    if (!Input.location.isEnabledByUser)
    //    {
    //        print("Location service not enabled");
    //        yield break;
    //    }


    //    Input.location.Start();

    //    int maxWait = 5;
    //    while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
    //    {
    //        yield return new WaitForSeconds(1);
    //        maxWait--;
    //    }

    //    if (maxWait < 1)
    //    {
    //        GPSStatus.text = "Time out";
    //        print("Waited too long");
    //        yield break;
    //    }

    //    if (Input.location.status == LocationServiceStatus.Failed)
    //    {
    //        GPSStatus.text = "Unable to determine device location";
    //        print("Connection failed");
    //        yield break;

    //    }
    //    else
    //    {
    //        GPSStatus.text = "Running";
    //        InvokeRepeating("UpdateGPSData", 0.5f, 1f);
    //    }
    //}

    //private void UpdateGPSData()
    //{
    //    if (Input.location.status == LocationServiceStatus.Running)
    //    {
    //        GPSStatus.text = "Running";
    //        latitudeValue.text = Input.location.lastData.latitude.ToString();
    //        longitudeValue.text = Input.location.lastData.longitude.ToString();
    //        altitudeValue.text = Input.location.lastData.altitude.ToString();
    //        horizontalAccuracyValue.text = Input.location.lastData.horizontalAccuracy.ToString();
    //        timestampValue.text = Input.location.lastData.timestamp.ToString();

    //    }
    //    else
    //    {
    //        GPSStatus.text = "Stop";
    //    }
    //}

}
