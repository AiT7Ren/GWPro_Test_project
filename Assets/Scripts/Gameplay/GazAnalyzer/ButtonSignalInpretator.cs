using System;

public class ButtonSignalInpretator:IDisposable
{
    public event Action OnPowerButtonStopHolding;
    public event Action OnStartHoldPowerButton;
    public event Action OnLeakButtonStopHolding;
    public event Action OnStartHoldLeakButton;
    private bool _lastUpdatePower;
    private bool _lastUpdateLeak;
    private bool _nowPower;
    private bool _nowLeak;
    private RemoteButton _power;
    private RemoteButton _leak;
    public ButtonSignalInpretator(RemoteButton power, RemoteButton leak)
    {
        _power = power;
        _leak = leak;
        SubscripeOnbuttons();
    }

    private void SubscripeOnbuttons()
    {
        _power.OnUsed += GetPowerButton;
        _leak.OnUsed += GetLeakButton;
    }

    void GetPowerButton(IUseble power)
    {
        _nowPower = power.IsActive;
        if(!_lastUpdatePower&&_nowPower)OnStartHoldPowerButton?.Invoke();
        if(_lastUpdatePower&&!_nowPower)OnPowerButtonStopHolding?.Invoke();
        _lastUpdatePower=_nowPower;
    }
    void GetLeakButton(IUseble leak)
    {
        _nowLeak = leak.IsActive;
        if(!_lastUpdateLeak&&_nowLeak)OnStartHoldLeakButton?.Invoke();
        if(_lastUpdateLeak&&!_nowLeak)OnLeakButtonStopHolding?.Invoke();
        _lastUpdateLeak = _nowLeak;
    }

    private void UnSubscripeOnbuttons()
    {
        _power.OnUsed -= GetPowerButton;
        _leak.OnUsed -= GetLeakButton;
    }
    public void Dispose()
    {
        UnSubscripeOnbuttons();
    }
}
