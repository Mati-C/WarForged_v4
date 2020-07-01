using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Grid : MonoBehaviour {

    public int ID;
	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	Node2[,] grid;
    List<Node2> listNodes = new List<Node2>();
    public List<Node2> path;
    public float nodeDiameter;
	int gridSizeX, gridSizeY;

	void Awake() {
		nodeDiameter = nodeRadius*2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
		CreateGrid();
	}

	void CreateGrid() {
		grid = new Node2[gridSizeX,gridSizeY];
		Vector3 worldBottomLeft = transform.position - Vector3.right * (transform.localPosition.x + gridWorldSize.x/2) - Vector3.forward *(transform.localPosition.z + gridWorldSize.y/2);

		for (int x = 0; x < gridSizeX; x ++) {
			for (int y = 0; y < gridSizeY; y ++) {
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (transform.localPosition.x + x * nodeDiameter + nodeRadius) + Vector3.forward * (transform.localPosition.z + y * nodeDiameter + nodeRadius);
				bool walkable = !(Physics.CheckSphere(worldPoint,nodeRadius,unwalkableMask));
				grid[x,y] = new Node2(walkable,worldPoint,x,y);
                listNodes.Add(grid[x, y]);
			}
		}
        
	}

   

    public List<Node2> GetNodesList()
    {
        return listNodes;
    }

	public List<Node2> GetNeighbours(Node2 node) {
		List<Node2> neighbours = new List<Node2>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
					neighbours.Add(grid[checkX,checkY]);
				}
			}
		}

		return neighbours;
	}
	

	public Node2 NodeFromWorldPoint(Vector3 worldPosition) {

        var n = listNodes.OrderBy(x=> {

            var distance = Vector3.Distance(worldPosition, x.worldPosition);
            return distance;
        }).First();

        return n;
        /*
		float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY-1) * percentY);
		return grid[x,y];
        */
	}

	
	void OnDrawGizmos() {
		Gizmos.DrawWireCube(transform.position,new Vector3(gridWorldSize.x,1,gridWorldSize.y));

		if (grid != null) {
			foreach (Node2 n in grid) {
				Gizmos.color = (n.walkable)?Color.white:Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
             
            }
		}
        
	}
}