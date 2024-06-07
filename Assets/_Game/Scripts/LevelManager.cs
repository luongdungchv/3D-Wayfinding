using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Utilities;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    [SerializeField] private List<FloorLayer> listLayer;
    [SerializeField] private List<Vector3> logResult;

    private void Awake() {
        Instance = this;
        listLayer.ForEach((item, index) => item.SetLayerIndex(index));
    }

    [Sirenix.OdinInspector.Button]
    public void FindPath(MapItem start, MapItem end){
        if(start.ParentLayer == end.ParentLayer){
            var layer = start.ParentLayer;
            layer.FindPathInLayer(start.transform.position, end.transform.position);
        }
        else{
            var startLayerIndex = start.ParentLayer.LayerIndex;
            var endLayerIndex = end.ParentLayer.LayerIndex;
            RecursivelyFindPath(start, end, 0, new List<Vector3>(), out var result);
            this.logResult = result;
            PathVisualizer.Instance.VisualizePath(result);
        }
    }

    private float RecursivelyFindPath(MapItem start, MapItem end, float currentDist, in List<Vector3> currentPath, out List<Vector3> result){
        if(start.ParentLayer == end.ParentLayer){
            var layer = start.ParentLayer;
            var path = layer.FindPathInLayer(start.transform.position, end.transform.position, out var length);
            result = new List<Vector3>(currentPath);
            result.AddRange(path);
            return currentDist + length;
        }
        else{
            var distList = new List<float>();
            var pathList = new List<List<Vector3>>();
            if(start is LayerConnector){
                var startConnector = start as LayerConnector;
                var distanceInfos = start.ParentLayer.GetDistanceInfo(startConnector);
                foreach(var info in distanceInfos){
                    var nextLayer = start.ParentLayer.LayerIndex < end.ParentLayer.LayerIndex ? info.connector.UpperLayer : info.connector.LowerLayer;
                    if(nextLayer == null) continue;
                    var nextConnector = start.ParentLayer.LayerIndex < end.ParentLayer.LayerIndex ? info.connector.UpperEnd : info.connector.LowerEnd;
                    distList.Add(RecursivelyFindPath(nextConnector, end, currentDist + info.distance, info.path, out var resultPath));
                    pathList.Add(resultPath);
                }
            }
            else{
                var distancesToConnectors = start.ParentLayer.PathsToConnectors(start.transform.position);
                foreach(var connector in distancesToConnectors.Keys){
                    var nextLayer = start.ParentLayer.LayerIndex < end.ParentLayer.LayerIndex ? connector.UpperLayer : connector.LowerLayer;
                    var nextConnector = start.ParentLayer.LayerIndex < end.ParentLayer.LayerIndex ? connector.UpperEnd : connector.LowerEnd;
                    Debug.Log((connector.name, nextLayer, nextConnector));
                    if(nextLayer == null) continue;
                    var (path, length) = distancesToConnectors[connector];
                    distList.Add(RecursivelyFindPath(nextConnector, end, currentDist + length, path, out var resultPath));
                    pathList.Add(resultPath);
                }
            }

            var minDist = float.MaxValue;
            List<Vector3> minPath = null;
            for(int i = 0; i < distList.Count; i++){
                var dist = distList[i];
                if(dist < minDist){
                    minDist = dist;
                    minPath = pathList[i];
                }
            }
            result = minPath;
            return minDist;
        }
    }
}
