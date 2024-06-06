using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder
{
    public List<Vector2Int> FindPath(int[,] map, Vector2Int start, Vector2Int end)
    {
        var open = new Dictionary<Vector2Int, Node>();
        var close = new Dictionary<Vector2Int, Node>();

        var startNode = new Node(start, 0, 0);
        close.Add(start, startNode);

        var directions = new List<Vector2Int>(){
            Vector2Int.up,
            Vector2Int.one,
            Vector2Int.right,
            new Vector2Int(1, -1),
            Vector2Int.down,
            -Vector2Int.one,
            Vector2Int.left,
            new Vector2Int(-1, 1)
        };

        foreach (var dir in directions)
        {
            var coord = start + dir;
            if (!IsCoordInsideMap(map, coord)) continue;
            if (map[coord.x, coord.y] > 0) continue;
            var g = CalculateManhattan(coord, start);
            var h = CalculateManhattan(coord, end);
            open.Add(coord, new Node(coord, g, h, startNode));
        }

        while (open.Count > 0)
        {
            var node = GetLeastNode(open);
            if (node.coord == end)
            {
                return GetPath(node);
            }
            open.Remove(node.coord);
            close.Add(node.coord, node);
            foreach (var dir in directions)
            {
                var coord = node.coord + dir;
                if (close.ContainsKey(coord)) continue;
                if (!IsCoordInsideMap(map, coord)) continue;
                if (map[coord.x, coord.y] > 0) continue;

                var g = CalculateManhattan(coord, start);
                var h = CalculateManhattan(coord, end);
                open.Add(coord, new Node(coord, g, h, node));
            }
        }

        return null;
    }

    private Node GetLeastNode(Dictionary<Vector2Int, Node> input)
    {
        float min = int.MaxValue;
        Node currentNode = null;
        foreach (var key in input.Keys)
        {
            var node = input[key];
            if (node.f < min)
            {
                min = node.f;
                currentNode = node;
            }
        }
        return currentNode;
    }

    private List<Vector2Int> GetPath(Node endNode)
    {
        var currentNode = endNode;
        var result = new List<Vector2Int>();
        while (currentNode != null)
        {
            result.Add(currentNode.coord);
            currentNode = currentNode.parent;
        }
        return result;
    }

    private bool IsCoordInsideMap(int[,] map, Vector2Int coord)
    {
        var width = map.GetLength(0);
        var height = map.GetLength(1);
        return coord.x > 0 && coord.x < width && coord.y > 0 && coord.y < height;
    }
    private float CalculateManhattan(Vector2Int start, Vector2Int end)
    {
        return Vector2.Distance(start, end);
    }
    public class Node
    {
        public Vector2Int coord;
        public Node parent;
        public float g, h;
        public float f => g + h;
        public bool HasParent => this.parent != null;
        public Node(Vector2Int coord, float g, float h)
        {
            this.coord = coord;
            this.g = g;
            this.h = h;
        }
        public Node(Vector2Int coord, float g, float h, Node parent) : this(coord, g, h)
        {
            this.parent = parent;
        }
    }
}
