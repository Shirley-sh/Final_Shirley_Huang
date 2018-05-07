using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M = MathNet.Numerics.LinearAlgebra.Matrix<float>;

public class SeekerRoom : Room {
    public List<Food> listOfFoods;
    public GameObject agentPrefab;
    public GameObject targetPrefab;
    public SeekerTarget seekerTarget;

    protected override void Awake() {
        base.Awake();
    }

    public override void InitTask() {
        //we now simply init the agent at the center of the room
        Vector3 pos = trans.position;
        GameObject newAgentObj = GameObject.Instantiate(agentPrefab, pos, Quaternion.identity);//agent's position will be set by resetpos
        newAgentObj.transform.parent = trans;
        agent = newAgentObj.GetComponent<SeekerAgent>();
        agent.SetRoom(this);

        //then set foods

        GameObject newTargetObj = GameObject.Instantiate(targetPrefab, pos, Quaternion.identity);//agent's position will be set by resetpos
        newTargetObj.transform.parent = trans;
        seekerTarget = newTargetObj.GetComponent<SeekerTarget>();
        seekerTarget.room = this;

        ResetTask();
    }

    public override Agent GetAgent() {
        return agent;
    }

    public override void ResetTask() {
        //first reset agent position
        //WILL NOT RESET AGENT MODEL WEIGHT
        Vector3 pos = trans.position;
        agent.trans.SetPositionAndRotation(pos, Quaternion.identity);
        agent.score = 0;

        seekerTarget.transform.position = trans.position;

        if (row_index == 0) {
            seekerTarget.ResetPosAndTrajectory();
        }
        else {
            SeekerTarget firstInRowTarget = ((SeekerRoom)Services.EvolutionManager.listOfRooms[column_index]).seekerTarget;
            float x = firstInRowTarget.xOrg;
            float y = firstInRowTarget.yOrg;
            seekerTarget.ResetPosAndTrajectoryWithXY(x, y);
        }

        //Vector3 randomPos = new Vector3(Random.Range(-3.5f, 3.5f),
        //        Random.Range(-3.5f, 3.5f), 0);
        //seekerTarget.transform.position = trans.position + randomPos;
    }

    protected override void FixedUpdate() {
    }

    // Update is called once per frame
    void Update () {
	}

    public override M GetLocalState() {
        // when agent calls the room to give it its local state
        // the room will return the relative position of the closest food
        Vector3 agentPos = agent.trans.position;

        M localState = M.Build.Dense(1, 4);

        Vector3 relativePos = seekerTarget.transform.position - agentPos;

        float dx = relativePos[0];
        float dy = relativePos[1];

        if (dx > 0) {
            localState[0, 0] = 1;
            localState[0, 1] = 0;
        }
        else {
            localState[0, 0] = 0;
            localState[0, 1] = -1;
        }

        if (dy > 0) {
            localState[0, 2] = 1;
            localState[0, 3] = 0;
        }
        else {
            localState[0, 2] = 0;
            localState[0, 3] = -1;
        }

        return localState;
    }


    public float GetScore() {
        // when agent calls the room to give it its local state
        // the room will return the relative position of the closest food
        Vector3 agentPos = agent.trans.position;

        M localState = M.Build.Dense(1, 4);

        Vector3 relativePos = seekerTarget.transform.position - agentPos;

        float dx = relativePos[0];
        float dy = relativePos[1];

        float distance = relativePos.magnitude;

        float newScore = 8 - distance;
        if (newScore<0) {
            newScore = 0;
        }

        return newScore;
    }
}
