using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour, IDragHandler
{
    public static CameraController Instance;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Camera mainCam;
    [SerializeField] private float rotateSensitivity, zoomSensitivity, moveSensitivity;
    [SerializeField] private int mode = 0; //0: rotate, 1: move;
    
    private void Awake(){
        Instance = this;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (mode == 0)
        {
            var x = eventData.delta.y * rotateSensitivity;
            var y = eventData.delta.x * rotateSensitivity;
            var eulerAngles = cameraPivot.eulerAngles;
            eulerAngles.x -= x;
            eulerAngles.y += y;

            eulerAngles.x = Mathf.Clamp(eulerAngles.x, 40, 90);

            cameraPivot.transform.rotation = Quaternion.Euler(eulerAngles);
        }
        else if (mode == 1){
            var x = eventData.delta.x * moveSensitivity;
            var y = eventData.delta.y * moveSensitivity;
            
            var pos = cameraPivot.transform.position;
            pos.x -= x;
            pos.z -= y;
            cameraPivot.transform.position = pos;
        }
    }
    private void Update()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            var scroll = Input.mouseScrollDelta.y;
            mainCam.transform.position += mainCam.transform.forward * (scroll * zoomSensitivity);
        }
    }

    public void SwitchTargetFloor(FloorLayer layer)
    {
        var targetPos = layer.FloorCenter;
        var eulerAngles = new Vector3(60, 0, 0);
        this.transform.position = targetPos;
        this.transform.rotation = Quaternion.Euler(eulerAngles);
    }
}
