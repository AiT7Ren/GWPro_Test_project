using System;

public interface IUseble:IInteractible
{
    event Action<IUseble> OnUsed;
    bool IsActive { get; }
    void OffButton();
}