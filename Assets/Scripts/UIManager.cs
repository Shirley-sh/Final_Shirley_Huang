using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public Text timerText;
    public GameObject bottomText;
    public Text levelFinishedText;
    public Text levelProgressText;
    public Text generationCountText;

    private void Start(){
        Services.EventManager.Register<EvolveComplete>(OnEvolveComplete);
    }

    // Update is called once per frame
    void Update() {
        timerText.text = ((int)Services.EvolutionManager.auto_evolve_timer).ToString();
        levelProgressText.text = ((int)Services.EvolutionManager.getEvoProgress()).ToString()+"%";
        generationCountText.text = "Gen "+ (Services.EvolutionManager.generation_count+1).ToString();
    }

    void OnEvolveComplete(GameEvent e) {
        bottomText.SetActive(true);
        levelFinishedText.text = "Learned how to "+Services.GameManager.currentLevel.name+".";
    }

}
