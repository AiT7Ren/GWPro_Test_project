using UnityEngine;

public interface IInputSystem
{
    void InputUpdateType();
    Vector3 GetMousePosition();
    Vector2 GetMouseDelta();
    Vector2 GetMoveInput();

    bool GetLeftTriggerPressed();
    bool GetRightTriggerPressed();
    bool GetLeftTriggerReleased();
    bool GetRightTriggerReleased();
    
    bool GetLeftMouseButtonPressed();
    bool GetRightMouseButtonPressed();
    bool GetLeftMouseButtonReleased();
    bool GetRightMouseButtonReleased();
    
}
