using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICameraModeSwitcher : MonoBehaviour
{
    [SerializeField] private Button btnMoveMode, btnRotateMode;
    
    private void Awake(){
        btnMoveMode.onClick.AddListener(() => {
            if(Picker.Instance.IsInViewPathMode) SwitchMode(2);
            else SwitchMode(1);
        });
        btnRotateMode.onClick.AddListener(() => SwitchMode(0));
    }
    
    private void SwitchMode(int newMode){
        CameraController.Instance.SwitchMode(newMode);
    }
}
