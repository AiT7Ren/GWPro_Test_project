using System;
using UnityEngine;
using HTC.UnityPlugin.Vive;

public class VRTestController : MonoBehaviour
{
    //Я с VR не работал, так что буду писать что я хочу получить от контроллера
    //потестить возможноти нет 
    //хочу сделать как Non_vive
    private Vector3 _lControllerPos; //хеши позиции контроллеров
    private Vector3 _rControllerPos;
    private Quaternion _lControllerRot;
    private Quaternion _rControllerRot;
    
    [SerializeField]private LayerMask _useMask;
    [SerializeField]private LayerMask _groundMask;
    private RaycastHit _lHit;
    private RaycastHit _rHit;
    private RaycastHit[] nonAllocatedHits = new RaycastHit[3];
    private IInteractible _lUseble;
    private IInteractible _rUseble;

    private bool _lHold;
    private bool _rHold;
    
    private Camera _mCam; 
    private VivePoseTracker _poseTracker;
    
    [SerializeField] Inventory _inventory;

    private void Start()
    {
        _mCam = Camera.main;
        _mCam.transform.parent = null;
        _poseTracker = _mCam.GetComponent<VivePoseTracker>(); //как я понял он работает автоматически
        _poseTracker.posOffset=new Vector3(0, 1.6f, 0); //
    }

    private void Update()
    {
        InputControllerRead(HandRole.LeftHand);
        InputControllerRead(HandRole.RightHand);
        if (ViveInput.GetPressDown(HandRole.LeftHand, ControllerButton.Bumper)) TryTeleport();
        GetControllerPosition();
        if(_inventory!=null)_inventory.VRHandInventory(_lControllerPos, _rControllerPos);
        if(_lHold)_lUseble.Use(); 
        if(_rHold)_rUseble.Use();
    }

    private void TryTeleport()
    {
        Ray lRay = new Ray(_lControllerPos,_lControllerRot*Vector3.forward); //телепорт с проверкой,
                                                                                         //но я все ещё не понимаю что делать с камерой и другими контроллерами
                                                                                         //вероятно должен быть какой то рутовый объект и его и надо двигать 
        if (Physics.Raycast(lRay, out RaycastHit hit, 10f, _groundMask))
        {
            int hitsNumber = Physics.SphereCastNonAlloc(hit.point, 0.25f, Vector3.up, nonAllocatedHits, 2f);
            if (hitsNumber == 0)
            {
                transform.position = hit.point;
                _poseTracker.transform.position = hit.point;   //ну допустим камера под винится а вот что с контроллерами
            }
        }
    }

    private void FixedUpdate()
    {
        Caster();
    }
    
    private void InputControllerRead(HandRole role)
    {
        //Для работы мне нужно нажатие и отпускания курка
        if(ViveInput.GetPressDown(role,ControllerButton.Trigger))   //как я понимаю мне надо роль это какой контроллер и кнопка
        {
            TryUse(role);
        }
        if(ViveInput.GetPressUp(role,ControllerButton.Trigger))
        {
            TryStopUse(role);
        }  
    }

    private void TryStopUse(HandRole role)
    {
        if(role == HandRole.LeftHand&&_lHold)_lHold = false;
        if(role == HandRole.RightHand&&_rHold)_rHold = false;
    }

    private void TryUse(HandRole hand)
    {
        if(hand == HandRole.LeftHand)LeftUse();
        if(hand == HandRole.RightHand)RightUse();
    }
    //думаю можно посидеть и не распараллеливать методы, но без теста думаю сделаю точно где то ошибку а потом не найду 
    private void LeftUse() //вот тут думаю будет ошибка 
    {
        
        if (_lUseble == null||((_inventory==null)&&(!_inventory.IsProbsPlace&&_inventory.ItemFromLeftHand==null))) return; //без обьекта для использования или нельзя зайти в ReturnPropsObjTo то выходим
        if (_lUseble is RemoteButton button) //проверяем что это кнопка
        {
            if (button.ButtonType == RemoteButton.ButtonState.Hold) _lHold = true; //если это кнопка с зажимом зажимаем
            else button.Use(); //если обычная просто используем
            return;
        }
        if (_inventory != null) //проверка на инвентарь
        {
            if (_inventory.IsProbsPlace&&_inventory.ItemFromLeftHand!=null) //если мы брали датчик и он не в руках
            {
                Debug.Log("Try return");
                _inventory.ReturnPropsObjTo(); //возврат в руки
                return;
            }
            if (_lUseble is ItemPickUp pickup) //проверяем что объект можно поднять 
            {
                if (pickup.ItemToHad == ItemPickUp.ItemHand.Left) // просто что об]ект для левой руки и добавляем в инвентарь
                    _inventory.EqipToLeftHand(pickup.gameObject); 
                pickup.Use();
            }
            if (_lUseble is TestPlase place) //проверка что место для тест датчика
            {
                Debug.Log("Try use placer");
                _inventory.ReleasePropsObjTo(_lHit.point); //ставим датчик в место
            }
        }
    }

    private void RightUse()
    {
        if (_rUseble == null) return;
        if (_rUseble is RemoteButton button)
        {
            if (button.ButtonType == RemoteButton.ButtonState.Hold) _rHold = true;
            else button.Use();
            return;
        }
        if (_inventory != null)
            if (_rUseble is ItemPickUp pickup)
            {
                if (pickup.ItemToHad == ItemPickUp.ItemHand.Right)
                    _inventory.EqipToRightHand(pickup.gameObject);
                pickup.Use();
            }
    }
    
    private void GetControllerPosition()
    {
        _lControllerPos = VivePose.GetPose(HandRole.LeftHand).pos;  //было бы лучше просто получить трансформ 
        _rControllerPos = VivePose.GetPose(HandRole.RightHand).pos; //это мне надо для рейкаста, возможно лучше было бы Vector3.forward за места вращения 
        _lControllerRot = VivePose.GetPose(HandRole.LeftHand).rot;  //а так я думаю повесить пустой монобех на контроллер и брать его трансформ 
        _rControllerRot = VivePose.GetPose(HandRole.RightHand).rot; //upd уже в конце когда делаю камеру увидел что есть компонент VivePoseTracker
    }
    private void Caster()
    {
        Ray lRay = new Ray(_lControllerPos,_lControllerRot*Vector3.forward); //должен получится луч из контроллера по локальной Z
        if (Physics.Raycast(lRay, out RaycastHit lhit, 3f, _useMask))
        {
            if(_lHit.collider!=lhit.collider)SetNewLRayCast(lhit);
        }
        Ray rRay = new Ray(_rControllerPos,_rControllerRot*Vector3.forward);
        if (Physics.Raycast(rRay, out RaycastHit rhit, 3f, _useMask))
        {
            if(_rHit.collider!=lhit.collider)SetNewRRayCast(lhit);
        }
        
    }
    private void SetNewLRayCast(RaycastHit hit)
    {
        if (_lHold)
        {
            _lHold = false;
            StopUse(_rUseble);
        }
        _lHit = hit;
        _lUseble = _lHit.collider.TryGetComponent(out IInteractible useble) ? useble : null;
    }
    private void SetNewRRayCast(RaycastHit hit)
    {
        if (_rHold)
        {
            _rHold = false;
            StopUse(_rUseble);
        }
        _rHit = hit;
        _rUseble = _rHit.collider.TryGetComponent(out IInteractible useble) ? useble : null;
    }
    
    private void StopUse(IInteractible _useble)
    {
        (_useble as RemoteButton)?.StopUse();
    }
    
    
    
    
}
