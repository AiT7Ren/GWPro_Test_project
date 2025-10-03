using UnityEngine;

public interface IGazAnalyzStateMachine
{
    void AddState(string key,IGazState state);
    void ChangeState(string key);
    void Tick(float deltaTime,Transform  transform=null);
}