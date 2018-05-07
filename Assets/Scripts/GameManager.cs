using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager: MonoBehaviour {
    public GameObject evolutionManager;
    public Transform levelWrapper;
    public Resources resources;
    public LevelData[] levels;
    public LevelData currentLevel;

    List<GameObject> Rooms;
    [SerializeField] int currentLevelIndex=0;


	void Awake () {
        Services.EventManager = new EventManager();
        Services.InputManager = new InputManager();
        Services.EvolutionManager = evolutionManager.GetComponent<EvolutionManager>();
        Services.GameManager = this;
        Services.Resources = resources;
        levels = Services.Resources.levels;
        ProceedToNextLevel();
    }

    void SetNewLevel(GameObject roomPrefab) {
        Services.EvolutionManager.roomPrefab = roomPrefab;
    }
	
	// Update is called once per frame
	void Update () {
        Services.InputManager.Update();
        if (Input.GetKeyDown("n")) {
            ProceedToNextLevel();
        }
	}

    public void ProceedToNextLevel(){
        if(currentLevelIndex==0){
            GameObject room = levels[currentLevelIndex].roomPrefab;
            SetNewLevel(room);
            currentLevel = levels[currentLevelIndex];
            currentLevelIndex++;
        }else if (currentLevelIndex < Services.Resources.rooms.Length) {
            ClearLevel();
            GameObject room = levels[currentLevelIndex].roomPrefab;
            SetNewLevel(room);
            Services.EvolutionManager.Reset();
            currentLevel = levels[currentLevelIndex];
            currentLevelIndex++;
        }else{
            currentLevelIndex = 0;
            ClearLevel();
            GameObject room = levels[currentLevelIndex].roomPrefab;
            SetNewLevel(room);
            Services.EvolutionManager.Reset();
            currentLevel = levels[currentLevelIndex];
            currentLevelIndex++;
        }
        Services.EvolutionManager.targetScore = currentLevel.targetScore;
    }

    void ClearLevel(){
        foreach (Transform child in levelWrapper) {
            GameObject.Destroy(child.gameObject);
        }
    }

    void OnEnterDemoMode() {
    }

    void OnDestroy() {
    }

}
