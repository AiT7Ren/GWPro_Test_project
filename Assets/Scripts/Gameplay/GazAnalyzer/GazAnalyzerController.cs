using UnityEngine;
using UnityEngine.UI;
using Helpers;
using UnityEngine.Events;


public class GazAnalyzerController : MonoBehaviour
{
  public FloatUnityEvent OnSetProbsPower;
  
  public UnityEvent OnOffState; //для разнообразия
  public StringUnityEvent OnOnState;
  public StringUnityEvent OnDangerState;
  public StringUnityEvent OnScaneState;
  public StringUnityEvent OnLeakFindState;
  
  [SerializeField] private Image _powerChangeIndicator;
  [SerializeField]GazStateIniter _gazAnalyzIniter;
  private float _currentHoldAnalizator;
  private float _currentHoldProbs;
  private bool _isActive;
  private bool _isPowerOn;
  private bool _isInDanger;
  private bool _isInLeakDetection;
  
  private IGazAnalyzStateMachine _stateMachine;
  private IDangerZoneHolder _dangerZoneHolder;
  private Transform _lastFoundDangerZone;
  private const float HOLDER_TO_POWER=3f;

  private bool _leakButtonHold;
  private bool _powerButtonHold;
  private ButtonSignalInpretator _buttonSignalInpretator;
  public void PowerStartHold()
  {
    _powerButtonHold=true;
  }
  public void PowerStopHold()
  {
    PowerButtonControll(0, true);
  }
  public void LeakStartHold()
  {
    _leakButtonHold=true;
  }
  public void LeakStopHold()
  {
    LeakButtonControll(0, true);
  }
  
  public void SetProbsEnable(bool active)
  {
    _isInLeakDetection=active;
    if(_isInLeakDetection)_gazAnalyzIniter.GetLeakDetectionButton().OnButton();
    else _gazAnalyzIniter.GetLeakDetectionButton().OffButton();
    GazBehaviour();
  }
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
    float tick=Time.deltaTime;
    _stateMachine.Tick( tick,_lastFoundDangerZone);
    if (_powerButtonHold) PowerButtonControll(tick);
    if(_leakButtonHold) LeakButtonControll(tick);
    
  }
  private void LeakButtonControll(float tick,bool stopHold=false)
  {
    if (stopHold)
    {
      _currentHoldProbs=0;
      OnSetProbsPower?.Invoke(0f);
      _leakButtonHold=false;
      return;
    }
    _currentHoldProbs += tick;
    OnSetProbsPower?.Invoke(_currentHoldProbs/HOLDER_TO_POWER);
  }
  private void PowerButtonControll(float tick, bool stopHold = false)
  {
    if (stopHold)
    {
      _currentHoldAnalizator = 0;
      _powerChangeIndicator.gameObject.SetActive(false);
      _powerButtonHold=false;
      return;
    }
    if(_currentHoldAnalizator <= HOLDER_TO_POWER) TryOnorOff(tick);
  }
  private void Init()
  {
    OnSetProbsPower?.Invoke(0f); 
    _stateMachine = new GazAnalyzStateMachine();
    if (_gazAnalyzIniter == null) _gazAnalyzIniter=GetComponent<GazStateIniter>();
    _gazAnalyzIniter.Init(_stateMachine);
    _stateMachine.ChangeState(GazStateName.OFF_STATE);
    _dangerZoneHolder = _gazAnalyzIniter.GetDangerZoneHolder();
    _dangerZoneHolder.OnDangerZone += DangerState;
    _buttonSignalInpretator = 
      new ButtonSignalInpretator(_gazAnalyzIniter.GetPowerOnButton(), _gazAnalyzIniter.GetLeakDetectionButton());
    ButtonControllSub();
  }

  private void GazBehaviour(bool tryOn = false)
  {
    if (!_isActive)
    {
      _isPowerOn = false;
      if(_gazAnalyzIniter.GetPowerOnButton().IsActive)_gazAnalyzIniter.GetPowerOnButton().OffButton();
      TryUseEvent(OnOffState);
      return;
    }

    if (tryOn && !_isPowerOn)
    {
      Invoke(nameof(DevicePowerOn), _gazAnalyzIniter.GetDisplayOnOFDelay());
      TryUseEvent(OnOnState, GazStateName.ON_STATE);
      return;
    }
    if (!_isPowerOn) return;
    FindCloserDangerZone();
    if (_isInLeakDetection)
    {
      TryUseEvent(OnLeakFindState, GazStateName.LEAKDET_STATE);
      if (Tutorial.Instance.TutorialSteps[4].active) Tutorial.Instance.TutorialSteps[4].waitForPlayerAction = true;
      return;
    }
    if (_isInDanger)
    {
      TryUseEvent(OnDangerState, GazStateName.DANGER_STATE);
      return;
    }

    TryUseEvent(OnScaneState, GazStateName.SCANE_STATE);
  }

  private void TryUseEvent(UnityEvent eventMetod)
  {
    Debug.Log($"WARNING Not hashMetod");
    eventMetod?.Invoke();
  }

  private void TryUseEvent(StringUnityEvent eventMetod, string par)
  {
    if(eventMetod.GetPersistentEventCount() > 0) eventMetod.Invoke(par);
    else
    {
      Debug.Log($"{par} used from code");
      _stateMachine.ChangeState(par);
    }
  }
  private void FindCloserDangerZone()
  {
    _lastFoundDangerZone = _dangerZoneHolder.GetNearlyZone(this.transform);
  }
  private void DevicePowerOn()
  {
    _isPowerOn = true;
    if(!_gazAnalyzIniter.GetPowerOnButton().IsActive)_gazAnalyzIniter.GetPowerOnButton().OnButton();
    if (Tutorial.Instance.TutorialSteps[2].active) Tutorial.Instance.TutorialSteps[2].waitForPlayerAction = true;
    GazBehaviour();
  }
  private void DangerState(bool isActive)
  {
    _isInDanger = isActive;
    if (Tutorial.Instance.TutorialSteps[3].active) Tutorial.Instance.TutorialSteps[3].waitForPlayerAction = _isInDanger;
    if (Tutorial.Instance.TutorialSteps[7].active) Tutorial.Instance.TutorialSteps[7].waitForPlayerAction = !_isInDanger;
    GazBehaviour();
  }
  private void TryOnorOff(float delta)
  {
    _currentHoldAnalizator+=delta;
    if (_currentHoldAnalizator >= HOLDER_TO_POWER)
    {
      _isActive = !_isActive;
      if(_isActive) GazBehaviour(true);
      else GazBehaviour();
      _powerChangeIndicator.gameObject.SetActive(false);
      return;
    }
    _powerChangeIndicator.gameObject.SetActive(true);
    _powerChangeIndicator.fillAmount = _currentHoldAnalizator / HOLDER_TO_POWER;
  }

  private void ButtonControllSub()
  {
    _buttonSignalInpretator.OnStartHoldPowerButton += PowerStartHold;
    _buttonSignalInpretator.OnStartHoldLeakButton += LeakStartHold;
    _buttonSignalInpretator.OnPowerButtonStopHolding+= PowerStopHold;
    _buttonSignalInpretator.OnLeakButtonStopHolding+= LeakStopHold;
  }
  private void ButtonControllUnsub()
  {
    _buttonSignalInpretator.OnStartHoldPowerButton -= PowerStartHold;
    _buttonSignalInpretator.OnStartHoldLeakButton -= LeakStartHold;
    _buttonSignalInpretator.OnPowerButtonStopHolding-= PowerStopHold;
    _buttonSignalInpretator.OnLeakButtonStopHolding-= LeakStopHold;
  }
  
  private void OnDestroy()
  {
    if(_dangerZoneHolder!=null)_dangerZoneHolder.OnDangerZone -= DangerState;
    ButtonControllUnsub();
    _buttonSignalInpretator?.Dispose();
  }
}





