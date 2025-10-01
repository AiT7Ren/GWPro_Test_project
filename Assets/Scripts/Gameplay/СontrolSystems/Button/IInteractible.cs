public interface IInteractible
{
    
    void Use(IUseCallBack useCallBack=null);
    IteractibleType GetInteractiveType();
}

public enum IteractibleType
{
    ClickButton,
    HoldButton,
    PickUp,
    Place
}