using System;
using System.Collections.Generic;
using UnityEngine;

public class CraneStateMachine : ICraneMoverStateMachine,IDisposable
{
    private readonly Dictionary<string, ICraneMoverState> _moveStates;
    private ICraneMoverState _currentMoverState;
    private IUseble _currentUseble;

    public CraneStateMachine()
    {
        _moveStates = new Dictionary<string, ICraneMoverState>(6);
    }
    public void AddState(string key, ICraneMoverState moverState)
    {
        _moveStates.Add(key, moverState);
        _moveStates[key].OnTargetDistanation += StopCurrentState;
    }
    public void EnterState(string key, IUseble useble)
    {
        Debug.Log("Entering state: " + key);
        StopCurrentState();
        if (!_moveStates.TryGetValue(key, out ICraneMoverState newMover) || !useble.IsActive) return;
        _currentMoverState = newMover;
        _currentUseble = useble;
    }
    public void Tick(float deltaTime)
    {
        Debug.Log($"tick to {_currentMoverState}");
        _currentMoverState?.Tick(deltaTime);
    }

    public ICraneMoverState GetCurrentMover(string key)
    {
        if (_moveStates.TryGetValue(key, out ICraneMoverState mover)) return mover;
        throw new Exception("No mover found with the key " + key);
    }

    public void StopCurrentState()
    {
        _currentMoverState?.Stop();
        _currentMoverState = null;
        _currentUseble?.OffButton();
        _currentUseble = null;
    }
    public void Dispose()
    {
        foreach (var state in _moveStates)
        {
            state.Value.OnTargetDistanation -= StopCurrentState;
        }
    }
}
