using UnityEngine;
using Helpers;

public class GazAnalyzerController : MonoBehaviour
{
  
  public BoolUnityEvent OnSetSencorPower;
  
  public StringUnityEvent OnOffState;
  public StringUnityEvent OnOnState;
  public StringUnityEvent OnDangerState;
  public StringUnityEvent OnScaneState;
  public StringUnityEvent OnLeakFindState;
  
  [SerializeField] private GazStateIniter _gazAnalyzIniter;
  
  private IGazStateManager _stateManager;
  private IGazAnalyzStateMachine _stateMachine;
  
  private bool _isInDanger;
  private IDangerZoneHolder _dangerZoneHolder;
  private Transform _lastFoundDangerZone;
  private bool _isInLeakDetection;
  
  private IHoldButton _powerButton;
  private IHoldButton _leakButton;
  
  public void ChangeState(string par)
  {
    _stateMachine.ChangeState(par);
  }
  private void Start()
  {
    Init();
  }
  
  private void Update()
  {
    ClassesUpdate();
  }
  private void ClassesUpdate()
  {
    float tick=Time.deltaTime;
    _stateMachine.Tick( tick,_lastFoundDangerZone);
    _powerButton.Tick(tick);
    _leakButton.Tick(tick);
  }
  private void Init()
  {
    OnSetSencorPower?.Invoke(false);
    _stateMachine = new GazAnalyzStateMachine();
    if (_gazAnalyzIniter == null) _gazAnalyzIniter = GetComponent<GazStateIniter>();
    _gazAnalyzIniter.Init(_stateMachine);
    _stateMachine.ChangeState(GazStateName.OFF_STATE);
    _dangerZoneHolder = _gazAnalyzIniter.GetDangerZoneHolder();
    _dangerZoneHolder.OnDangerZone += DangerState;
    _stateManager = new GazStateManager(_stateMachine, OnOffState, OnOnState,
                                        OnDangerState, OnScaneState, OnLeakFindState,
                                        _gazAnalyzIniter.GetDisplayOnOFDelay()
    );
    _powerButton = _gazAnalyzIniter.GetPowerOnButton();
    _leakButton=_gazAnalyzIniter.GetLeakDetectionButton();
    Subscripe();
  }

  private void Subscripe()
  {
    _powerButton.OnStateChanged += UpdateGazBehaviour;
    _leakButton.OnStateChanged += UpdateGazBehaviour;
    _leakButton.OnStateChanged += () => OnSetSencorPower?.Invoke(_leakButton.IsActive);
  }
  
  private void UpdateGazBehaviour()
  {
    
    FindCloserDangerZone();
    GazAnalyzerState state = new GazAnalyzerState(
      _powerButton.IsActive,
      _isInDanger, _leakButton.IsActive
    );
    _stateManager.TryChangeState(state);
  }
  private void FindCloserDangerZone()
  {
    _lastFoundDangerZone = _dangerZoneHolder.GetNearlyZone(this.transform);
  }
  private void DangerState(bool isActive)
  {
    _isInDanger = isActive;
    UpdateGazBehaviour();
  }
  private void OnDestroy()
  {
    if(_dangerZoneHolder!=null)_dangerZoneHolder.OnDangerZone -= DangerState;
    UNsubscripe();
  }
  
  private void UNsubscripe()
  {
    _powerButton.OnStateChanged -= UpdateGazBehaviour;
    _leakButton.OnStateChanged -= UpdateGazBehaviour;
    _leakButton.OnStateChanged -= () => OnSetSencorPower?.Invoke(_leakButton.IsActive);
  }
  
}





