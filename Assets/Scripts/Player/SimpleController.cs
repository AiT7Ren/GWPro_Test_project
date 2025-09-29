using System;
using UnityEngine;
using UnityEngine.Serialization;

public class SimpleController : MonoBehaviour
{
    [Header("Chose ControllerType")]
    public ControllerType controllerType=ControllerType.NotVRTest;
    [FormerlySerializedAs("_noneVrPlayerController")]
    [Header("Controllers Prefabs")]
    [SerializeField] private NoneViveTestController _noneVrPlayerController; 
    [SerializeField] private VRTestPlayerController _vivePlayerController;
    private IPlayerControllerIniter _playerControllerIniter;
    [SerializeField]private Inventory _inventoryHolder;
    [SerializeField]private Camera _mainCamera;
    public enum ControllerType
    {
        NotVRTest,
        VR
    }
    private void Start()
    {
        _playerControllerIniter = controllerType switch
        {
            ControllerType.NotVRTest => Instantiate(_noneVrPlayerController, this.transform),
            ControllerType.VR => Instantiate(_vivePlayerController, this.transform),
            _ => _playerControllerIniter
        };
        if(_playerControllerIniter==null)throw new Exception("PlayerControllerIniter is null");
        _playerControllerIniter?.Init(_mainCamera,this.transform,_inventoryHolder);
    }
}
