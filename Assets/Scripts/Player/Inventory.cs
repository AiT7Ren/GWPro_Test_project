using UnityEngine;
using UnityEngine.SceneManagement;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Transform _rHand;
    public GameObject ItemFromRightHand;
    [SerializeField] private Transform _lHand;
    public GameObject ItemFromLeftHand;
    [SerializeField] private Transform _wire;
    public bool IsProbsPlace = false;
    public void EqipToRightHand(GameObject item)
    {
        if (item != null) ItemFromRightHand = ItemAttendTo(item, _rHand);
        if(IsNotHaveInstance())return; 
        Tutorial.Instance.TutorialSteps[0].waitForPlayerAction=true;
    }

    
    public void EqipToLeftHand(GameObject item)
    {
        if (item != null)
        {
            ItemFromLeftHand = ItemAttendTo(item, _lHand);
            IsProbsPlace = false;
            if(IsNotHaveInstance())return;
            Tutorial.Instance.TutorialSteps[1].waitForPlayerAction=true;
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
        if (IsNotHaveInstance()) return;
        if(Tutorial.Instance.TutorialSteps[5].active)Tutorial.Instance.TutorialSteps[5].waitForPlayerAction = true;
    }
    public void ReturnPropsObjTo()
    {
        if(ItemFromLeftHand==null) return;
        if(!IsProbsPlace) return;
        _wire.gameObject.SetActive(false);
        var nn = ItemAttendTo(ItemFromLeftHand, _lHand);
        IsProbsPlace = false;
        if (IsNotHaveInstance()) return;
        if(Tutorial.Instance.TutorialSteps[6].active)Tutorial.Instance.TutorialSteps[6].waitForPlayerAction = true;
    }

    private GameObject ItemAttendTo(GameObject item, Transform hand)
    {
        item.transform.SetParent(hand); 
        item.transform.localPosition = Vector3.zero; 
        item.transform.localRotation = Quaternion.identity;
        return item;
    }
    private bool IsNotHaveInstance()
    {
        return Tutorial.Instance == null;
    }
    
}