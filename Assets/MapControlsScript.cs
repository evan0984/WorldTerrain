using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapControlsScript : MonoBehaviour
{
    private Camera mapViewCamera;
    private GameObject mapViewer;
    private Toggle mapMarkersToggle;
    private Toggle thoughtPanelsToggle;
    private Toggle thoughtPanelsHoverToggle;
    private Button resetViewButton;
    private Button twoDViewButton;
    private Button topViewButton;

    // Start is called before the first frame update
    void Start()
    {
        mapViewCamera = GameObject.Find("Map Camera").GetComponent<Camera>();
        mapViewer = GameObject.Find("Viewer");

        mapMarkersToggle = GameObject.Find("MapMarkersToggle").GetComponent<Toggle>();
        mapMarkersToggle.onValueChanged.AddListener(delegate { MapMarkersToggleChanged(); });

        thoughtPanelsToggle = GameObject.Find("ThoughtPanelsToggle").GetComponent<Toggle>();
        thoughtPanelsToggle.onValueChanged.AddListener(delegate { ThoughtPanelsToggleChanged(); });

        thoughtPanelsHoverToggle = GameObject.Find("ThoughtPanelsHoverToggle").GetComponent<Toggle>();
        thoughtPanelsHoverToggle.onValueChanged.AddListener(delegate { ThoughtPanelsHoverToggleChanged(); });
        
        resetViewButton = GameObject.Find("ResetViewButton").GetComponent<Button>();
        resetViewButton.onClick.AddListener(delegate { ResetViewButtonClicked(); });

        twoDViewButton = GameObject.Find("2DViewButton").GetComponent<Button>();
        twoDViewButton.onClick.AddListener(delegate { TwoDViewButtonClicked(); });

        topViewButton = GameObject.Find("TopViewButton").GetComponent<Button>();
        topViewButton.onClick.AddListener(delegate { TopViewButtonClicked(); });

        thoughtPanelsToggle.isOn = false;
        thoughtPanelsHoverToggle.isOn = true;
    }

    void Update()
    {
        
    }

    private void ShowHideGameObjectByTag(string tagName, bool show)
    {
        Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>();
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].hideFlags == HideFlags.None)
            {
                if (objs[i].CompareTag(tagName))
                {
                    objs[i].gameObject.SetActive(show);
                }
            }
        }
    }

    private void MapMarkersToggleChanged()
    {
        ShowHideGameObjectByTag("MapMarkers", mapMarkersToggle.isOn);
    }

    private void ThoughtPanelsToggleChanged()
    {
        ShowHideGameObjectByTag("ThoughtPanels", thoughtPanelsToggle.isOn);
        if (thoughtPanelsToggle.isOn)
        {
            thoughtPanelsHoverToggle.isOn = false;
        }
    }

    private void ThoughtPanelsHoverToggleChanged()
    {
        if (thoughtPanelsHoverToggle.isOn)
        {
            thoughtPanelsToggle.isOn = false;
        }
    }

    private void ResetViewButtonClicked()
    {
        mapViewCamera.transform.position = new Vector3(0, 200, -500);
        mapViewCamera.transform.rotation = new Quaternion(0.17f, 0, 0, 1.0f);
        mapViewer.transform.position = new Vector3(0, 0, 0);
    }

    private void TwoDViewButtonClicked()
    {
        mapViewCamera.transform.position = new Vector3(0, 50, -1130);
        mapViewCamera.transform.rotation = new Quaternion(0, 0, 0, 1.0f);
    }

    private void TopViewButtonClicked()
    {
        mapViewCamera.transform.position = new Vector3(0, 2050, -115);
        mapViewCamera.transform.rotation = new Quaternion(0.7f, 0, 0, 0.7f);
    }
}
