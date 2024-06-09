using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardDisplay : MonoBehaviour
{
    private Camera mainCam;
    private void Awake(){
        this.mainCam = Camera.main;
    }
    private void Update(){
        // var dirToCameraYZ = (mainCam.transform.position - transform.position).Set(x: 0).YZ();
        // var angle = -VectorUtils.GetAngleFromVector2D(dirToCameraYZ);
        // Debug.Log((dirToCameraYZ, angle));
        // var eulerAngles = new Vector3(90 - angle, 0, 0);
        transform.rotation = (CameraController.Instance.PivotPosition);
    }
}
