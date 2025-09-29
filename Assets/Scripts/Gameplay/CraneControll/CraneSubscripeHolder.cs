using System;
using System.Collections.Generic;
using Helpers;

public class CraneSubscripeHolder : IDisposable
{
    private readonly ICraneMoverStateMachine _stateMachine;
    private IRemoteControler _remoteController;
    private readonly IFloatReactProperty _bFSpeed;
    private readonly IFloatReactProperty _lRSpeed;

    private readonly Dictionary<string, Action<IUseble>> _buttonHandlers;
    private readonly Dictionary<string, Action<float>> _speedHandlers;
    
    public CraneSubscripeHolder(ICraneMoverStateMachine stateMachine, IRemoteControler remoteController, IFloatReactProperty bFSpeed, IFloatReactProperty lRSpeed)
    {
        _stateMachine = stateMachine;
        _remoteController = remoteController;
        _bFSpeed = bFSpeed;
        _lRSpeed = lRSpeed;
        _buttonHandlers=new Dictionary<string, Action<IUseble>>();
        _speedHandlers=new Dictionary<string, Action<float>>();
        if(_stateMachine==null||_remoteController==null||_bFSpeed==null||_lRSpeed==null) throw new NullReferenceException("SubscripeHolder NullRef");
        InitHash();
        ButtonSubscripe();
        SpeedChangeSubscripe();
    }
    private void InitHash()
    {
        _buttonHandlers[CraneStateName.HOOK_DOWN] = button => _stateMachine.EnterState(CraneStateName.HOOK_DOWN, button);
        _buttonHandlers[CraneStateName.HOOK_UP] = button => _stateMachine.EnterState(CraneStateName.HOOK_UP, button);
        _buttonHandlers[CraneStateName.CRANE_FORWARD] = button => _stateMachine.EnterState(CraneStateName.CRANE_FORWARD, button);
        _buttonHandlers[CraneStateName.CRANE_BACKWARD] = button => _stateMachine.EnterState(CraneStateName.CRANE_BACKWARD, button);
        _buttonHandlers[CraneStateName.CRANE_RIGHT] = button => _stateMachine.EnterState(CraneStateName.CRANE_RIGHT, button);
        _buttonHandlers[CraneStateName.CRANE_LEFT] = button => _stateMachine.EnterState(CraneStateName.CRANE_LEFT, button);
        
        _speedHandlers[CraneStateName.CRANE_LEFT] = speed => _stateMachine.GetCurrentMover(CraneStateName.CRANE_LEFT).ChangeMoveSpeed(speed);
        _speedHandlers[CraneStateName.CRANE_RIGHT] = speed => _stateMachine.GetCurrentMover(CraneStateName.CRANE_RIGHT).ChangeMoveSpeed(speed);
        _speedHandlers[CraneStateName.CRANE_BACKWARD] = speed => _stateMachine.GetCurrentMover(CraneStateName.CRANE_BACKWARD).ChangeMoveSpeed(speed);
        _speedHandlers[CraneStateName.CRANE_FORWARD] = speed => _stateMachine.GetCurrentMover(CraneStateName.CRANE_FORWARD).ChangeMoveSpeed(speed);
    }
    public void SetNewRemoteControler(IRemoteControler newRemotecontroller)
    {
        ButtonUnsubscripe();
        _remoteController = newRemotecontroller;
        ButtonSubscripe();
    }
    public void Dispose()
    {
        ButtonUnsubscripe();
        SpeedChangeUnsubscripe();
    }
    private void ButtonSubscripe()
    {
        _remoteController.OnDownButtonHook += _buttonHandlers[CraneStateName.HOOK_DOWN];
        _remoteController.OnUpButtonHook += _buttonHandlers[CraneStateName.HOOK_UP];
        _remoteController.OnForwardButtonCrane += _buttonHandlers[CraneStateName.CRANE_FORWARD];
        _remoteController.OnBackButtonCrane += _buttonHandlers[CraneStateName.CRANE_BACKWARD];
        _remoteController.OnRightButtonCrane += _buttonHandlers[CraneStateName.CRANE_RIGHT];
        _remoteController.OnLeftButtonCrane += _buttonHandlers[CraneStateName.CRANE_LEFT];
    }
    private void SpeedChangeSubscripe()
    {
        _lRSpeed.OnSpeedChange += _speedHandlers[CraneStateName.CRANE_LEFT];
        _lRSpeed.OnSpeedChange += _speedHandlers[CraneStateName.CRANE_RIGHT];
        _bFSpeed.OnSpeedChange += _speedHandlers[CraneStateName.CRANE_BACKWARD];
        _bFSpeed.OnSpeedChange += _speedHandlers[CraneStateName.CRANE_FORWARD];
    }
    private void ButtonUnsubscripe()
    {
        _remoteController.OnDownButtonHook -= _buttonHandlers[CraneStateName.HOOK_DOWN];
        _remoteController.OnUpButtonHook -= _buttonHandlers[CraneStateName.HOOK_UP];
        _remoteController.OnForwardButtonCrane -= _buttonHandlers[CraneStateName.CRANE_FORWARD];
        _remoteController.OnBackButtonCrane -= _buttonHandlers[CraneStateName.CRANE_BACKWARD];
        _remoteController.OnRightButtonCrane -= _buttonHandlers[CraneStateName.CRANE_RIGHT];
        _remoteController.OnLeftButtonCrane -= _buttonHandlers[CraneStateName.CRANE_LEFT];
    }
    private void SpeedChangeUnsubscripe()
    {
        _lRSpeed.OnSpeedChange -= _speedHandlers[CraneStateName.CRANE_LEFT];
        _lRSpeed.OnSpeedChange -= _speedHandlers[CraneStateName.CRANE_RIGHT];
        _bFSpeed.OnSpeedChange -= _speedHandlers[CraneStateName.CRANE_BACKWARD];
        _bFSpeed.OnSpeedChange -= _speedHandlers[CraneStateName.CRANE_FORWARD];
    }
}