using System;
using System.Collections.Generic;
using UnityEngine;
using HTC.UnityPlugin.Vive;

public class VRTestPlayerController : MonoBehaviour, IPlayerControllerIniter
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
    private RaycastHit[] _nonAllocatedHits = new RaycastHit[3];
    private IInteractible _lUseble;
    private IInteractible _rUseble;

    private bool _lHold;
    private bool _rHold;
    
    private bool _lRayHit;
    private bool _rRayHit;
    private Camera _mCam;
    private Transform _playerRoot;
    private Inventory _inventory;
   // [SerializeField] private List<VivePoseTracker> _trakers;
    public void Init(Camera cam, Transform playerRoot, Inventory inventory = null)
    {
        _mCam = cam;
        _playerRoot=playerRoot;
        _inventory = inventory;
       // if(_trakers==null)_trakers=new List<VivePoseTracker>(); 
        CamSetup(cam.transform);
    }

    private void CamSetup(Transform cam)
    {
        if (cam == null)throw new Exception("No camera set");
        cam.parent = this.transform;
        cam.localPosition = new Vector3(0, 1.6f, 0);
        VivePoseTracker cameraPoseTracker = cam.gameObject.AddComponent<VivePoseTracker>();
        cameraPoseTracker.viveRole.SetEx(ViveRoleProperty.New(DeviceRole.Hmd));
        cameraPoseTracker.origin = transform;
        
        //if(_trakers.Count>0)_trakers.Add(cameraPoseTracker);
        //PoserOrigionSetup();
        MainBodyPhysicsSetup();
    }

    private void MainBodyPhysicsSetup()
    {
       CapsuleCollider playerCol=_playerRoot.gameObject.AddComponent<CapsuleCollider>();
       playerCol.radius = 0.4f;
       playerCol.height = 1.7f;
       playerCol.center = new Vector3(0, playerCol.height/2f, 0);
       Rigidbody rb=_playerRoot.gameObject.AddComponent<Rigidbody>();
       rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }

    #region hidden_not_use
    //думаю пока это не надо если использовать готовый риг 
    /*private void PoserOrigionSetup()
    {
        if (_trakers.Count<=1)
        {
            var trakers = GetComponentsInChildren<VivePoseTracker>();
            _trakers.AddRange(trakers);
        }
        foreach (var traker in _trakers)
        {
            traker.origin = _playerRoot;
        }
    }*/
    #endregion
    
    

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
        if (Physics.Raycast(lRay, out RaycastHit hit, 10f, _groundMask))
        {
            int hitsNumber = Physics.SphereCastNonAlloc(hit.point, 0.25f, Vector3.up, _nonAllocatedHits, 2f);
            if (hitsNumber == 0)
            {
                Vector3 offset = hit.point - _mCam.transform.position;
                offset.y = hit.point.y - _playerRoot.position.y;
                _playerRoot.position += offset;
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
        
        if (_lUseble == null || _inventory == null) return;
        if (!_inventory.IsProbsPlace && _inventory.ItemFromLeftHand == null) return;
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
        // Левый контроллер
        Ray lRay = new Ray(_lControllerPos, _lControllerRot * Vector3.forward);
        bool leftHit = Physics.Raycast(lRay, out RaycastHit lhit, 3f, _useMask);
    
        if (leftHit)
        {
            if (!_lRayHit || _lHit.collider != lhit.collider)
            {
                SetNewLRayCast(lhit);
            }
            _lRayHit = true;
        }
        else
        {
            if (_lRayHit)
            {
                ClearLeftTarget();
            }
            _lRayHit = false;
        }
    
        // Правый контроллер
        Ray rRay = new Ray(_rControllerPos, _rControllerRot * Vector3.forward);
        bool rightHit = Physics.Raycast(rRay, out RaycastHit rhit, 3f, _useMask);
    
        if (rightHit)
        {
            if (!_rRayHit || _rHit.collider != rhit.collider)
            {
                SetNewRRayCast(rhit);
            }
            _rRayHit = true;
        }
        else
        {
            if (_rRayHit)
            {
                ClearRightTarget();
            }
            _rRayHit = false;
        }
    }
    
    private void SetNewLRayCast(RaycastHit hit)
    {
        if (_lHold)
        {
            _lHold = false;
            StopUse(_lUseble);
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
    
    private void ClearLeftTarget()
    {
        if (_lHold)
        {
            _lHold = false;
            StopUse(_lUseble);
        }
        _lUseble = null;
    }

    private void ClearRightTarget()
    {
        if (_rHold)
        {
            _rHold = false;
            StopUse(_rUseble);
        }
        _rUseble = null;
    }
    
}
