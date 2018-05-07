using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M = MathNet.Numerics.LinearAlgebra.Matrix<float>;

public abstract class Room : MonoBehaviour {
    public int row_index;
    public int column_index;
    public GameObject roomObj;
    public Transform trans;
    public Agent agent;
    public float target_score;

    // Use this for initialization
    protected virtual void Awake () {
        roomObj = gameObject;
        trans = transform;
    }

    public abstract void InitTask();


    public abstract Agent GetAgent();

    public void SetAgent(Agent newAgent) {
        agent.CopyModelFromOtherAgent(newAgent);
    }

    public abstract void ResetTask();

    protected virtual void FixedUpdate() {
    }

    public abstract M GetLocalState();
}
