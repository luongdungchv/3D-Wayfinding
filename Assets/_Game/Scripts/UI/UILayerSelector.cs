using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.Utilities;

public class UILayerSelector : MonoBehaviour
{
    [SerializeField] private List<Button> listBtn;
    
    private void Awake(){
        this.listBtn.ForEach((btn, index) => btn.onClick.AddListener(() => HandleButtonClick(index)));
    }
    private void HandleButtonClick(int index){
        LevelManager.Instance.ShowLayer(index);
        CameraController.Instance.SwitchTargetFloor(LevelManager.Instance.GetLayer(index));
    }
}
