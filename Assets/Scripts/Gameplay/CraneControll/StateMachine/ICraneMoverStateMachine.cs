public interface ICraneMoverStateMachine
{
    void AddState(string key, ICraneMoverState moverState);
    void EnterState(string key, IUseble useble);
    ICraneMoverState GetCurrentMover(string key);
    void Tick(float deltaTime);
}