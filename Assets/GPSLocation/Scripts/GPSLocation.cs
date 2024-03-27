using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class GPSLocation : MonoBehaviour
{
    //public TextMeshProUGUI GPSStatus;
    // public TextMeshProUGUI latitudeValue;
    // public TextMeshProUGUI longitudeValue;
    // public TextMeshProUGUI userLocationAddress;



    // Variables of pipes
    // public TextMeshProUGUI nearestPipeInfo; // Add this for displaying nearest pipe info
    public TextMeshProUGUI nearestPipeAddress;
    public LoadVancouverSewers pipeLoader; // Reference to your PipeLoader script
    // Start is called before the first frame update

    public TextMeshProUGUI distanceToPipeText; // Assign in the Inspector
    public TextMeshProUGUI reachedPipe; // Assign in the Inspector

    private LoadVancouverSewers.SewerPipe targetPipe = null; // This will store the nearest pipe found as the target
    private bool isWithinRange = false; // True if within 50 meters of the target pipe

    // Button to go to the camera scene
    public Button goToCameraSceneButton;

    // Button to see the pipe location in the map
    public Button seePipeLocationButton;


    // Target distance of the target pipe
    public int targetDistance = 5000;



    // Pipe image
    // public Image pipeImg;
    // Purple Municipal GIF
    // public Image purpleWalkGIF;

    void Start()
    {
        //StartCoroutine(GPSloc());

        // Hide the image at the start
        // if (pipeImg != null) pipeImg.enabled = false;
        // Hide GIF at the start
        // if (purpleWalkGIF != null) purpleWalkGIF.enabled = false;

        if (goToCameraSceneButton != null) goToCameraSceneButton.gameObject.SetActive(false);

        // Add click listener for the button
        if (seePipeLocationButton != null)
            seePipeLocationButton.onClick.AddListener(OnSeePipeLocationClicked);
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
            //GPSStatus.text = "Location service not enabled";
            Debug.Log("Location service not enabled");
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
            //GPSStatus.text = "Timed out";
            Debug.Log("Timed out");

            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            //GPSStatus.text = "Unable to determine device location";
            Debug.Log("Unable to determine device location");

            yield break;
        }
        else
        {
            // Access location data once here instead of starting continuous updates
            UpdateGPSData();
            //Input.location.Stop(); // Stop service if you no longer need it after fetching location
        }
    }
    //private void UpdateStatus(string status)
    //{
    //    if (GPSStatus != null)
    //    {
    //        GPSStatus.text = status;
    //    }
    //    else
    //    {
    //        Debug.LogError("GPSStatus TextMeshProUGUI component not set in the inspector.");
    //    }
    //}

    private void UpdateGPSData()
    {
        // Ensure that Input.location.status is checked right after it's been initialized
        if (Input.location.status == LocationServiceStatus.Running)
        {
            // UpdateTextFields(
            //     Input.location.lastData.latitude.ToString(),
            //     Input.location.lastData.longitude.ToString(),
            //     Input.location.lastData.altitude.ToString(),
            //     Input.location.lastData.horizontalAccuracy.ToString(),
            //     Input.location.lastData.timestamp.ToString()
            // );

            // Do nothing
            Debug.Log("Location service running");
        }
        else
        {
            //UpdateStatus("Location service stopped");
            Debug.Log("Location service stopped");
        }
    }

    // private void UpdateTextFields(string latitude, string longitude, string altitude, string horizontalAccuracy, string timestamp)
    // {
    //     if (latitudeValue != null) latitudeValue.text = latitude;
    //     else Debug.LogError("latitudeValue TextMeshProUGUI component not set in the inspector.");

    //     if (longitudeValue != null) longitudeValue.text = longitude;
    //     else Debug.LogError("longitudeValue TextMeshProUGUI component not set in the inspector.");
    // }



    // Update is called once per frame
    void Update()
    {

    }


    // Find nearest pipe
    private LoadVancouverSewers.SewerPipe FindNearestPipe(Vector2 userLocation, List<LoadVancouverSewers.SewerPipe> pipes)
    {
        //int counter = 0;
        LoadVancouverSewers.SewerPipe nearestPipe = null;
        float smallestDistance = float.MaxValue;

        Debug.Log($"There are " + pipes.Count + " pipes in this area");

        foreach (var pipe in pipes)
        {
            // Assuming geo_point_2d contains the latitude and longitude
            Vector2 pipeLocation = new Vector2((float)pipe.geo_point_2d.lat, (float)pipe.geo_point_2d.lon);
            float distance = Vector2.Distance(userLocation, pipeLocation);

            if (distance < smallestDistance)
            {
                smallestDistance = distance;
                nearestPipe = pipe;
                //Debug.Log($"Nearest pipe changed with smallest distance" + distance);
            }
        }

        targetPipe = nearestPipe;

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
        StartCoroutine(UpdateLocationAndFindNearestPipe());
        //StartCoroutine(UpdateLocationAndFindtargetPipe());
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
                    // nearestPipeInfo.text = "Found a pipe around you:\n" + $"Target Pipe Location: {nearestPipe.geo_point_2d.lat}, {nearestPipe.geo_point_2d.lon}\n"
                    //     + $"Diameter: {nearestPipe.diameter_mm} mm, " +
                    //                        $"Material: {nearestPipe.material}, " +
                    //                        $"Installed: {nearestPipe.install_yr}";

                    // nearestPipeInfo.text = "Found a pipe around you!\n";

                    // Show hidden pipe image
                    // if (pipeImg != null) pipeImg.enabled = true;
                    // Show hidden GIF
                    // if (purpleWalkGIF != null) purpleWalkGIF.enabled = true;
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

            float userLat = Input.location.lastData.latitude;
            float userLon = Input.location.lastData.longitude;


            StartCoroutine(GetAddressFromCoordinates(userLat, userLon, address =>
            {
                // userLocationAddress.text = $"Your Address: {address}";
                Debug.Log($"Your Address: {address}");
            }));


            if (nearestPipe != null)
            {
                // Set the nearest pipe information in PipeDataHolder
                PipeDataHolder.Instance.Diameter = nearestPipe.diameter_mm;
                PipeDataHolder.Instance.YearOfInstallation = nearestPipe.install_yr;
                PipeDataHolder.Instance.Material = nearestPipe.material;

                Vector2 targetLocation = new Vector2((float)targetPipe.geo_point_2d.lat, (float)targetPipe.geo_point_2d.lon);
                float distance = Vector2.Distance(userLocation, targetLocation) * 111000; // Convert degrees to meters approximately
                distanceToPipeText.text = $"Distance to Target Pipe: {distance} meters";
                isWithinRange = distance <= targetDistance;

                if (isWithinRange)
                {
                    reachedPipe.text = "\nNice! You have reached the pipe! Clik on the top right button to see the pipe and Municipal using your camera!";
                    if (goToCameraSceneButton != null) goToCameraSceneButton.gameObject.SetActive(true); // Show the button
                }
                else
                {
                    reachedPipe.text = "\nYou are not close enough to the pipe, try to get closer. Click on the button below to refresh your location.";
                    if (goToCameraSceneButton != null) goToCameraSceneButton.gameObject.SetActive(false); // Hide the button
                }

                distanceToPipeText.text = $"Distance to Target Pipe: {distance} meters";

                Vector2 pipeLocation = new Vector2((float)nearestPipe.geo_point_2d.lat, (float)nearestPipe.geo_point_2d.lon);
                StartCoroutine(GetAddressFromCoordinates(pipeLocation.x, pipeLocation.y, address =>
                {
                    nearestPipeAddress.text = $"The pipe is at: {address}";
                }));


                // Display the nearest pipe information
                // nearestPipeInfo.text = "Found a pipe around you!\n";

                // Show hidden pipe image
                // if (pipeImg != null) pipeImg.enabled = true;
                // Show hidden GIF
                // if (purpleWalkGIF != null) purpleWalkGIF.enabled = true;

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


    private void UpdateDistanceAndCheckProximity(Vector2 userLocation)
    {
        if (targetPipe != null)
        {
            Vector2 targetLocation = new Vector2((float)targetPipe.geo_point_2d.lat, (float)targetPipe.geo_point_2d.lon);
            float distance = Vector2.Distance(userLocation, targetLocation) * 111000; // Convert degrees to meters approximately

            distanceToPipeText.text = $"Distance to Target Pipe: {distance} meters";
            isWithinRange = distance <= targetDistance;

            if (isWithinRange)
            {
                reachedPipe.text = "\nNice! You have reached the pipe! Clik on the Next button to see the pipe and Municipal using your camera!";
                if (goToCameraSceneButton != null) goToCameraSceneButton.gameObject.SetActive(true); // Show the button
            }
            else
            {
                reachedPipe.text = "\nNot there yet, try to get closer. Click on the button below to refresh your location. Can't find the pipe? Use the map button to see the pipe location.";
                if (goToCameraSceneButton != null) goToCameraSceneButton.gameObject.SetActive(false); // Hide the button
            }
        }
    }


    public void OnRefreshLocationClicked()
    {
        StartCoroutine(RefreshLocationAndDistance());
    }

    private IEnumerator RefreshLocationAndDistance()
    {
        yield return StartCoroutine(GPSloc());

        if (Input.location.status == LocationServiceStatus.Running)
        {
            Vector2 userLocation = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
            UpdateDistanceAndCheckProximity(userLocation); // Call this to update the distance to the target pipe
        }
    }

    //IEnumerator GetAddressFromCoordinates(float lat, float lon)
    //{
    //    string url = $"https://nominatim.openstreetmap.org/reverse?format=json&lat={lat}&lon={lon}&zoom=18&addressdetails=1";

    //    using (UnityWebRequest request = UnityWebRequest.Get(url))
    //    {
    //        request.SetRequestHeader("User-Agent", "Your Unity Application");
    //        yield return request.SendWebRequest();

    //        if (request.result != UnityWebRequest.Result.Success)
    //        {
    //            Debug.LogError("Reverse geocoding failed: " + request.error);
    //            userLocationAddress.text = "Failed to get address.";
    //        }
    //        else
    //        {
    //            // Successfully received the response
    //            string responseText = request.downloadHandler.text;
    //            Debug.Log("Geocoding response: " + responseText);

    //            // Simple parsing for demonstration purposes, assuming "display_name" is in the response
    //            string addressMarker = "\"display_name\":\"";
    //            int startIndex = responseText.IndexOf(addressMarker) + addressMarker.Length;
    //            int endIndex = responseText.IndexOf("\"", startIndex);
    //            string address = responseText.Substring(startIndex, endIndex - startIndex);

    //            // Update the UI with the address
    //            userLocationAddress.text = $"Address: {address}";
    //        }
    //    }
    //}

    IEnumerator GetAddressFromCoordinates(float lat, float lon, Action<string> callback)
    {
        string url = $"https://nominatim.openstreetmap.org/reverse?format=json&lat={lat}&lon={lon}&zoom=18&addressdetails=1";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("User-Agent", "Your Unity Application");
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Reverse geocoding failed: " + request.error);
                callback?.Invoke("Failed to get address.");
            }
            else
            {
                string responseText = request.downloadHandler.text;
                string addressMarker = "\"display_name\":\"";
                int startIndex = responseText.IndexOf(addressMarker) + addressMarker.Length;
                int endIndex = responseText.IndexOf("\"", startIndex);
                string address = responseText.Substring(startIndex, endIndex - startIndex);

                callback?.Invoke(address);
            }
        }
    }


    // Method to handle the button click
    void OnSeePipeLocationClicked()
    {
        if (targetPipe != null)
        {
            // Construct the Google Maps URL with the pipe's location
            string url = $"https://www.google.com/maps/search/?api=1&query={targetPipe.geo_point_2d.lat},{targetPipe.geo_point_2d.lon}";

            // Open the URL in the default web browser
            Application.OpenURL(url);
        }
        else
        {
            Debug.Log("No target pipe found. Please find a pipe first.");
        }
    }
}
