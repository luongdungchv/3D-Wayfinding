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
        listLayer.ForEach((item, index) => {
            item.SetLayerIndex(index);
        });
        CameraController.Instance.SwitchTargetFloor(this.listLayer[0]);
        
        
    }
    private void Start(){
        listLayer.ForEach((item, index) => {
            item.Init();
        });
    }

    [Sirenix.OdinInspector.Button]
    public void FindPath(MapItem start, MapItem end){
        if(start.ParentLayer == end.ParentLayer){
            var layer = start.ParentLayer;
            layer.FindPathInLayer(start.EntryPosition, end.EntryPosition);
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
            var path = layer.FindPathInLayer(start.EntryPosition, end.EntryPosition, out var length);
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
                Debug.Log(start);
                foreach(var info in distanceInfos){
                    var nextLayer = start.ParentLayer.LayerIndex < end.ParentLayer.LayerIndex ? info.connector.UpperLayer : info.connector.LowerLayer;
                    if(nextLayer == null) continue;
                    var nextConnector = start.ParentLayer.LayerIndex < end.ParentLayer.LayerIndex ? info.connector.UpperEnd : info.connector.LowerEnd;
                    Debug.Log((info.connector.name, nextLayer, nextConnector));
                    var path = new List<Vector3>(currentPath);
                    path.AddRange(info.path);
                    distList.Add(RecursivelyFindPath(nextConnector, end, currentDist + info.distance, path, out var resultPath));
                    pathList.Add(resultPath);
                }
            }
            else{
                var distancesToConnectors = start.ParentLayer.PathsToConnectors(start.EntryPosition);
                foreach(var connector in distancesToConnectors.Keys){
                    var nextLayer = start.ParentLayer.LayerIndex < end.ParentLayer.LayerIndex ? connector.UpperLayer : connector.LowerLayer;
                    var nextConnector = start.ParentLayer.LayerIndex < end.ParentLayer.LayerIndex ? connector.UpperEnd : connector.LowerEnd;
                    Debug.Log((connector.name, nextLayer, nextConnector));
                    if(nextLayer == null) continue;
                    var (path, length) = distancesToConnectors[connector];
                    var newPath = new List<Vector3>(currentPath);
                    newPath.AddRange(path);
                    distList.Add(RecursivelyFindPath(nextConnector, end, currentDist + length, newPath, out var resultPath));
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
    
    public void ShowLayer(int index){
        this.listLayer.ForEach(x => x.gameObject.SetActive(false));
        this.listLayer[index].gameObject.SetActive(true);
    }
    public void ShowLayers(List<int> indices){
        this.listLayer.ForEach(x => x.gameObject.SetActive(false));
        indices.ForEach(x => this.listLayer[x].gameObject.SetActive(true));
    }
    public void ShowAllLayers(){
        this.listLayer.ForEach(x => x.gameObject.SetActive(true));   
    }
    
    public FloorLayer GetLayer(int index){
        return this.listLayer[index];
    }
}
