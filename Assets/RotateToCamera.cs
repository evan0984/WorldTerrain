using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCamera : MonoBehaviour
{
    private GameObject MainCamera;
    private float Speed = 20.0f;

    void Awake()
    {
        MainCamera = GameObject.FindGameObjectWithTag("StrategyCam");
    }

    void Update()
    {
        // Check if object needs to rotate
        if (this.transform.rotation.eulerAngles.y != MainCamera.transform.rotation.eulerAngles.y)
            SetRotate(this.gameObject, MainCamera);
    }

    void SetRotate(GameObject toRotate, GameObject camera)
    {
        // Rotate current game object to face specified camera
        transform.rotation = Quaternion.Lerp(toRotate.transform.rotation, camera.transform.rotation, Speed * Time.deltaTime);
    }
}
