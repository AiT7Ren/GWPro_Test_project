using System;

public class HoldButtonInpretator:IDisposable
{
    public event Action OnStopHolding;
    public event Action OnStartHolding;
    private bool _lastUpdate;
    private bool _thisUpdate ;
    private RemoteButton _remoteButton;
    public HoldButtonInpretator(RemoteButton remoteButton)
    {
        _remoteButton = remoteButton;
        SubscripeOnbuttons();
    }
    private void SubscripeOnbuttons()
    {
        _remoteButton.OnUsed += GetRemoteButtonButton;
    }

    void GetRemoteButtonButton(IUseble power)
    {
        _thisUpdate = power.IsActive;
        if(!_lastUpdate&&_thisUpdate)OnStartHolding?.Invoke();
        if(_lastUpdate&&!_thisUpdate)OnStopHolding?.Invoke();
        _lastUpdate=_thisUpdate;
    }
    private void UnSubscripeOnbuttons()
    {
        _remoteButton.OnUsed -= GetRemoteButtonButton;
    }
    public void Dispose()
    {
        UnSubscripeOnbuttons();
    }
}
