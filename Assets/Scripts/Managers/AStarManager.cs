using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AStarManager : MonoBehaviour
{
    public AstarPath AStar;

    public void UpdateAstarPosition(Vector3 position)
    {
        AStar.data.gridGraph.center = position;
        AStar.Scan();
    }

}
