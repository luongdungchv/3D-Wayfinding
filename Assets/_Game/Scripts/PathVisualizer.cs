using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathVisualizer : MonoBehaviour
{
    public static PathVisualizer Instance;
    [SerializeField] private Material lineMaterial;
    [SerializeField] private List<Transform> testTransforms;

    private void Awake() {
        Instance = this;
    }

    private LineRenderer lineRenderer => this.GetComponent<LineRenderer>();

    public void VisualizePath(List<Vector3> pathNodes){
        float totalLength = 0;
        for(int i = 0; i < pathNodes.Count - 1; i++){
            totalLength += Vector3.Distance(pathNodes[i], pathNodes[i + 1]);
        }
        lineMaterial.SetFloat("_Length", totalLength);
        lineRenderer.positionCount = pathNodes.Count;
        lineRenderer.SetPositions(pathNodes.ToArray());
    }

    [Sirenix.OdinInspector.Button]
    private void Test(){
        var posList = testTransforms.Select(x => x.transform.position).ToList();
        VisualizePath(posList);
    }

    public void DisableLine(){
        this.gameObject.SetActive(false);
    }
}
