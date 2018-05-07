using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using M = MathNet.Numerics.LinearAlgebra.Matrix<float>;

public class EvolutionManager : MonoBehaviour {
    // the job of the evolution manager is to create rooms at the beginning of the game

    public int num_rows;
    public int num_rooms_per_row;
    public int num_food_per_room;
    public float room_gap_width;
    public float room_gap_height;
    public float room_width_for_food_gen;
    public float room_height_for_food_gen;
    public float food_min_distance_to_center;
    // the total number of rooms is defined by the number of rows and num
    // of rooms per row.
    public float ratio_of_elite;
    public float ratio_of_random_new_individuals;

    public bool agent_use_stochastic;
    public bool use_random_food_setup;
    public bool agent_at_center_setup;

    public bool auto_evolve;
    public float auto_evolve_time_interval;
    public float auto_evolve_timer;

    public List<Room> listOfRooms;//here stores all the rooms
    [HideInInspector]
    public float ave_score;
    public GameObject roomPrefab;
    M Good130BestIndividualWeights;
    Agent currentBestAgent = null;

    public int generation_count = 0;

    public List<List<Vector3>> foodPositionMap;

    GameObject levelWrapper;
    public float targetScore;
    bool levelFinished;
    public float levelProgressPercentage;

    static int SortByScore(Agent a1, Agent a2) {
        if (a1.score < a2.score) {
            return -1;
        }
        else {
            return 1;
        }
    }

    private void Awake() {
        auto_evolve_timer = auto_evolve_time_interval;
    }

    // Use this for initialization
    void Start () {
        levelWrapper = GameObject.FindWithTag("Level Wrapper");

        InitAllRooms();
        InitAllAgents();

        //////////////////// for showcase purposes
        float[,] x ={{ 0.0051074f, - 0.00197719f, - 0.00112816f, - 0.00242918f},
        { -0.00544416f,    0.0207213f, - 0.0201701f, - 0.0191051f},
            { -0.00377308f,   0.00882821f, - 0.00431628f, - 0.00935774f},
            { -0.00509104f,   0.00312184f,   0.00687503f,   0.00847564f},
            { -0.015756f, - 0.0114021f, - 0.00479246f, - 0.00715397f}};
        Good130BestIndividualWeights = M.Build.DenseOfArray(x);
    }

    void InitAllRooms() {
        //call this at start to get all the rooms init.
        listOfRooms = new List<Room>();
        for (int r = 0; r < num_rows; r++) {
            for (int c = 0; c < num_rooms_per_row; c++) {
                Vector3 pos = new Vector3(c * room_gap_width, -r * room_gap_height, 0);
                GameObject newRoomObj = GameObject.Instantiate(roomPrefab, pos, Quaternion.identity);
                newRoomObj.transform.parent = levelWrapper.transform;
                Room newRoom = newRoomObj.GetComponent<Room>();
                newRoom.row_index = r;
                newRoom.column_index = c;
                listOfRooms.Add(newRoom);
                newRoom.InitTask();
            }
        }
    }

    void InitAllAgents() {
        //call this after all rooms are init, now for each room, we have the agents init.
        for (int r = 0; r < num_rows; r++) {
            for (int c = 0; c < num_rooms_per_row; c++) {
                int index = r * num_rooms_per_row + c;
                Room room = listOfRooms[index];
                if (c!=0) {
                    Agent firstInRowAgent = listOfRooms[r * num_rooms_per_row].GetAgent();
                    room.SetAgent(firstInRowAgent);
                }
            }
        }
    }

    void EvolveToNextGeneration(bool showScoreLog=false) {
        // when this function is called, we look at the scores of all agents
        // and then just do a natural selection and reproduction
        // then the new population is assigned to the rooms.

        //first we need to get all the agents from all the rooms
        List<Agent> population = new List<Agent>();
        //now we let each row has one single agent (each agent in the row has the same weight)

        for (int r = 0; r < num_rows; r++) {
            int roomIndex = r * num_rooms_per_row;
            Agent agentToEvaluate = listOfRooms[roomIndex].agent;
            for (int c = 1; c < num_rooms_per_row; c++) {
                Agent agentWithSameGenes = listOfRooms[r*num_rooms_per_row+c].agent;
                agentToEvaluate.score += agentWithSameGenes.score;
            }
            agentToEvaluate.score /= num_rooms_per_row;
            population.Add(agentToEvaluate);
        }

        population.Sort((x, y) => x.CompareTo(y));
        population.Reverse();

        //the following one line is for displaying purposes
        currentBestAgent = population[0];

        //now population is sorted and the index = 0 individual is the best one
        int population_size = population.Count;
        int num_elite = (int)Mathf.Floor(population_size * ratio_of_elite);
        int num_random = (int)Mathf.Floor(population_size * ratio_of_random_new_individuals);
        Assert.IsTrue(population_size > (num_elite+ num_random));
        Assert.IsTrue(num_elite>0);

        for (int i = population_size - 1; i >= num_elite+ num_random; i--) {// i is the index of an individual to be discarded
            // first find a good parent from the elites to provide NN weights
            int parentIndex = Random.Range(0, num_elite);
            // now we get the mutated weights of this parent
            List<M> listOfLayers = population[parentIndex].GetModel().GetMutatedLayersCopy();
            //then we set the child's weights to be this new mutated weights
            //notice here discarding the worst is done at the same time that new sibling is made
            population[i].GetModel().setLayers(listOfLayers);
        }

        for (int i = num_elite + num_random-1; i >= num_elite; i--) {// i is the index of an individual to be discarded
            // set this new individual to have random weights
            population[i].GetModel().RandomlyInitLayers();
        }

        if (showScoreLog) {
            ave_score = 0;
            for (int i = 0; i < population.Count; i++) {
                ave_score += population[i].score;
            }
            ave_score /= population.Count;
            Debug.Log("Generation: "+generation_count+". Current best score: "+population[0].score+". Average score: "+ave_score);
        }

        //in the end we have reached a new generation of population, we reset the scores 
        // so the evolution proceeds.

        // now we need to assign the agents to rooms
        for (int r = 0; r < num_rows; r++) {
            Agent firstInRowAgent = population[r];
            for (int c = 0; c < num_rooms_per_row; c++) {
                Room room = listOfRooms[r * num_rooms_per_row + c];
                Agent agent = room.agent;
                agent.CopyModelFromOtherAgent(firstInRowAgent);
                room.ResetTask();
            }
        }

        // add to generations evolved count
        generation_count += 1;
    }

    // Update is called once per frame
    void Update () {
        if (auto_evolve) {
            auto_evolve_timer -= Time.deltaTime;
            if (auto_evolve_timer<=0) {
                EvolveToNextGeneration(true);
                auto_evolve_timer += auto_evolve_time_interval;
            }
        }




        if (Input.GetKeyDown("b")) {
            if (currentBestAgent != null) {
                Debug.Log("current best agent weights:" + currentBestAgent.GetModel().getLayersCopy()[0]);
            }
        }

        if (Input.GetKeyDown("=")) {
            Debug.Log("Apply pretrained individuals");
            for (int c = 0; c < num_rooms_per_row; c++) {
                List<M> listOfLayers = new List<M>();
                listOfLayers.Add(Good130BestIndividualWeights);
                listOfRooms[c].agent.GetModel().setLayers(listOfLayers);
            }
        }

        if (Input.GetKeyDown("k")) {
            EvolveToNextGeneration(true);
            auto_evolve_timer = auto_evolve_time_interval;
        }

	}

    public void Reset(){
        auto_evolve_timer = auto_evolve_time_interval;
        generation_count = -1;
        listOfRooms.Clear();
        levelFinished = false;
        InitAllRooms();
        InitAllAgents();
        EvolveToNextGeneration(true);
    }

    public float getEvoProgress(){
        if(!levelFinished){
            levelProgressPercentage = ave_score/targetScore*100;
            if(levelProgressPercentage>100){
                levelProgressPercentage = 100;
                levelFinished = true;
                Services.EventManager.Fire(new EvolveComplete());
            }
        }else{
            levelProgressPercentage = 100;
        }
        return levelProgressPercentage;
    }

}


public class EvolveComplete : GameEvent{

    public EvolveComplete() {
    }
}