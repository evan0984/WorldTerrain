/* ========================================================================================================
 * 70:30 Demo GUI - created by D.Michalke / 70:30 / http://70-30.de / info@70-30.de
 * demo scene script for triggering all button events
 * ========================================================================================================
 */

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class demo_GUI : MonoBehaviour {

	public GameObject targetCamSystem;

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

	public void SpeedMinus()
	{
		//button for reducing speed
		SmoothOrbitCam camScript = targetCamSystem.GetComponent<SmoothOrbitCam>();
		camScript.xSpeed -= 1;
		camScript.ySpeed -= 1;
	}

	public void SpeedPlus()
	{
		//button for increasing speed
		SmoothOrbitCam camScript = targetCamSystem.GetComponent<SmoothOrbitCam>();
		camScript.xSpeed += 1;
		camScript.ySpeed += 1;
	}

	public void SmoothMinus()
	{
		//button for reducing smoothtime
		SmoothOrbitCam camScript = targetCamSystem.GetComponent<SmoothOrbitCam>();
		camScript.smoothTime -=1;
	}

	public void SmoothPlus()
	{
		//button for increasing smoothtime
		SmoothOrbitCam camScript = targetCamSystem.GetComponent<SmoothOrbitCam>();
		camScript.smoothTime +=1;
	}

	public void OrbitMinus()
	{
		//button for reducing smoothtime
		SmoothOrbitCam camScript = targetCamSystem.GetComponent<SmoothOrbitCam>();
		camScript.orbitingSpeed -=1;
	}

	public void OrbitPlus()
	{
		//button for reducing smoothtime
		SmoothOrbitCam camScript = targetCamSystem.GetComponent<SmoothOrbitCam>();
		camScript.orbitingSpeed +=1;
	}

	public void EnableOrbit()
	{
		//button for enabling orbitmode
		SmoothOrbitCam camScript = targetCamSystem.GetComponent<SmoothOrbitCam>();
		camScript.enableAutomaticOrbiting = true;
	}

	public void DisableOrbit()
	{
		//button for enabling orbitmode
		SmoothOrbitCam camScript = targetCamSystem.GetComponent<SmoothOrbitCam>();
		camScript.enableAutomaticOrbiting = false;
	}
}
