using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour {

	public Grid grid;
    public List<Node2> myPath = new List<Node2>();

	void Awake() {

	}

	void Update() {

       

    }

	public IEnumerator FindPath(Vector3 startPos, Vector3 targetPos) {
		Node2 startNode = grid.NodeFromWorldPoint(startPos);
		Node2 targetNode = grid.NodeFromWorldPoint(targetPos);

		List<Node2> openSet = new List<Node2>();
		HashSet<Node2> closedSet = new HashSet<Node2>();
		openSet.Add(startNode);

		while (openSet.Count > 0) {
			Node2 node = openSet[0];
			for (int i = 1; i < openSet.Count; i ++) {
				if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost) {
					if (openSet[i].hCost < node.hCost)
						node = openSet[i];
				}
			}

			openSet.Remove(node);
			closedSet.Add(node);

			if (node == targetNode) {
				RetracePath(startNode,targetNode);  
                
                yield return new WaitForEndOfFrame();

            }

			foreach (Node2 neighbour in grid.GetNeighbours(node)) {
				if (!neighbour.walkable || closedSet.Contains(neighbour)) {
					continue;
				}

				int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
				if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
					neighbour.gCost = newCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetNode);
					neighbour.parent = node;

					if (!openSet.Contains(neighbour))
						openSet.Add(neighbour);
				}
			}

           
        }
	}

	void RetracePath(Node2 startNode, Node2 endNode) {

		List<Node2> path = new List<Node2>();
		Node2 currentNode = endNode;
        myPath.Clear();

		while (currentNode != startNode) {
			path.Add(currentNode);
			myPath.Add(currentNode);
			currentNode = currentNode.parent;
		}
		path.Reverse();
        myPath.Reverse();
        grid.path = path;
       

	}

	int GetDistance(Node2 nodeA, Node2 nodeB) {
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}

    void OnDrawGizmos()
    {
        
        if (grid != null)
        {
            foreach (var n in myPath)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (grid.nodeDiameter - .1f));
            }
        }
    }
}
