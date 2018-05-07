using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Inventory/Level", order = 1)]
public class LevelData  : ScriptableObject {
    public string name = "name";
    public Color mainColor = Color.white;
    public Color secondaryColor = Color.grey;
    public GameObject roomPrefab;
    public float targetScore;
}
