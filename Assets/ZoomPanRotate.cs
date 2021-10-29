using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomPanRotate : MonoBehaviour
{
    public Transform target;
    public float maxOffsetDistance = 2000f;
    public float orbitSpeed = 15f;
    public float panSpeed = .5f;
    public float zoomSpeed = 10f;
    private Vector3 targetOffset = Vector3.zero;
    private Vector3 targetPosition;

    private Plane plane;
    private Vector3 dragOrigin;

    // Use this for initialization
    void Start()
    {
        if (target != null)
        {
            //transform.LookAt(target);
        }

        plane = new Plane(Vector3.up, new Vector3(0, 1f, 0));
    }

    void Update()
    {
        GameObject theSlider = GameObject.Find("Slider");
        if (theSlider != null)
        {
            PointerEvent pointerEventScript = theSlider.GetComponent<PointerEvent>();

            if (pointerEventScript != null && pointerEventScript.sliderPressed)
            {
                // User has pressed slider so do not perform zoom, pan, or rotate
                return;
            }
        }

        if (target != null)
        {
            targetPosition = target.position + targetOffset;

            if (Input.GetMouseButtonDown(0))
            {
                dragOrigin = Input.mousePosition;
                return;
            }

            // Right Mouse to Orbit
            if (Input.GetMouseButton(1))
            {
                // Rotate around mouse click point
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (plane.Raycast(ray, out float distance))
                {
                    targetPosition = ray.GetPoint(distance);
                }

                transform.RotateAround(targetPosition, Vector3.up, Input.GetAxis("Mouse X") * orbitSpeed);
                float pitchAngle = Vector3.Angle(Vector3.up, transform.forward);
                float pitchDelta = -Input.GetAxis("Mouse Y") * orbitSpeed;
                float newAngle = Mathf.Clamp(pitchAngle + pitchDelta, 100f, 180f);
                pitchDelta = newAngle - pitchAngle;
                transform.RotateAround(targetPosition, transform.right, pitchDelta);
            }
            // Left Mouse To Move in X and/or Z axis
            if (Input.GetMouseButton(0))
            {
                // Left Mouse To Pan
                //Vector3 offset = transform.right * -Input.GetAxis("Mouse X") * panSpeed + transform.up * -Input.GetAxis("Mouse Y") * panSpeed;
                //Vector3 newTargetOffset = Vector3.ClampMagnitude(targetOffset + offset, maxOffsetDistance);
                //transform.position += newTargetOffset - targetOffset;
                //targetOffset = newTargetOffset;

                Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
                Vector3 move = new Vector3(pos.x * panSpeed * -1, 0, pos.y * panSpeed * -1);

                target.transform.Translate(move, Space.World);
                transform.Translate(move, Space.World);
            }

            // Scroll to Zoom
            transform.position += transform.forward * Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
            // TODO: Limit zoom factor
        }
    }
}