using UnityEngine;
using System.Collections.Generic;


public class LoadVancouverSewers : MonoBehaviour
{
    [System.Serializable]
    public class SewerPipe
    {
        public float diameter_mm;
        public string install_yr;
        public string material;
        public GeoPoint geo_point_2d;
    }

    [System.Serializable]
    public class GeoPoint
    {
        public double lon;
        public double lat;
    }

    [System.Serializable]
    private class SewerPipesWrapper
    {
        public List<SewerPipe> pipes;
    }

    void Start()
    {
        LoadSewerData();
    }

    public List<SewerPipe> pipes;

    private void LoadSewerData()
    {
        TextAsset file = Resources.Load<TextAsset>("sewers_van");
        if (file == null)
        {
            Debug.LogError("Failed to load JSON file.");
            return;
        }

        SewerPipesWrapper wrapper = JsonUtility.FromJson<SewerPipesWrapper>("{\"pipes\":" + file.text + "}");

        if (wrapper != null && wrapper.pipes != null)
        {
            pipes = wrapper.pipes; // Assign the loaded pipes to the public variable
            Debug.Log("Loaded sewer pipe data.");
        }
        else
        {
            Debug.LogError("Failed to parse sewer data.");
        }
    }
}