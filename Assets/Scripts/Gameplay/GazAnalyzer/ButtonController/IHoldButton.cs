using System;

public interface IHoldButton
{
    bool IsActive { get; }
    void Tick(float tick);
    Action OnStateChanged { get; set; }
}