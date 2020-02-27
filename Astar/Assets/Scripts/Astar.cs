using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
	/// <summary>
	/// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path
	/// Note that you will probably need to add some helper functions
	/// from the startPos to the endPos
	/// </summary>
	/// <param name="startPos"></param>
	/// <param name="endPos"></param>
	/// <param name="grid"></param>
	/// <returns></returns>
	public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
	{
		List<Node> openList = new List<Node>();
		List<Node> closedList = new List<Node>();

		Node _startNode = new Node(startPos, null, 0, 0);
		Node _currentNode = _startNode;
		openList.Add(_startNode);

		while (openList.Count > 0)
		{
			_currentNode = openList[0];

			for (int i = 1; i < openList.Count; i++)
			{
				if (openList[i].FScore < _currentNode.FScore)
				{
					_currentNode = openList[i];
				}
			}

			openList.Remove(_currentNode);
			closedList.Add(_currentNode);

			if (_currentNode.position == endPos)
			{
				break;
			}

			foreach (Cell neighbour in GetNeighbours(grid[_currentNode.position.x, _currentNode.position.y], grid))
			{
				Node _neighbourNode = new Node(neighbour.gridPosition, _currentNode, (int)_currentNode.GScore + 1, GetDistance(neighbour.gridPosition, endPos));

				if (closedList.Any(n => n.position == neighbour.gridPosition))
				{
					continue;
				}

				if (openList.Any(n => n.position == neighbour.gridPosition && _neighbourNode.GScore > n.GScore))
				{
					continue;
				}
				openList.Add(_neighbourNode);
			}
		}
		return RetracePath(_startNode, _currentNode);
	}

	private List<Vector2Int> RetracePath(Node startNode, Node endNode)
	{
		List<Vector2Int> pathValue = new List<Vector2Int>();
		Node _currentNode = endNode;

		while (_currentNode != startNode)
		{
			pathValue.Add(_currentNode.position);
			_currentNode = _currentNode.parent;
		}
		pathValue.Reverse();
		return pathValue;
	}

	private List<Cell> GetNeighbours(Cell cell, Cell[,] grid)
	{
		List<Cell> _getNeighbours = new List<Cell>();
		int _xStart, _xEnd, _yStart, _yEnd;

		_xStart = _xEnd = _yStart = _yEnd = 0;

		if (!cell.HasWall(Wall.LEFT)) { _xStart = -1; }
		if (!cell.HasWall(Wall.RIGHT)) { _xEnd = 1; }
		if (!cell.HasWall(Wall.DOWN)) { _yStart = -1; }
		if (!cell.HasWall(Wall.UP)) { _yEnd = 1; }

		for (int x = _xStart; x <= _xEnd; x++)
		{
			for (int y = _yStart; y <= _yEnd; y++)
			{
				int _cellX = cell.gridPosition.x + x;
				int _cellY = cell.gridPosition.y + y;

				if (_cellX < 0 || _cellX >= grid.GetLength(0) || _cellY < 0 || _cellY >= grid.GetLength(1) || Mathf.Abs(x) == Mathf.Abs(y))
				{
					continue;
				}
				_getNeighbours.Add(grid[_cellX, _cellY]);
			}
		}
		return _getNeighbours;
	}

	private int GetDistance(Vector2Int beginPoint, Vector2Int endPoint)
	{
		int _getDistance = Mathf.Abs(beginPoint.x - endPoint.x) + Mathf.Abs(beginPoint.y - endPoint.y);
		return _getDistance;
	}
}	

/// <summary>
/// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
/// </summary>
public class Node
{
	public Vector2Int position; //Position on the grid
	public Node parent; //Parent Node of this node

	public float FScore
	{ //GScore + HScore
		get { return GScore + HScore; }
	}
	public float GScore; //Current Travelled Distance
	public float HScore; //Distance estimated based on Heuristic

	public Node() { }
	public Node(Vector2Int position, Node parent, int GScore, int HScore)
	{
		this.position = position;
		this.parent = parent;
		this.GScore = GScore;
		this.HScore = HScore;
	}
}