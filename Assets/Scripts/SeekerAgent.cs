using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerAgent : Agent {
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

    protected override void FixedUpdate() {
        base.FixedUpdate();
        //in fixed udpate we do the physics of the new move
        SeekerRoom sroom = (SeekerRoom)room;
        score += sroom.GetScore();
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
        //trans.LookAt(trans.position + new Vector3(force.normalized.x, force.normalized.y, 0));
    }
}
