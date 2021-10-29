using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class PointerEvent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler// required interface when using the OnPointerDown method.
{
    public bool sliderPressed = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //Do this when the mouse is clicked over the selectable object this script is attached to.
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(this.gameObject.name + " Was Clicked.");
        sliderPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log(this.gameObject.name + " Was Unclicked.");
        sliderPressed = false;
    }
}