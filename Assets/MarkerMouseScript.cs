using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarkerMouseScript : MonoBehaviour
{
    public GameObject thoughtPanel;

    private Toggle thoughtPanelsHoverToggle;
    private bool showPanelOnHover = false;

    // Start is called before the first frame update
    void Start()
    {
        thoughtPanelsHoverToggle = GameObject.Find("ThoughtPanelsHoverToggle").GetComponent<Toggle>();
        thoughtPanelsHoverToggle.onValueChanged.AddListener(delegate { ThoughtPanelsHoverToggleChanged(); });
        showPanelOnHover = thoughtPanelsHoverToggle.isOn;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ThoughtPanelsHoverToggleChanged()
    {
        showPanelOnHover = thoughtPanelsHoverToggle.isOn;
    }

    private void OnMouseEnter()
    {
        //Debug.Log("Mouse Enter");

        if (showPanelOnHover)
            thoughtPanel.SetActive(true);
    }

    private void OnMouseExit()
    {
        //Debug.Log("Mouse Exit");
        if (showPanelOnHover)
            thoughtPanel.SetActive(false);
    }
}
