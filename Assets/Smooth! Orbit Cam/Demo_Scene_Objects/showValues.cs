/* ========================================================================================================
 * 70:30 Show Values Script - created by D.Michalke / 70:30 / http://70-30.de / info@70-30.de
 * demo scene script for showing the current values
 * ========================================================================================================
 */

using UnityEngine;
using System.Collections; 
using UnityEngine.UI;

public class showValues : MonoBehaviour {

    //cam system
    public GameObject CameraSystem;

	//define text-carrying objects
	public GameObject gameObjectForShowingSpeed;
	public GameObject gameObjectForShowingSmooth;
	public GameObject gameObjectForShowingOrbitSpeed;
	public GameObject gameObjectForShowingOrbit;

	

	void Update()
	{		
		//get the variable to show
		SmoothOrbitCam camScript = CameraSystem.GetComponent<SmoothOrbitCam>();

		//get the text component in the gameobject
		Text label = gameObjectForShowingSpeed.GetComponent<Text>(); 
		label.text = camScript.xSpeed.ToString (); //set the text in the text component

		//get the text component in the gameobject
		Text label2 = gameObjectForShowingSmooth.GetComponent<Text>(); 
		label2.text = camScript.smoothTime.ToString (); //set the text in the text component

		//get the text component in the gameobject
		Text label3 = gameObjectForShowingOrbitSpeed.GetComponent<Text>(); 
		label3.text = camScript.orbitingSpeed.ToString (); //set the text in the text component

		//get the text component in the gameobject
		Text label4 = gameObjectForShowingOrbit.GetComponent<Text>(); 
		label4.text = camScript.enableAutomaticOrbiting.ToString (); //set the text in the text component
	}

}
