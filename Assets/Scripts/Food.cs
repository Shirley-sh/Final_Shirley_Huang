using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour {
    public Sprite[] sprites;
    public Transform trans;
    public float foodValue;
    public float initialFoodValue = 10;
    float rotateSpeed;

    void Awake() {
        trans = gameObject.transform;
        Reset();
    }

    // Use this for initialization
    void Start () {
        Reset();
    }

    public void Reset() {
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
        gameObject.SetActive(true);
        foodValue = initialFoodValue;
        GetComponent<Rigidbody2D>().AddTorque(Random.Range(-50, 50));
    }

    // Update is called once per frame
    void Update () {
        
	}

    public void UpdateFoodState() {
        // if got eaten already, set gameobj to false
        if (foodValue <= 0) {
            gameObject.SetActive(false);
        }
    }

    private void FixedUpdate() {
        //
    }

    Vector3 GetPos() {
        return trans.position;
    }
}
