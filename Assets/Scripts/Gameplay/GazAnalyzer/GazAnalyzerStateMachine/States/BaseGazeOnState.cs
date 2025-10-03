using UnityEngine;

public abstract class BaseGazeOnState:IGazState
{ 
    protected Transform _dispaly;
    public BaseGazeOnState(Transform display)
    {
        _dispaly = display;
    }
    public void Enter()
    {
        _dispaly.gameObject.SetActive(true);
    }

    public void Exit()
    {
        _dispaly.gameObject.SetActive(false);
    }

    public virtual void Update(float deltaTime, Transform transform = null)
    {
    }
}
