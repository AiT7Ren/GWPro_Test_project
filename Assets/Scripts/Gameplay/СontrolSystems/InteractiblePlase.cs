using UnityEngine;

public class InteractiblePlase : MonoBehaviour,IInteractible
{
    public void Use(IUseCallBack useCallBack)
    {
        
    }

    public IteractibleType GetInteractiveType()
    {
        return IteractibleType.Place;
    }
}
