using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerTarget : MonoBehaviour {

    public Room room;
    public float timer, freq, scale;
    public float xOrg;
    public float yOrg;
    Vector3 position;

    void Awake() {
        ResetPosAndTrajectory();
    }

    public void ResetPosAndTrajectory() {
        timer = 0;
        xOrg = Random.Range(-1.0f, 1.0f);
        yOrg = Random.Range(-1.0f, 1.0f);
        position = new Vector3(transform.position.x,
                               transform.position.y,
                               0);
    }

    public void ResetPosAndTrajectoryWithXY(float xnew, float ynew) {
        timer = 0;
        xOrg = xnew;
        yOrg = ynew;
        position = new Vector3(transform.position.x,
                               transform.position.y,
                               0);
    }

    // Update is called once per frame
    void Update() {
        timer += Time.deltaTime;
        float x = Mathf.PerlinNoise(xOrg + timer * freq, 0) - 0.5f;
        float y = Mathf.PerlinNoise(yOrg + timer * freq, 0) - 0.5f;
        Vector3 offset = new Vector3(x, y, 0);
        transform.position = position + offset * scale;
    }
}
