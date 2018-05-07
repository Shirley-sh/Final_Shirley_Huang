using UnityEngine;

[CreateAssetMenu(menuName = "Resources")]
public class Resources : ScriptableObject {


    [SerializeField] private GameObject[] _rooms;
    public GameObject[] rooms {
        get { return _rooms; }
    }

    [SerializeField] private GameObject[] _levelIntros;
    public GameObject[] levelIntros {
        get { return _levelIntros; }
    }

    [SerializeField] private LevelData[] _levels;
    public LevelData[] levels {
        get { return _levels; }
    }

}