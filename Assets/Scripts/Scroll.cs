using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scroll : MonoBehaviour
{
    [SerializeField] private Vector2 limits;
    [SerializeField] private List <RectTransform> Items;
    [SerializeField] private RectTransform parent;
    private float itemDistance;
    [SerializeField] private TextMeshProUGUI nameText;

    private void Start(){
        itemDistance = (limits.y - limits.x) / Items.Count;
    }

    // Update is called once per frame
    void Update()
    {

        float yPosition = parent.localPosition.y;

        int index = Mathf.FloorToInt((yPosition - limits.x) / itemDistance);
        // print($"{yPosition}. Index is {index}");

        if(index > -1 && index < Items.Count)
        {
            nameText.text = Items[index].GetComponent<Module>().ModuleName;
        }
    }
}
