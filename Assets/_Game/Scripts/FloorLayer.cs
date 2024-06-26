using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloorLayer : Sirenix.OdinInspector.SerializedMonoBehaviour
{
    [SerializeField] private GridSystem grid;
    [SerializeField] private List<MapItem> mapItemList;
    [SerializeField] private List<LayerConnector> layerConnectorList;
    [SerializeField] private List<Vector2> logPath;
    [SerializeField] private Transform floorCenter;

    [SerializeField] private Dictionary<LayerConnector, List<DistanceInfo>> connectorDistanceMap;

    private int layerIndex;

    public int LayerIndex => this.layerIndex;

    private float height => transform.position.y;
    public Vector3 FloorCenter => this.floorCenter.position;

    private void Awake()
    {
        
    }

    public void Init() {
        this.CalculateConnectorDistanceMap();
        this.BakeGrid();
    }

    public void SetLayerIndex(int index) => this.layerIndex = index;

    public List<Vector3> FindPathInLayer(Vector3 start, Vector3 end, bool visualize = false)
    {
        var path2D = grid.FindPath(start.ToVectorXZ(), end.ToVectorXZ());
        var path3D = path2D.Select(node => new Vector3(node.x, height, node.y)).ToList();
        this.logPath = path2D;
        if (visualize) PathVisualizer.Instance.VisualizePath(path3D);
        return path3D;
    }
    public List<Vector3> FindPathInLayer(Vector3 start, Vector3 end, out float length, bool visualize = false)
    {
        var path2D = grid.FindPath(start.ToVectorXZ(), end.ToVectorXZ());
        var path3D = path2D.Select(node => new Vector3(node.x, height, node.y)).ToList();
        if (visualize) PathVisualizer.Instance.VisualizePath(path3D);

        var totalLength = 0f;
        for (int i = 0; i < path2D.Count - 1; i++)
        {
            var nodeLength = Vector2.Distance(path2D[i], path2D[i + 1]);
            totalLength += nodeLength;
        }
        length = totalLength;
        return path3D;
    }
    public (LayerConnector, List<Vector3>, float) FindPathToNearestConnector(Vector3 start)
    {
        LayerConnector selectedConnector = null;
        var selectedPath = new List<Vector2>();
        var minDist = float.MaxValue;
        foreach (var connector in this.layerConnectorList)
        {
            var path2D = grid.FindPath(start.ToVectorXZ(), connector.EntryPosition.ToVectorXZ());
            var totalLength = 0f;
            for (int i = 0; i < path2D.Count - 1; i++)
            {
                var length = Vector2.Distance(path2D[i], path2D[i + 1]);
                totalLength += length;
            }
            if (totalLength < minDist)
            {
                minDist = totalLength;
                selectedPath = path2D;
                selectedConnector = connector;
            }
        }
        var path3D = selectedPath.Select(node => new Vector3(node.x, height, node.y)).ToList();
        return (selectedConnector, path3D, minDist);
    }
    public Dictionary<LayerConnector, (List<Vector3>, float)> PathsToConnectors(Vector3 start)
    {
        var result = new Dictionary<LayerConnector, (List<Vector3>, float)>();
        foreach (var connector in this.layerConnectorList)
        {
            if(start == connector.EntryPosition){
                result.Add(connector, (new List<Vector3>(), 0));
                continue;
            }
            var path2D = grid.FindPath(start.ToVectorXZ(), connector.EntryPosition.ToVectorXZ());
            var totalLength = 0f;
            for (int i = 0; i < path2D.Count - 1; i++)
            {
                var length = Vector2.Distance(path2D[i], path2D[i + 1]);
                totalLength += length;
            }
            var path3D = path2D.Select(node => new Vector3(node.x, height, node.y)).ToList();
            result.Add(connector, (path3D, totalLength));
        }
        return result;
    }

    public List<DistanceInfo> GetDistanceInfo(LayerConnector connector){
        if(!this.connectorDistanceMap.ContainsKey(connector)) return null;
        return this.connectorDistanceMap[connector];
    }

    private void CalculateConnectorDistanceMap()
    {
        connectorDistanceMap ??= new Dictionary<LayerConnector, List<DistanceInfo>>();
        connectorDistanceMap.Clear();
        foreach (var start in this.layerConnectorList)
        {
            var map = this.PathsToConnectors(start.EntryPosition);
            Debug.Log(map);
            var distanceInfo = new List<DistanceInfo>();
            foreach (var connector in map.Keys)
            {
                var info = new DistanceInfo()
                {
                    connector = connector,
                    distance = map[connector].Item2,
                    path = map[connector].Item1
                };
                distanceInfo.Add(info);
            }
            connectorDistanceMap.Add(start, distanceInfo);
        }
    }

    public void AddMapItem(MapItem mapItem)
    {
        mapItemList.Add(mapItem);
    }

    private void BakeGrid(){
        foreach (var item in mapItemList){
            foreach(var collider in item.GetComponents<BoxCollider>()){
                grid.AddRectangleObstacle(collider);
            }
        }
    }

    public struct DistanceInfo
    {
        public LayerConnector connector;
        public List<Vector3> path;
        public float distance;
    }
}
