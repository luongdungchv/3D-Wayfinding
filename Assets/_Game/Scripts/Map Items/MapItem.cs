using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItem : MonoBehaviour
{
    [SerializeField] protected FloorLayer parentLayer;
    [SerializeField] private bool pickable;
    
    public bool IsPickable => this.pickable;

    public FloorLayer ParentLayer => this.parentLayer;
    public virtual Vector3 EntryPosition => this.transform.position;
    

    public void SetParentLayer(FloorLayer parentLayer){
        this.parentLayer = parentLayer;
    }
}
