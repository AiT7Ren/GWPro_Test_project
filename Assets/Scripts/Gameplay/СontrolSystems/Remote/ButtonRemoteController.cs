using System;
using UnityEngine;

public class ButtonRemoteController : MonoBehaviour, IRemoteControler
{
    public static ButtonRemoteController Instance;
    
    public event Action<IUseble> OnUpButtonHook;
    public event Action<IUseble> OnDownButtonHook;
    public event Action<IUseble> OnRightButtonCrane;
    public event Action<IUseble> OnLeftButtonCrane;
    public event Action<IUseble> OnBackButtonCrane;
    public event Action<IUseble> OnForwardButtonCrane;
    
    [Header("Remove Buttons")]
    [SerializeField] private RemoteButton _hookDown;
    [SerializeField] private RemoteButton _hookUp; 
    [SerializeField] private RemoteButton _remoteBack;
    [SerializeField] private RemoteButton _remoteForward;
    [SerializeField] private RemoteButton _remoteLeft;
    [SerializeField] private RemoteButton _remoteRight;
    
    private Action<IUseble> _hookDownHandler;
    private Action<IUseble> _hookUpHandler;
    private Action<IUseble> _remoteBackHandler;
    private Action<IUseble> _remoteForwardHandler;
    private Action<IUseble> _remoteLeftHandler;
    private Action<IUseble> _remoteRightHandler;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            Debug.LogError("Another Static Instance of RemoteController is created");
            return;
        }
        ButtonSub();
    }
    private void OnDestroy()
    {
        ButtonUnSub();
    }    
    private void ButtonSub()
    {
        _hookDownHandler = isPressed => OnDownButtonHook?.Invoke(isPressed);
        _hookUpHandler = isPressed => OnUpButtonHook?.Invoke(isPressed);
        _remoteBackHandler = isPressed => OnBackButtonCrane?.Invoke(isPressed);
        _remoteForwardHandler = isPressed => OnForwardButtonCrane?.Invoke(isPressed);
        _remoteLeftHandler = isPressed => OnLeftButtonCrane?.Invoke(isPressed);
        _remoteRightHandler = isPressed => OnRightButtonCrane?.Invoke(isPressed); 
        
        _hookDown.OnUsed += _hookDownHandler; 
        _hookUp.OnUsed += _hookUpHandler; 
        _remoteBack.OnUsed += _remoteBackHandler; 
        _remoteForward.OnUsed += _remoteForwardHandler; 
        _remoteLeft.OnUsed += _remoteLeftHandler; 
        _remoteRight.OnUsed += _remoteRightHandler;
    }
    private void ButtonUnSub()
    { 
        _hookDown.OnUsed -= _hookDownHandler; 
        _hookUp.OnUsed -= _hookUpHandler; 
        _remoteBack.OnUsed -= _remoteBackHandler; 
        _remoteForward.OnUsed -= _remoteForwardHandler; 
        _remoteLeft.OnUsed -= _remoteLeftHandler; 
        _remoteRight.OnUsed -= _remoteRightHandler;
    }
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_hookDown == null) Debug.LogError("HookDown Button is null");
        if (_hookUp == null) Debug.LogError("HookUp Button is null");
        if (_remoteBack == null) Debug.LogError("RemoteBack Button is null");
        if (_remoteForward == null) Debug.LogError("RemoteForward Button is null");
        if (_remoteLeft == null) Debug.LogError("RemoteLeft Button is null");
        if (_remoteRight == null) Debug.LogError("RemoteRight Button is null");
    }
#endif

}