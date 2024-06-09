using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Picker : MonoBehaviour
{
    [SerializeField] private LayerMask mask;
    [SerializeField] private TMP_Text textNoti;

    [SerializeField] private GameObject startPrompter, destPrompter;
    [SerializeField] private Button btnSetStart, btnSetDest;

    [SerializeField] private MapItem selectedStart;
    [SerializeField] private MapItem selectedEnd;
    private MapItem pendingSelect;

    private Camera mainCam;

    private void Awake()
    {
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
            LevelManager.Instance.ShowAllLayers();
            LevelManager.Instance.FindPath(this.selectedStart, this.selectedEnd);
            this.destPrompter.SetActive(false);
        });
    }

    public void Clear()
    {
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
