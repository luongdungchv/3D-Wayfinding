using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItem : MonoBehaviour
{
    [SerializeField] protected FloorLayer parentLayer;

    public FloorLayer ParentLayer => this.parentLayer;

    public void SetParentLayer(FloorLayer parentLayer){
        this.parentLayer = parentLayer;
    }
}
