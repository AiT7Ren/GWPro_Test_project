using UnityEngine;

public interface IPlayerControllerIniter
{
    void Init(Camera cam, Transform playerRoot, Inventory inventory = null, СontextHints contextHints = null);
}