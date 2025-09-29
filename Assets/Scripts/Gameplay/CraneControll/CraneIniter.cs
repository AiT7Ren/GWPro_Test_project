using System;
using UnityEngine;
using Gameplay.CraneControll.StateMachine.States.Upd;

public class CraneIniter : MonoBehaviour
{
    [Header("Remove")]
    [SerializeField] MonoBehaviour _remoteController;
    [Header("Crane parts to move")]
    [SerializeField] private Transform _hook;
    [SerializeField] private Transform _beamHolder;
    [SerializeField] private Transform _craneBase;
    [Header("Crane parts rotate and settings")]
    [SerializeField] private Transform _craneCoil;
    [field:SerializeField,Range(0f,720f)] private float _coilRotateSpeed;
    [Header("Move Ankers")]
    [SerializeField] private Transform _uLFMoveClamp;
    [SerializeField] private Transform _dRBMoveClamp;
    private ICraneMoverStateMachine _stateMachine;

    public void Init(ICraneMoverStateMachine  stateMachine,float hookSpeed,float lrSpeed,float fbSpeed)
    {
        _stateMachine = stateMachine;
        CreateStates(hookSpeed, lrSpeed, fbSpeed);
    }

    public IRemoteControler GetRemoteController()
    {
        if(_remoteController == null)GetStaticController();
        if(_remoteController is IRemoteControler controler) return controler;
        throw new Exception("Remote controller is not IRemoteControler");
    }

    private void GetStaticController()
    {
        Debug.Log("Try GetStaticController");
        _remoteController = ButtonRemoteController.Instance; 
        if (_remoteController == null) throw new Exception("no any RemoteController");
    }
    
    private void CreateStates(float hookSpeed, float lrSpeed, float fbSpeed)
    {
        _stateMachine.AddState(CraneStateName.HOOK_UP,
            new HookMoverY(_hook, 
                _uLFMoveClamp.position,hookSpeed,
                hookCoilHolder:_craneCoil,rotateSpeed:_coilRotateSpeed));
        _stateMachine.AddState(CraneStateName.HOOK_DOWN,
            new HookMoverY(_hook, 
                _dRBMoveClamp.position,hookSpeed,
                hookCoilHolder:_craneCoil,rotateSpeed:-_coilRotateSpeed));
        _stateMachine.AddState(CraneStateName.CRANE_LEFT,
            new CraneMoverX(_craneBase,
                _uLFMoveClamp.position,
                lrSpeed));
        _stateMachine.AddState(CraneStateName.CRANE_RIGHT,
            new CraneMoverX(_craneBase,
                _dRBMoveClamp.position,
                lrSpeed));
        _stateMachine.AddState(CraneStateName.CRANE_FORWARD,
            new BeamMoverZ(_beamHolder,
                _dRBMoveClamp.position,
                fbSpeed));
        _stateMachine.AddState(CraneStateName.CRANE_BACKWARD,
            new BeamMoverZ(_beamHolder,
                _uLFMoveClamp.position,
                fbSpeed));
    }
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if(_hook==null)Debug.LogError("Crane Hook is null");
        if(_beamHolder==null)Debug.LogError("Beam Holder is null");
        if(_craneBase==null)Debug.LogError("Crane Base is null");
        
        if(!gameObject.activeInHierarchy) return;
        if(_dRBMoveClamp==null)Debug.LogError("Down Right Back Anker is null");
        if(_uLFMoveClamp==null)Debug.LogError("Top Left Front Anker is null");
    }
#endif
    
    
}
