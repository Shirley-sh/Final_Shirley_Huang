using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M = MathNet.Numerics.LinearAlgebra.Matrix<float>;

public class EaterRoom : Room {
    public List<Food> listOfFoods;
    public GameObject agentPrefab;
    public GameObject foodPrefab;
    List<Vector3> foodPositions;

    protected override void Awake() {
        base.Awake();
        foodPositions = new List<Vector3>();
    }

    public override void InitTask() {
        //we now simply init the agent at the center of the room
        Vector3 pos = trans.position;
        GameObject newAgentObj = GameObject.Instantiate(agentPrefab, pos, Quaternion.identity);//agent's position will be set by resetpos
        newAgentObj.transform.parent = trans;
        agent = newAgentObj.GetComponent<EaterAgent>();
        agent.SetRoom(this);
        //then set foods

        for (int i = 0; i < Services.EvolutionManager.num_food_per_room; i++) {
            pos = roomObj.transform.position;
            GameObject newFoodObj = GameObject.Instantiate(foodPrefab, pos, Quaternion.identity);
            newFoodObj.transform.parent = trans;
            listOfFoods.Add(newFoodObj.GetComponent<Food>());
        }

        ResetTask();
    }

    public override Agent GetAgent() {
        return agent;
    }

    void GenerateListOfFoodRelativePositions() {
        foodPositions.Clear();
        for (int i = 0; i < Services.EvolutionManager.num_food_per_room; i++) {
            // each food is generated at a random position (but not wihtin a distance to the center)
            Vector3 pos = new Vector3(Random.Range(-3.5f, 3.5f),
                Random.Range(-3.5f, 3.5f), 0);
            foodPositions.Add(pos);
        }
    }

    public override void ResetTask() {
        //first reset agent position
        //WILL NOT RESET AGENT MODEL WEIGHT
        Vector3 pos = trans.position;
        agent.trans.SetPositionAndRotation(pos, Quaternion.identity);
        agent.score = 0;

        //then reset food positions
        if (row_index==0) {
            GenerateListOfFoodRelativePositions();
        }
        else {
            foodPositions = ((EaterRoom)Services.EvolutionManager.listOfRooms[column_index]).foodPositions;
        }

        for (int i = listOfFoods.Count - 1; i >= 0; i--) {
            pos = trans.position + foodPositions[i];
            listOfFoods[i].trans.SetPositionAndRotation(pos, Quaternion.identity);
            listOfFoods[i].Reset();
        }
    }

    protected override void FixedUpdate() {
        for (int i = listOfFoods.Count - 1; i >= 0; i--) {
            Food food = listOfFoods[i];
            food.UpdateFoodState();
        }
    }


    public override M GetLocalState() {
        // when agent calls the room to give it its local state
        // the room will return the relative position of the closest food
        Vector3 agentPos = agent.trans.position;

        M localState = M.Build.Dense(1, 4);

        int closestFoodIndex = -1;
        float minDistance = 9999999;

        for (int i = 0; i < listOfFoods.Count; i++) {
            if (listOfFoods[i].foodValue<=0) {
                continue;
            }

            float distance = Vector3.Distance(agentPos, listOfFoods[i].trans.position);
            if (distance < minDistance) {
                minDistance = distance;
                closestFoodIndex = i;
            }
        }

        if (closestFoodIndex >= 0) {
            Vector3 relativePos = listOfFoods[closestFoodIndex].trans.position - agentPos;
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
        }
        return localState;
    }
}
