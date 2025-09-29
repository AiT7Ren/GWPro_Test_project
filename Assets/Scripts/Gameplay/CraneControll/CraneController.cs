using System;
using Helpers;
using UnityEngine;

public class CraneController : MonoBehaviour
{
    [SerializeField] private CraneIniter _craneIniter;
    
    private ICraneMoverState _currentMoverState;
    private IUseble _currentUseble;
    private IRemoteControler _remoteController;
    private ICraneMoverStateMachine _stateMachine;
    [Header("Speed Settings")]
    [field:SerializeField,Range(0.1f,25)]private float _backFrontSpeed=3f;
    [field:SerializeField,Range(0.1f,25)]private float _leftRightSpeed=2f;
    
    private float _hookUoDownSpeed=2f;
    private IFloatReactProperty _bfSpeed; 
    private IFloatReactProperty _lrSpeed;
    
    private CraneSubscripeHolder _craneSubscripeHolder;
    
    public void ChangeLrSpeed(float newLrSpeed)
    {
        _lrSpeed.CurrentSpeed=newLrSpeed;
    }
    public void ChangeFbSpeed(float newFbSpeed)
    {
        _bfSpeed.CurrentSpeed=newFbSpeed;
    }
    private void OnDestroy()
    {
        (_stateMachine as IDisposable)?.Dispose();
        _craneSubscripeHolder?.Dispose();
    }
    private void Start()
    {
        _stateMachine = new CraneStateMachine();
        _lrSpeed = new FloatReactProperty(_backFrontSpeed);
        _bfSpeed = new FloatReactProperty(_leftRightSpeed);
        if (_craneIniter == null) _craneIniter = GetComponent<CraneIniter>();
        _craneIniter.Init(_stateMachine,_hookUoDownSpeed,_lrSpeed.CurrentSpeed,_bfSpeed.CurrentSpeed);
        _remoteController = _craneIniter.GetRemoteController();
        _craneSubscripeHolder=new CraneSubscripeHolder(_stateMachine,_remoteController,_bfSpeed,_lrSpeed);
    }
    public void ChangeRemove(IRemoteControler newRemoteControler)
    {
        _craneSubscripeHolder.SetNewRemoteControler(newRemoteControler);
        _remoteController = newRemoteControler;
    }
    private void Update()
    {
        _stateMachine.Tick(Time.deltaTime);
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        if(_bfSpeed!=null)_bfSpeed.CurrentSpeed=_backFrontSpeed;
        if(_lrSpeed!=null)_lrSpeed.CurrentSpeed=_leftRightSpeed;
    }
#endif
    
}