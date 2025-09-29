using System;

public interface ICraneMoverState
{
    void Tick(float deltaTime);
    void ChangeMoveSpeed(float newSpeed);
    event Action OnTargetDistanation;
    void Stop();
}