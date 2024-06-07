using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;

public class LayerConnector : MapItem
{
    [SerializeField] private FloorLayer upperLayer, lowerLayer;
    [SerializeField] private LayerConnector upperEnd, lowerEnd;
    public FloorLayer UpperLayer => this.upperLayer;
    public FloorLayer LowerLayer => this.lowerLayer;
    public LayerConnector UpperEnd => this.upperEnd;
    public LayerConnector LowerEnd => this.lowerEnd;
}
