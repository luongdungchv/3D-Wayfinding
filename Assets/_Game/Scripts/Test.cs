using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private FloorLayer testLayer;
    [SerializeField] private GameObject start, end;

    [Sirenix.OdinInspector.Button]
    private void Testing(){
        testLayer.FindPathInLayer(start.transform.position, end.transform.position, true);
    }
}
