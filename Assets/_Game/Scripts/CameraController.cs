using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour, IDragHandler
{
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Camera mainCam;
    [SerializeField] private float sensitivity, zoomSensitivity;

    public void OnDrag(PointerEventData eventData)
    {
        var x = eventData.delta.y * sensitivity;
        var y = eventData.delta.x * sensitivity;
        var eulerAngles = cameraPivot.eulerAngles;
        eulerAngles.x -= x;
        eulerAngles.y += y;

        eulerAngles.x = Mathf.Clamp(eulerAngles.x, 40, 90);
        //eulerAngles.y = Mathf.Clamp(eulerAngles.y, 40, 90);

        cameraPivot.transform.rotation = Quaternion.Euler(eulerAngles);
    }
    private void Update() {
        if(Input.mouseScrollDelta.y != 0){
            var scroll = Input.mouseScrollDelta.y;
            mainCam.transform.position += mainCam.transform.forward * (scroll * zoomSensitivity);
        }
    }

    public void SwtichTargetFloor(FloorLayer layer){
        var targetPos = layer.FloorCenter;
    }
}
