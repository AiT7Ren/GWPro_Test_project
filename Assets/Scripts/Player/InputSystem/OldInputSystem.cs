using UnityEngine;

public class OldInputSystem : IInputSystem
{
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

    public void InputUpdateType()
    {
        _mousePosition = Input.mousePosition;
        
        _mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        _moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        
        _lButtonDown = Input.GetKeyDown(KeyCode.Q);
        _rButtonDown = Input.GetKeyDown(KeyCode.E);
        _lButtonUp = Input.GetKeyUp(KeyCode.Q);
        _rButtonUp = Input.GetKeyUp(KeyCode.E);
        
        _m0ButtonDown = Input.GetKeyDown(KeyCode.Mouse0);
        _m1ButtonDown = Input.GetKeyDown(KeyCode.Mouse1);
        _m0ButtonUp = Input.GetKeyUp(KeyCode.Mouse0);
        _m1ButtonUp = Input.GetKeyUp(KeyCode.Mouse1);
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
}