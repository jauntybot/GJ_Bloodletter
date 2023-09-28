using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcGenMaze : MonoBehaviour {


    public Vector2 cellCount;

    [SerializeField] GameObject[] cellPrefabs;
    public Dictionary<Vector2, MazeCell> mazeCells;

    void GenerateMaze() {

        for (int i = 0; i < cellCount.x * cellCount.y; i++) {
            
        }

    }

}
