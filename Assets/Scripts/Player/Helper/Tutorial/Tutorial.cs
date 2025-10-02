using System;
using System.Collections.Generic;
using TMPro;

using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject _arrowPreFab;
    [SerializeField]private bool _isComplited=false;
    [SerializeField] private TextMeshProUGUI _tuturialText;
    [SerializeField] private TutorialDate _stepDescriptionDate;
    private GameObject _arrow;
    #region Class TutorialStep
    [System.Serializable]
    public class TutorialStep
    {
        public bool active;
        public string WhatNeewToDo;
        public Transform StepHelpObject;
        public bool waitForPlayerAction;
        [HideInInspector]public Action OnEnter;
        [HideInInspector]public Action OnExit;
    }
    #endregion
    public void SetGaz()
    {
        TutorialSteps[0].waitForPlayerAction = true;
        StepUpdate();
    }

    public void SetProbsPiked()
    {
        TutorialSteps[1].waitForPlayerAction = true;
        StepUpdate();
    }

    public void ProbsInHand(bool inHand)
    {
        TutorialSteps[5].waitForPlayerAction = !inHand;
        TutorialSteps[6].waitForPlayerAction = inHand;
        StepUpdate();
    }
    
    public void TryOn(bool isGazPowerOn)
    {
        TutorialSteps[2].waitForPlayerAction=isGazPowerOn;
        StepUpdate();
    }
    public void InDangerZone(bool inDangerZone)
    {
        Debug.Log($"In Danger Zone {inDangerZone}");
        TutorialSteps[3].waitForPlayerAction = inDangerZone;
        TutorialSteps[8].waitForPlayerAction=!inDangerZone;
        StepUpdate();
    }
    public void LeakOn(bool isLeakOn)
    {
        TutorialSteps[4].waitForPlayerAction = isLeakOn;
        TutorialSteps[7].waitForPlayerAction = !isLeakOn;
        StepUpdate();
    }
    
    public List<TutorialStep> TutorialSteps;
    private int _currentStep = -1;

    private void Start()
    { 
        Init();
    }
    
    private void TextUpdate()
    {
        if(_tuturialText==null) return;
        _tuturialText.text = TutorialSteps[_currentStep].WhatNeewToDo;
        _tuturialText.gameObject.SetActive(true);
    }

    private void StepUpdate()
    {
        if(_isComplited)return;
        Debug.Log($"StepUpdate: {_currentStep}");
        if (_currentStep > 2 && !TutorialSteps[2].waitForPlayerAction)
        {
            OffCurrentState();
            _currentStep = 1;
            GoToNewState();
            return;
        }
        if(TutorialSteps[_currentStep].active&&TutorialSteps[_currentStep].waitForPlayerAction)GoToNewState();
    }
    
    private void Init()
    {
        
        TutorialSteps[0].OnEnter= () => ArrowTo(TutorialSteps[0].StepHelpObject);
        TutorialSteps[1].OnEnter= () => ArrowTo(TutorialSteps[1].StepHelpObject);
        TutorialSteps[2].OnEnter = () => ButtonTutorial(TutorialSteps[2].StepHelpObject);
        TutorialSteps[3].OnEnter = TextUpdate;
        TutorialSteps[4].OnEnter = () => ButtonTutorial(TutorialSteps[4].StepHelpObject);
        TutorialSteps[5].OnEnter =()=> ButtonTutorial(TutorialSteps[5].StepHelpObject);
        TutorialSteps[6].OnEnter = TextUpdate;
        TutorialSteps[7].OnEnter = () => ButtonTutorial(TutorialSteps[7].StepHelpObject);
        TutorialSteps[8].OnEnter = TextUpdate;
        
        _arrow = Instantiate(_arrowPreFab);

        foreach (var step in TutorialSteps)
        {
            step.OnExit=(step.StepHelpObject != null)
                ? () => OffToTransform(step.StepHelpObject)
                : () => OffToTransform(null);
        }

        for (int i = 0; i < TutorialSteps.Count; i++)
        {
            TutorialSteps[i].WhatNeewToDo = _stepDescriptionDate.tutorialStep[i];
        }
        GoToNewState();
    }
    
    private void ArrowTo(Transform arrow)
    {
        _arrow.gameObject.SetActive(true);
        _arrow.transform.parent = TutorialSteps[_currentStep].StepHelpObject;
        _arrow.transform.localPosition = Vector3.zero;
        TextUpdate();
    }

    private void ButtonTutorial(Transform button)
    {
        button.gameObject.SetActive(true);
        TextUpdate();
    }

    private void OffToTransform(Transform obj=null)
    {
        _tuturialText?.gameObject.SetActive(false);
        if(!_arrow.gameObject.activeSelf&&obj!=null)obj.gameObject.SetActive(false);
       // if(!_arrow.activeSelf&&TutorialSteps[_currentStep].StepHelpObject!=null)TutorialSteps[_currentStep].StepHelpObject.gameObject.SetActive(false); 
       else _arrow?.SetActive(false);
    }


    void OffCurrentState()
    {
        Debug.Log($"Try Off {_currentStep}");
        if (_currentStep < 0 || _currentStep >= TutorialSteps.Count) return;
        TutorialSteps[_currentStep].active = false;
        TutorialSteps[_currentStep].OnExit?.Invoke();
    }
    private void GoToNewState()
    {
        if(_isComplited)return;
        if (_currentStep >= 0)
        {
            if(TutorialSteps[_currentStep].active)OffCurrentState();
            if (_currentStep == TutorialSteps.Count - 1) _isComplited = true;
        }
        _currentStep++;
        if (_currentStep >= 0 && _currentStep < TutorialSteps.Count)
        {
            TutorialSteps[_currentStep].active=true;
            TutorialSteps[_currentStep].OnEnter?.Invoke();
            if (TutorialSteps[_currentStep].waitForPlayerAction) GoToNewState();
        }
        
    }
}