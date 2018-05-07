using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager {
	
	// Update is called once per frame
	public void Update () {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        Camera.main.GetComponent<CameraController>().Pan(inputX, inputY);
        if(Input.GetKey("z"))Camera.main.GetComponent<CameraController>().ZoomIn();
        if (Input.GetKey("x")) Camera.main.GetComponent<CameraController>().ZoomOut();

	}

    //IEnumerator Fadeout(){
        
    //    yield return null;
    //}
}
