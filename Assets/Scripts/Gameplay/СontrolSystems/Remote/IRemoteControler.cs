using System;

public interface IRemoteControler
{
    event Action<IUseble> OnUpButtonHook;
    event Action<IUseble> OnDownButtonHook;
    event Action<IUseble> OnRightButtonCrane;
    event Action<IUseble> OnLeftButtonCrane;
    event Action<IUseble> OnBackButtonCrane;
    event Action<IUseble> OnForwardButtonCrane;
}