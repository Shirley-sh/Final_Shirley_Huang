using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public float panSpeed;
    public float zoomSpeed;
    public float minZoom;
    public float maxZoom;
    float panRangeX;
    float panRangeY;

    private void Start(){
        panRangeX = Services.EvolutionManager.num_rooms_per_row * Services.EvolutionManager.room_gap_width;
        panRangeY = Services.EvolutionManager.num_rows * Services.EvolutionManager.room_gap_height;

        Debug.Log(panRangeX+";"+panRangeY);
    }


    public void Pan(float inputX, float inputY){
        inputX *= panSpeed*Time.deltaTime;
        inputY *= panSpeed* Time.deltaTime;
        if(transform.position.x + inputX > 0 && transform.position.x+ inputX < panRangeX){
            transform.position = new Vector3(transform.position.x + inputX,
                                             transform.position.y,
                                             transform.position.z);
        }

        if (transform.position.y+ inputY > -panRangeY && transform.position.y+ inputY < 0) {
            transform.position = new Vector3(transform.position.x,
                                             transform.position.y+ inputY,
                                             transform.position.z);
        }

    }

    public void ZoomIn(){
        if(GetComponent<Camera>().orthographicSize -zoomSpeed*Time.deltaTime > minZoom){
            GetComponent<Camera>().orthographicSize -= zoomSpeed* Time.deltaTime;
        }

    }

    public void ZoomOut(){
        if (GetComponent<Camera>().orthographicSize + zoomSpeed * Time.deltaTime < maxZoom) {
            GetComponent<Camera>().orthographicSize += zoomSpeed * Time.deltaTime;
        }
    }
}
