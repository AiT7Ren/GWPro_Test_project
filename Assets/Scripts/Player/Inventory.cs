using UnityEngine;
using UnityEngine.Events;
using Helpers;

public class Inventory : MonoBehaviour
{
    public UnityEvent OnPickGaz;
    public UnityEvent OnPickProbs;
    public BoolUnityEvent OnProbsInHand;
    [SerializeField] private Transform _rHand;
    [SerializeField]private GameObject ItemFromRightHand;
    [SerializeField] private Transform _lHand;
    [SerializeField]private GameObject ItemFromLeftHand;
    [SerializeField] private Transform _wire;
    public bool IsProbsPlace = false;
    [SerializeField] private RemoteButton _onPowerButton;
    [SerializeField] private RemoteButton _onLeakButton;


    public void SetPowerActive(bool active)
    {
        Debug.Log($"TRY POWER: {active}");
      //  if(!IfItemOnHand(false)) return;
        if(active)_onPowerButton.Use();
        else _onPowerButton.StopUse();
    }
    public void SetLeakActive(bool active)
    {
        Debug.Log($"TRY LEAK: {active}");
       // if(!IfItemOnHand(false)) return;
        if(active)_onLeakButton.Use();
        else _onLeakButton.StopUse();
    }
    
    public bool IfItemOnHand(bool isLeft)
    {
        if (isLeft) return _lHand.transform.childCount > 0; 
        return _rHand.transform.childCount > 0;
    }

    public bool HaveObject(bool isLeft)
    {
        if(isLeft)return ItemFromLeftHand!=null;
        return ItemFromRightHand!=null;
    }

    public void EqipToRightHand(GameObject item)
    {
        if (item != null) ItemFromRightHand = ItemAttendTo(item, _rHand); 
        OnPickGaz?.Invoke();
    }
    public void EqipToLeftHand(GameObject item)
    {
        if (item != null)
        {
            ItemFromLeftHand = ItemAttendTo(item, _lHand);
            IsProbsPlace = false;
            OnPickProbs?.Invoke();
        }
    }

    public void VRHandInventory(Vector3 leftHandPos, Vector3 rightHandPos)
    {
        if(ItemFromLeftHand!=null)_lHand.position = leftHandPos;
        if(ItemFromRightHand!=null)_rHand.position = rightHandPos;
    }

    public void ReleasePropsObjTo(Vector3 position)
    {
        if(ItemFromLeftHand==null) return;
        Debug.Log($"{ItemFromLeftHand.name} - {IsProbsPlace}");
        if(IsProbsPlace) return;
        _wire.gameObject.SetActive(true);
        ItemFromLeftHand.transform.parent = null; 
        ItemFromLeftHand.transform.position = position;
        IsProbsPlace = true;
        OnProbsInHand?.Invoke(false);
       
    }
    public void ReturnPropsObjTo()
    {
        if(ItemFromLeftHand==null) return;
        if(!IsProbsPlace) return;
        _wire.gameObject.SetActive(false);
        var nn = ItemAttendTo(ItemFromLeftHand, _lHand);
        IsProbsPlace = false;
        OnProbsInHand?.Invoke(true);
    }

    private GameObject ItemAttendTo(GameObject item, Transform hand)
    {
        item.transform.SetParent(hand); 
        item.transform.localPosition = Vector3.zero; 
        item.transform.localRotation = Quaternion.identity;
        return item;
    }
    
    
}