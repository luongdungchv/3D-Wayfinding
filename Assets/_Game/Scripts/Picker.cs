using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Picker : MonoBehaviour
{
    public static Picker Instance;
    [SerializeField] private LayerMask mask;
    [SerializeField] private TMP_Text textNoti;

    [SerializeField] private GameObject startPrompter, destPrompter;
    [SerializeField] private Button btnSetStart, btnSetDest, btnReturn;

    [SerializeField] private MapItem selectedStart;
    [SerializeField] private MapItem selectedEnd;
    
    public bool IsInViewPathMode => selectedEnd != null && selectedStart != null;
    private MapItem pendingSelect;

    private Camera mainCam;

    private void Awake()
    {
        Instance = this;
        mainCam = Camera.main;
        btnSetStart.onClick.AddListener(() => {
            Debug.Log("start click");
            this.selectedStart = pendingSelect;
            this.pendingSelect = null;
            this.startPrompter.SetActive(false);
        });
        btnSetDest.onClick.AddListener(() =>
        {
            Debug.Log("end Click");
            this.selectedEnd = pendingSelect;
            pendingSelect = null;
            //LevelManager.Instance.ShowAllLayers();
            var startIndex = selectedStart.ParentLayer.LayerIndex;
            var endIndex = selectedEnd.ParentLayer.LayerIndex;
            var plus = startIndex > endIndex ? -1 : 1;
            var showLayers = new List<int>();
            for(int i = selectedStart.ParentLayer.LayerIndex; i != endIndex + plus; i += plus){
                showLayers.Add(i);
            }
            LevelManager.Instance.ShowLayers(showLayers);
            LevelManager.Instance.FindPath(this.selectedStart, this.selectedEnd);
            this.destPrompter.SetActive(false);
            
            var medianPos = (selectedStart.ParentLayer.FloorCenter + selectedEnd.ParentLayer.FloorCenter) / 2;
            CameraController.Instance.MovePivotTo(medianPos);
            CameraController.Instance.SwitchMode(2);
            CameraController.Instance.SetCameraZ(-45);
            CameraController.Instance.SetRotation(new Vector3(40, 0, 0));
            btnReturn.gameObject.SetActive(true);
        });
        btnReturn.onClick.AddListener(() =>{
            this.Clear();
            btnReturn.gameObject.SetActive(false); 
        });
    }

    public void Clear()
    {
        CameraController.Instance.SwitchMode(0);
        CameraController.Instance.MovePivotTo(selectedStart.ParentLayer.FloorCenter);
        this.selectedEnd = null;
        this.selectedStart = null;
        pendingSelect = null;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 100, mask))
            {
                Debug.Log(hit.collider.gameObject);
                var mapItem = hit.collider.GetComponent<MapItem>();
                if (mapItem == null || !mapItem.IsPickable) return;
                CameraController.Instance.MovePivotTo(mapItem.transform.position);
                if(selectedStart == null){
                    this.pendingSelect = mapItem;
                    this.ShowStartPointPrompt(mapItem);
                }
                else if(selectedEnd == null){
                    this.pendingSelect = mapItem;
                    this.ShowDestPointPrompt(mapItem);
                }
            }
        }
    }

    private void ShowStartPointPrompt(MapItem start)
    {
        this.startPrompter.SetActive(true);
        this.startPrompter.transform.position = start.transform.position;
    }
    private void ShowDestPointPrompt(MapItem dest)
    {
        this.destPrompter.SetActive(true);
        this.destPrompter.transform.position = dest.transform.position;
    }

}
