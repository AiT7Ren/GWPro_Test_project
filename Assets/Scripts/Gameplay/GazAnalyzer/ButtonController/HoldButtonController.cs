using System;

public class HoldButtonController:IHoldButton,IDisposable
{
    public Action OnStateChanged { get; set; }
    public bool IsActive { get; private set; }
    
    private readonly float _timeToUse;
    private readonly HoldIndicator _buttonIndicator;
    private readonly HoldButtonInpretator _holdButtonInpretator;
    
    private float _currentHoldTime; 
    private bool _isHolding;
    
    public HoldButtonController(float timeToUse,HoldIndicator buttonIndicator,RemoteButton remoteButton)
    {
        _timeToUse = timeToUse;
        _buttonIndicator = buttonIndicator;
        _holdButtonInpretator = new HoldButtonInpretator(remoteButton);
        SubscripeHolder();
    }
    private void SubscripeHolder()
    {
        _holdButtonInpretator.OnStartHolding += StartHold;
        _holdButtonInpretator.OnStopHolding += StopHold;
    }
    private void StartHold()
    {
        _currentHoldTime = 0;
        Holding(_currentHoldTime);
        _buttonIndicator.gameObject.SetActive(true);
        _isHolding = true;
    }
    private void StopHold()
    { 
        _isHolding = false;
        _buttonIndicator.gameObject.SetActive(false);
    }
    public void Tick(float tick)
    {
        if (!_isHolding) return;
        _currentHoldTime += tick;
        Holding(_currentHoldTime / _timeToUse);
        if (_currentHoldTime >= _timeToUse)
        {
            HoldComplete();
            StopHold();
        }
    }
    private void Holding(float normalizedProgress)
    {
        _buttonIndicator.SetAmount(normalizedProgress);
    }
    private void HoldComplete()
    {
        IsActive=!IsActive;
        OnStateChanged?.Invoke();
    }

    public void Dispose()
    {
        _holdButtonInpretator?.Dispose();
    }
}



