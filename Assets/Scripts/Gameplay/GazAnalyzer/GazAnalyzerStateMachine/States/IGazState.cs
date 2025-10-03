using UnityEngine;
public interface IGazState
{
    void Enter();
    void Exit();
    void Update(float deltaTime,Transform transform=null);
}
