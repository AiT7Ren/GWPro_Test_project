using System;
using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using UnityEngine;

public class SimpleController : MonoBehaviour
{
    [SerializeField] private NoneViveTestController _nonVr;
    [SerializeField] private VRTestController _viveController;

    public enum ControllerType
    {
        NotVRTest,
        VR //ну надеюсь будет работать 
    }
    public ControllerType controllerType=ControllerType.NotVRTest;

    private void Start()
    {
        if (controllerType == ControllerType.NotVRTest)
        {
            _nonVr.enabled = true;
            _viveController.enabled = false;
        }
        if (controllerType == ControllerType.VR)
        {
            _nonVr.enabled = false;
            _viveController.enabled = true;
        }
    }
}
