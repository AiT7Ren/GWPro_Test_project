using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public static Tutorial Instance;
    [SerializeField] private GameObject _arrowPreFab;
    private GameObject _arrow;
    [SerializeField] private TextMeshProUGUI _tuturialText;

    [Serializable]
    public class TutorialStep
    {
        public bool active;
        public string WhatNeewToDo;
        public Transform ArrowPosition;
        public bool waitForPlayerAction;
    }

    public List<TutorialStep> TutorialSteps;
    private int i = -1;
    [SerializeField] private bool _complite;
    private bool _initialized;

    private void Start()
    { 
        if (Instance == null) Instance = this;
        else throw new Exception("Tutorial Already Have");
        Init();
    }

    private void Init()
    {
       
        TutorialSteps[0].WhatNeewToDo = "PICK UP \n GazAnatizator \n (RTrigger for VR / E for Keyboard)";
        TutorialSteps[1].WhatNeewToDo = "PICK UP \n Sensor \n (LTrigger for VR / Q for Keyboard)";
        TutorialSteps[2].WhatNeewToDo = "PowerOn GazAnatizator \n (HoldButton(AnyTrigger) for VR / Hold M1 for Keyboard/HoldButton E for KeyboardVRSimular)";
        TutorialSteps[3].WhatNeewToDo = "Move to the Danger Zone";
        TutorialSteps[4].WhatNeewToDo = "PowerOn Seansor \n (HoldButton(AnyTrigger) for VR / Hold M0 for Keyboard/HoldButton E for KeyboardVRSimular)";
        TutorialSteps[5].WhatNeewToDo = "Attach Seansor to Pipe \n (LTrigger) for VR / Q for Keyboard";
        TutorialSteps[6].WhatNeewToDo = "Return Seansor to Hand \n (LTrigger) for VR / Q for Keyboard";
        TutorialSteps[7].WhatNeewToDo = "Go out of the Danger Zone";
        
        _arrow = Instantiate(_arrowPreFab);
        _initialized = true;
        GoToNewState(true);
    }

    private void Update()
    {
        if (_complite && _tuturialText.gameObject.activeInHierarchy)
        {
            i = 7;
            GoToNewState();
        }
        if (_complite||!_initialized) return;
        if (i < TutorialSteps.Count&&i>-1)
        {
            ArrowHollow();
            if (TutorialSteps[i].waitForPlayerAction)
            {
                GoToNewState();
               
            }
        }
    }
    private void ArrowHollow()
    {
        if (TutorialSteps[i].ArrowPosition == null)
        {
            _arrow.gameObject.SetActive(false);
            return;
        }
        _arrow.gameObject.SetActive(true);
        _arrow.transform.position = TutorialSteps[i].ArrowPosition.position;
    }

    private void GoToNewState(bool isFirst = false)
    {
        if (i >= TutorialSteps.Count-1)
        {
            _complite = true;
            Debug.Log($"Tutorial Done Step Count: {TutorialSteps.Count}");
            _tuturialText.gameObject.SetActive(false);
            _arrow.SetActive(false);
            return;
        }
        if (i < TutorialSteps.Count)
        {
            if (!isFirst) TutorialSteps[i].active = false;
            i++;
            TutorialSteps[i].active = true;
            _tuturialText.text = TutorialSteps[i].WhatNeewToDo;
        }
        
    }
}