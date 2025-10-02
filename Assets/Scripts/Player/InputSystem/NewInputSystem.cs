using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewInputSystem : IInputSystem,IDisposable
{
    private InputMap _inputActions;

    private Vector3 _mousePosition;
    private Vector2 _mouseInput;
    private Vector2 _moveInput;
    private bool _lButtonDown;
    private bool _rButtonDown;
    private bool _lButtonUp;
    private bool _rButtonUp;
    private bool _m0ButtonDown;
    private bool _m1ButtonDown;
    private bool _m0ButtonUp;
    private bool _m1ButtonUp;

    public NewInputSystem()
    {
        _inputActions = new InputMap();
        _inputActions.Enable();
    }
    public void InputUpdateType()
    {
        _mousePosition = Mouse.current.position.ReadValue();
        _mouseInput = _inputActions.Player.Look.ReadValue<Vector2>();
        _moveInput = _inputActions.Player.Move.ReadValue<Vector2>();
        
        _lButtonDown = _inputActions.Player.LUse.WasPressedThisFrame();
        _rButtonDown = _inputActions.Player.RUse.WasPressedThisFrame();
        _lButtonUp = _inputActions.Player.LUse.WasReleasedThisFrame();
        _rButtonUp = _inputActions.Player.RUse.WasReleasedThisFrame();


        _m0ButtonDown = _inputActions.Player.MainFire.WasPressedThisFrame();
        _m1ButtonDown = _inputActions.Player.SecondaryFire.WasPressedThisFrame();
        _m0ButtonUp = _inputActions.Player.MainFire.WasReleasedThisFrame();
        _m1ButtonUp = _inputActions.Player.SecondaryFire.WasReleasedThisFrame();
    }
    public Vector3 GetMousePosition()
    {
        return _mousePosition;
    }
    public Vector2 GetMouseDelta()
    {
        return _mouseInput;
    }

    public Vector2 GetMoveInput()
    {
        return _moveInput;
    }

    public bool GetLeftTriggerPressed()
    {
        return _lButtonDown;
    }

    public bool GetRightTriggerPressed()
    {
        return _rButtonDown;
    }

    public bool GetLeftTriggerReleased()
    {
        return _lButtonUp;
    }

    public bool GetRightTriggerReleased()
    {
        return _rButtonUp;
    }

    public bool GetLeftMouseButtonPressed()
    {
        return _m0ButtonDown;
    }

    public bool GetRightMouseButtonPressed()
    {
        return _m1ButtonDown;
    }

    public bool GetLeftMouseButtonReleased()
    {
        return _m0ButtonUp;
    }

    public bool GetRightMouseButtonReleased()
    {
        return _m1ButtonUp;
    }
    public void Dispose()
    {
        _inputActions?.Dispose();
    }
}