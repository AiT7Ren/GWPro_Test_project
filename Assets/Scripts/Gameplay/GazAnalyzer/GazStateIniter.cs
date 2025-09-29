using Gameplay.GazAnalyzer.StateMachine.States;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GazStateIniter : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _perhapsDangerZoneHolder;
    [Header("Display Settings")]
    [SerializeField] private Transform _display;
    [SerializeField]private Color _unused;
    [SerializeField]private Color _used;
    [field:SerializeField,Range(0,2.5f)] private float _displayTimeOnOrOff=0.5f;
    [field:SerializeField,Range(0,5f)] private float _emissionIntensity = 2f;
    [Header("Displays State Root")]
    [SerializeField] private Transform _dangerScaneDisplay;
    [SerializeField] private Transform _dangerDisplay;
    [SerializeField] private Transform _leakDetectionDisplay;
    [Header("Scane State Indicators")]
    [SerializeField] private TextMeshProUGUI _indicatorText; 
    [SerializeField] private RectTransform _indicatorArrow;
    [Header("LeakDetection State Indicators")]
    [SerializeField] private Transform _airCollector;
    [SerializeField] private Image _leakIndiator;
    [Header("Mouse or VR Buttons")]
    [SerializeField] private RemoteButton _poweronButton;
    [SerializeField] private RemoteButton _leakerButton;
    
    
    private IGazAnalyzStateMachine _stateMachine;
    private Material _hashDisplayMaterial;
    
    public void Init(IGazAnalyzStateMachine stateMachine)
    {
        _stateMachine=stateMachine;
        GetComponentToState();
        StateBuilder();
    }

    public IDangerZoneHolder GetDangerZoneHolder()
    {
        if (_stateMachine != null)
        {
            if(_perhapsDangerZoneHolder is IDangerZoneHolder dangerZoneHolder) return dangerZoneHolder; 
        }
        return TakeStaticZoneHolder();
    }

    public RemoteButton GetPowerOnButton()
    {
        return _poweronButton;
    }

    public RemoteButton GetLeakDetectionButton()
    {
        return _leakerButton;
    }

    private IDangerZoneHolder TakeStaticZoneHolder()
    {
        Debug.Log("Try Take Static Danger Zone");
        if (DangerZoneHolder.Instance != null) return DangerZoneHolder.Instance;
        throw new System.NullReferenceException("No DangerZoneHolder found in scene");
    }
    
    public float GetDisplayOnOFDelay()
    {
        return _displayTimeOnOrOff;
    }
    
    private void StateBuilder()
    {
        _stateMachine.AddState(GazStateName.OFF_STATE,new PowerState(_hashDisplayMaterial,_unused,_displayTimeOnOrOff,false));
        _stateMachine.AddState(GazStateName.ON_STATE,new PowerState(_hashDisplayMaterial,_used,_displayTimeOnOrOff,true,_emissionIntensity));
        _stateMachine.AddState(GazStateName.SCANE_STATE,new ScaneDetectorState(_dangerScaneDisplay,_indicatorArrow,_indicatorText));
        _stateMachine.AddState(GazStateName.DANGER_STATE,new DangerGazState(_dangerDisplay)); 
        _stateMachine.AddState(GazStateName.LEAKDET_STATE,new LeakDetectorState(_leakDetectionDisplay,_airCollector,_leakIndiator));
    }
    
    void GetComponentToState()
    {
        _hashDisplayMaterial = _display.GetComponent<Renderer>().material;
    }
    
}
