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

    public Quaternion PivotPosition => cameraPivot.rotation;

    private void Awake()
    {
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

            eulerAngles.x = Mathf.Clamp(eulerAngles.x, 30, 90);

            cameraPivot.transform.rotation = Quaternion.Euler(eulerAngles);
        }
        else if (mode == 1)
        {
            var x = eventData.delta.x * moveSensitivity;
            var y = eventData.delta.y * moveSensitivity;

            var pos = cameraPivot.transform.position;
            pos.x -= x;
            pos.z -= y;
            cameraPivot.transform.position = pos;
        }
        else if (mode == 2)
        {
            var x = -eventData.delta.x * moveSensitivity;
            var y = -eventData.delta.y * moveSensitivity;

            var pos = cameraPivot.transform.position;
            var offset = transform.up * y + transform.right * x;
            pos += offset;
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
        this.cameraPivot.position = targetPos;
        this.cameraPivot.rotation = Quaternion.Euler(eulerAngles);
    }

    public void MovePivotTo(Vector3 dest)
    {
        this.cameraPivot.transform.position = dest;
    }

    public void SwitchMode(int mode)
    {
        this.mode = mode;
    }
    
    public void SetRotation(Vector3 eulers){
        cameraPivot.transform.rotation = Quaternion.Euler(eulers);
    }
    public void SetCameraZ(float z){
        this.mainCam.transform.localPosition = mainCam.transform.localPosition.Set(z: z);
    }
}
