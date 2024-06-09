using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private LevelManager testLevel;
    [SerializeField] private MapItem start, end;

    [Sirenix.OdinInspector.Button]
    private void Testing(){
        testLevel.FindPath(start, end);
    }
}
