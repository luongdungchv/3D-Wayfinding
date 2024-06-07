using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DL.Utils;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private float cellSize;
    [SerializeField] private int cellsPerDim;
    [SerializeField] private GameObject cellVisualizerPrefab;
    [SerializeField] private List<Color> colorList;
    [SerializeField] private List<Vector2Int> edgeLog;


    private AStarPathfinder pathFinder = new AStarPathfinder();
    private int cellsPerEdge => cellsPerDim * 2;

    private int[,] occupationMatrix;
    private GameObject[,] visualizerMatrix;
    private int currentID = 1;

    private void Awake()
    {
        this.Initialize();
        //this.AddRectangleObstacle(GameObject.Find("Test Obs").GetComponent<BoxCollider>());
    }

    [Sirenix.OdinInspector.Button]
    public void Initialize()
    {
        occupationMatrix = new int[cellsPerEdge, cellsPerEdge];
        // visualizerMatrix = new GameObject[cellsPerEdge, cellsPerEdge];
        // visualizerMatrix.Loop((x, y) =>
        // {
        //     var item = Instantiate(cellVisualizerPrefab, new Vector3(x * cellSize, 0, y * cellSize), Quaternion.identity);
        //     visualizerMatrix[x, y] = item;
        // });
    }
    [Sirenix.OdinInspector.Button]
    public void AddRectangleObstacle(Vector2 center, Vector2 extend, float rotation)
    {
        Debug.Log((center, extend));
        var rad = -rotation * Mathf.Deg2Rad;
        var corners = new List<Vector2>();
        var edgeCells = new List<Vector2Int>();
        Vector2[] offsetList = {
            extend, extend.Set(y: -extend.y), -extend, extend.Set(x: -extend.x)
        };
        offsetList.ForEach(x => corners.Add(center + x));

        for (int i = 0; i < corners.Count; i++)
        {
            var corner = corners[i];
            var offset = offsetList[i];
            var rotatedCorner = new Vector2();

            var cos = Mathf.Cos(rad);
            var sin = Mathf.Sin(rad);

            rotatedCorner.x = offset.x * cos - offset.y * sin;
            rotatedCorner.y = offset.x * sin + offset.y * cos;

            corners[i] = center + rotatedCorner;
        }
        // var cellsInLine = BresenhamLine(corners[2], corners[3]);
        // cellsInLine.ForEach(coord => occupationMatrix[coord.x, coord.y] = currentID);
        // cellsInLine.ForEach(x => Debug.Log(x));

        for (int i = 0; i < corners.Count; i++)
        {
            var start = corners[i];
            var end = corners[i == corners.Count - 1 ? 0 : i + 1];
            var cellsInLine = BresenhamLine(start, end);
            edgeCells.AddRange(cellsInLine);
        }   
        edgeCells.ForEach(coord => occupationMatrix[coord.x, coord.y] = currentID);
        this.edgeLog = edgeCells;

        var (minX, maxX, minY, maxY) = GetBoundingCellCoords(corners);
        for(int y = minY; y <= maxY; y++){
            int minFilled = int.MaxValue;
            int maxFilled = int.MinValue;
            for(int x = minX; x <= maxX; x++){
                if(occupationMatrix[x, y] == currentID){
                    if(x < minFilled) minFilled = x;
                    if(x > maxFilled) maxFilled = x;
                }
            }
            for(int x = minFilled; x <= maxFilled; x++){
                occupationMatrix[x, y] = currentID;
            }
        }
        currentID++;
        //UpdateVisualizer();
    }

    [Sirenix.OdinInspector.Button]
    public void AddRectangleObstacle(BoxCollider collider)
    {
        var extend = collider.size;
        extend.x *= collider.transform.lossyScale.x;
        extend.z *= collider.transform.lossyScale.z;
        extend /= 2;
        AddRectangleObstacle(collider.bounds.center.ToVectorXZ(), extend.ToVectorXZ(), collider.transform.eulerAngles.y);
    }
    public Vector2Int WorldToCellCoord(Vector2 worldCoord)
    {
        var x = (int)(worldCoord.x / cellSize);
        var y = (int)(worldCoord.y / cellSize);
        return new Vector2Int(x, y);
    }

    private List<Vector2Int> BresenhamLine(Vector2 point1, Vector2 point2)
    {
        Vector2 start = point1.x > point2.x ? point2 : point1;
        Vector2 end = point1.x > point2.x ? point1 : point2;
        start = start.x > end.x ? end : start;
        var startCellCoord = WorldToCellCoord(start);
        var endCellCoord = WorldToCellCoord(end);
        var slope = (end.y - start.y) / (end.x - start.x);

        var result = new List<Vector2Int>();

        // Debug.Log((start, end, startCellCoord, endCellCoord, slope));

        var currentCoord = startCellCoord;
        result.Add(currentCoord);
        int c = 0;
        while (currentCoord != endCellCoord)
        {
            if (slope > -1 && slope < 1)
            {
                var moveLength = Mathf.Min((end - start).x, cellSize);
                var newPos = new Vector2(start.x + moveLength, start.y + slope * moveLength);
                if(slope > 0){
                    var p = currentCoord + Vector2Int.one;
                    var a1 = (p - start / cellSize).normalized;
                    var a2 = (end-start).normalized;
                    if(a1.y > a2.y) result.Add(WorldToCellCoord(newPos.Set(y: start.y)));
                    else result.Add(WorldToCellCoord(newPos.Set(x: start.x)));
                }
                else if(slope <= 0){
                    var p = currentCoord + Vector2Int.right;
                    var a1 = (p - start / cellSize).normalized;
                    var a2 = (end-start).normalized;
                    if(a1.y > a2.y) result.Add(WorldToCellCoord(newPos.Set(x: start.x)));
                    else result.Add(WorldToCellCoord(newPos.Set(y: start.y)));
                }

                start.y += slope * moveLength;
                start.x += moveLength;
            }
            else if (slope < -1 || slope > 1)
            {
                var moveLength = Mathf.Min(Mathf.Abs((end - start).y), cellSize);
                var newPos = new Vector2(start.x + moveLength / Mathf.Abs(slope), start.y + moveLength * (slope < 0 ? -1 : 1));
                if(slope > 1){
                    var p = currentCoord + Vector2Int.one;
                    var a1 = (p - start / cellSize).normalized;
                    var a2 = (end-start).normalized;
                    // Debug.Log((a1, a2, p));
                    if(a1.y > a2.y) result.Add(WorldToCellCoord(newPos.Set(y: start.y)));
                    else result.Add(WorldToCellCoord(newPos.Set(x: start.x)));
                }
                else if(slope <= -1){
                    var p = currentCoord + Vector2Int.right;
                    var a1 = (p - start / cellSize).normalized;
                    var a2 = (end-start).normalized;
                    if(a1.y > a2.y) result.Add(WorldToCellCoord(newPos.Set(x: start.x)));
                    else result.Add(WorldToCellCoord(newPos.Set(y: start.y)));
                }

                start.y += moveLength * (slope < 0 ? -1 : 1);
                start.x += moveLength / Mathf.Abs(slope);
            }
            else
            {
                var moveLength = Mathf.Min((end - start).x, cellSize);
                start.y += slope * moveLength;
                start.x += moveLength;
            }
            currentCoord = WorldToCellCoord(start);
            result.Add(currentCoord);
        }
        result.Add(endCellCoord);
        return result;
    }

    private (int, int, int, int) GetBoundingCellCoords(List<Vector2> corners)
    {
        float minX = int.MaxValue, maxX = int.MinValue, minY = int.MaxValue, maxY = int.MinValue;
        foreach (var coord in corners)
        {
            if (coord.x > maxX) maxX = coord.x;
            if (coord.x < minX) minX = coord.x;
            if (coord.y < minY) minY = coord.y;
            if (coord.y > maxY) maxY = coord.y;
        }

        return ((int)(minX / cellSize), (int)(maxX / cellSize), (int)(minY / cellSize), (int)(maxY / cellSize));
    }

    public List<Vector2> FindPath(Vector3 start, Vector3 end){
        var cellStartCoord = WorldToCellCoord(start);
        var cellEndCoord = WorldToCellCoord(end);
        var initResult = this.pathFinder.FindPath(this.occupationMatrix, cellStartCoord, cellEndCoord);
        var result = initResult.Select(x => (Vector2)x * cellSize + Vector2.one * cellSize / 2).ToList();
        result.Reverse();
        return result;
    }

    private void UpdateVisualizer()
    {
        this.occupationMatrix.Loop((item, x, y) =>
        {
            var renderer = visualizerMatrix[x, y].GetComponentInChildren<MeshRenderer>();
            var matBlock = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(matBlock);
            matBlock.SetColor("_Color", colorList[item]);
            renderer.SetPropertyBlock(matBlock);
            renderer.transform.parent.localScale = new Vector3(cellSize, 1, cellSize);
        });
    }
}
