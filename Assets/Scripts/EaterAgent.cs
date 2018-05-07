using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EaterAgent : Agent {
    [HideInInspector]
    public int number_states = 4;//how many different types of actions the unit can make
    [HideInInspector]
    public int number_actions = 4;//how many different types of actions the unit can make

    protected override void Awake() {
        base.Awake();
        model = new NeuralNetwork(number_states, number_actions);
    }

    protected override void Move(Vector2 move) {
        rd.AddForce(move * moveSpeed);
    }

    protected override void Move(int move) {
        // 0, 1, 2, 3 means right, up, left, down, stay
        Vector2 force;
        switch (move) {
            case 0:
                force = new Vector2(1, 0);
                break;
            case 1:
                force = new Vector2(0, 1);
                break;
            case 2:
                force = new Vector2(-1, 0);
                break;
            case 3:
                force = new Vector2(0, -1);
                break;
            default:
                force = new Vector2(0, 0);
                break;
        }
        rd.AddForce(force * moveSpeed);
    }

    protected override void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Food")) {
            // when eat a food, add to score
            Food food = collision.gameObject.GetComponent<Food>();
            score += food.foodValue;
            // then set food value to 0
            food.foodValue = 0;
            // TakeDamage(collision.gameObject.GetComponent<BulletPlayer>().power);
        }
    }
}
