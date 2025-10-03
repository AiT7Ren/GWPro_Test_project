using System; 
using System.Collections.Generic;
using UnityEngine;

public class GazAnalyzStateMachine : IGazAnalyzStateMachine
{
    private Dictionary<string,IGazState> _states = new Dictionary<string, IGazState>();
    private IGazState _currentState;
    public void AddState(string key,IGazState state)
    {
        _states.Add(key, state);
    }
    public void ChangeState(string key)
    {
        if (_states.TryGetValue(key, out var newCurrentState))
        {
            if (_currentState == newCurrentState) return;
            _currentState?.Exit();
            _currentState = newCurrentState;
            _currentState.Enter();
        }
        else
        {
            throw new Exception("State not found in GazAnalyzStateMachine");
        }
    }
    public void Tick(float deltaTime,Transform  transform=null)
    {
        _currentState?.Update(deltaTime,transform);
    }
}
