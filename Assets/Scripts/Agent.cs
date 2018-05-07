using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using M = MathNet.Numerics.LinearAlgebra.Matrix<float>;
using Trig = MathNet.Numerics.Trig;
using SpecialFunc = MathNet.Numerics.SpecialFunctions;

public class Agent : MonoBehaviour {

    [Header("Attributes")]
    public float moveSpeed;

    protected Rigidbody2D rd;
    protected Collider2D col;
    protected GameObject roomObj; // the agent knows which room it is in

    [HideInInspector]
    public Room room; //the agent will ask the room to give it its local state
    [HideInInspector]
    public Transform trans;

    public bool useStochasticMove;
    public float score = 0;
    public NeuralNetwork model;//this will be the agent's brain to make decisions
    public float softmax_temperature;

    public NeuralNetwork GetModel() {
        return model;
    }

    public void CopyModelFromOtherAgent(Agent other) {
        // set this agent's model's weights to be 
        // the same as another agent's model's weights
        model.setLayers(other.GetModel().getLayersCopy());
    }

    // Use this for initialization
    protected virtual void Awake() {
        //creatureGO = Resources.Load("Prefab Name") as GameObject;
        rd = gameObject.GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        trans = gameObject.transform;
    }

    protected virtual void Start() {
    }

    protected virtual void FixedUpdate() {
        //in fixed udpate we do the physics of the new move
        M localGameState = room.GetLocalState();
        int moveToTake = GetMove(localGameState, useStochasticMove);
        Move(moveToTake);
    }

    public void SetRoom(Room r) {
        room = r;
    }

    public int CompareTo(Agent other) {
        if (score < other.score) {
            return -1;
        }
        else {
            return 1;
        }
    }

    float FindMaxInMatrix(M m) {
        float maxV = -99999;
        for (int i = 0; i < m.RowCount; i++) {
            if (m[0,i] > maxV) {
                maxV = m[0, i];
            }
        }
        return maxV;
    }

    private int GetMove(M gameState, bool stochastic = true) {
        M results = model.Forward(gameState);
        //Debug.Log("state: " + gameState);
        //Debug.Log("results: " + results);

        if (stochastic) {
            // do softmax
            float sumv = 0;
            results = results / softmax_temperature;
            results -= FindMaxInMatrix(results);
            for (int i = 0; i < results.ColumnCount; i++) {
                results[0, i] = Mathf.Exp(results[0, i]);
                sumv += results[0, i];
            }

            //Debug.Log("row count: " + results.ColumnCount);

            for (int i = 0; i < results.ColumnCount; i++) {
                results[0, i] = results[0, i] / sumv;
            }

            //Debug.Log("prob: " + results);

            // then find the stochastic action
            float rn = Random.value;
            for (int i = 0; i < results.ColumnCount; i++) {
                if (rn < results[0, i]) {
                    return i;
                }
                else {
                    rn -= results[0, i];
                }
            }
        }
        else {
            //else return deterministic action
            int moveToChoose = 0;
            float maxValue = -999999;
            for (int i = 0; i < results.ColumnCount; i++) {
                if (results[0, i] > maxValue) {
                    maxValue = results[0, i];
                    moveToChoose = i;
                }
            }
            return moveToChoose;
        }

        //move[0] = ((float)SpecialFunc.Logistic(move[0])-0.5f)*20;
        //move[1] = ((float)SpecialFunc.Logistic(move[1]));
        return 0;// return default action 0, this line should never be reached
    }

    protected virtual void Move(Vector2 move) {
    }

    protected virtual void Move(int move) {
    }

 
    protected virtual void Attack() {
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision) {
    }
}

