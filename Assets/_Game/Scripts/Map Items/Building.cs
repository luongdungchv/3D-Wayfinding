using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MapItem
{
    [SerializeField] private Transform entry;
    public override Vector3 EntryPosition => this.entry.position;
}
