using System;
using UnityEngine;

public class NoneViveTestController : MonoBehaviour,IPlayerControllerIniter
{
    [SerializeField] private bool _VRControlImitation;
    private CharacterController _cc;
    private Vector2 _mouseInput;
    private Vector2 _moveInput;
    private Vector3 _moveDir;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _mouseSpeed;
    [SerializeField]private Transform _camHolder;
    private Transform _playerRoot;
    private Camera _mCam;
    private float _pitch;
    private float _yaw;
    private IInteractible _useble;
    [SerializeField] private LayerMask _mask;
    [SerializeField] private Transform _usePromt;
    [SerializeField] private Transform _aimHelper;
    [SerializeField] private Transform _holdPromt;
    [SerializeField] private Transform _pickUpLPromt;
    [SerializeField] private Transform _pickUpRPromt;
    private const float ZERO_FOR_INPUT = 0.001f;
    private const float Y_CAM_LIMIT = 89f;
    private bool _buttonHolding;
    [SerializeField] private GazAnalyzerController _itemController;
    [SerializeField] private Inventory _inventory;

    private Vector3 hitPos;
    [SerializeField]private float _rayDistance=2.5f;
    [SerializeField] private Transform _tutorialText;
#if UNITY_EDITOR
    private void OnValidate()
    {
        OffOnCursore();
    }
#endif
    private void OffOnCursore()
    {
        if (_VRControlImitation)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            _aimHelper.gameObject.SetActive(false);
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _aimHelper.gameObject.SetActive(true);
        }
    }
    
    public void Init(Camera cam,Transform playerRoot,Inventory inventory=null)
    {
        _inventory = inventory;
        _playerRoot = playerRoot;
        _mCam = cam;
        _cc = _playerRoot.gameObject.AddComponent<CharacterController>();
        CCSettings();
        if (_camHolder == null) _camHolder = transform.GetChild(0).transform;
        cam.transform.parent = _camHolder;
        cam.transform.localPosition = Vector3.zero;
        _pitch = _camHolder.localRotation.eulerAngles.x;
        _yaw = transform.rotation.eulerAngles.y;
        if (Tutorial.Instance != null)Tutorial.Instance.SetText(_tutorialText);
    }

    void CCSettings()
    {
        if(_cc==null) throw new Exception($"character controller not found in NoneViveTestController");
        _cc.radius = 0.4f;
        _cc.height = 1.8f;
        _cc.center = new Vector3(0,_cc.height/2f, 0);
    }

    private void Update()
    {
        InputRead();
        Move();
        if (_buttonHolding) _useble?.Use();
    }

    private void LateUpdate()
    {
        Rotate();
    }

    private void FixedUpdate()
    {
        if (_VRControlImitation) ToMouseShoot();
        else CameraShoot();
    }

    private void CameraShoot()
    {
        var ray = new Ray(_mCam.transform.position, _mCam.transform.forward);
        ShootTest(ray);
    }

    private void ToMouseShoot()
    {
        var ray = _mCam.ScreenPointToRay(Input.mousePosition);
        ShootTest(ray);
    }

    private void ShootTest(Ray ray)
    {
        if (Physics.Raycast(ray, out var hit, _rayDistance, _mask))
        {
            if (hit.collider.TryGetComponent(out IInteractible useble))
            {
                if (_useble is TestPlase testPlase)
                {
                    hitPos=hit.point;
                }
                if (_useble != useble) SetNewUseble(useble);
                return;
            }
            else
            {
                SetNewUseble(null);
                return;
            }
        }
        else SetNewUseble(null);
        
    }

    private void UpdateUseIcon(IInteractible useble)
    {
        _usePromt.gameObject.SetActive(false);
        _holdPromt.gameObject.SetActive(false);
        _pickUpLPromt.gameObject.SetActive(false);
        _pickUpRPromt.gameObject.SetActive(false);
        if (useble == null) return;
        if (useble is RemoteButton button)
        {
            if (button.ButtonType == RemoteButton.ButtonState.Hold) _holdPromt.gameObject.SetActive(true);
            else _usePromt?.gameObject.SetActive(true);
            return;
        }

        if (_inventory != null)
            if (useble is ItemPickUp pickup)
            {
                if (pickup.ItemToHad == ItemPickUp.ItemHand.Left) _pickUpLPromt.gameObject.SetActive(true);
                else _pickUpRPromt.gameObject.SetActive(true);
            }
    }

    private void Rotate()
    {
        if (_mouseInput.sqrMagnitude < ZERO_FOR_INPUT) return;
        _yaw = Clamper(_yaw + _mouseInput.x);
        _pitch = Clamper(_pitch - _mouseInput.y, -Y_CAM_LIMIT, Y_CAM_LIMIT);
        _playerRoot.rotation = Quaternion.Euler(0, _yaw, 0);
        _camHolder.localRotation = Quaternion.Euler(_pitch, 0, 0);
    }

    private float Clamper(float current, float min = 0, float max = 0)
    {
        if (min != 0 || max != 0) return Mathf.Clamp(current, min, max);
        if (current > 360f) current -= 360f;
        if (current < -360f) current += 360f;
        return current;
    }

    private void Move()
    {
        if (_moveInput.sqrMagnitude < ZERO_FOR_INPUT)
        {
            _cc.Move(Vector3.zero);
            return;
        }

        _moveDir = _playerRoot.forward * _moveInput.y + _playerRoot.right * _moveInput.x;
        _moveDir=_moveDir.normalized *Time.deltaTime;
        _cc.Move(_moveDir * _moveSpeed);
    }

    private void InputRead()
    {
        _moveInput.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        _mouseInput.Set(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        if (Input.GetKeyDown(KeyCode.E)) TryUse();
        if (Input.GetKeyDown(KeyCode.Q)) TryUseLeft();
        if (Input.GetKeyUp(KeyCode.E)) StopUse();
        if (!_VRControlImitation && _itemController != null)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1)) _itemController.PowerStartHold();
            if (Input.GetKeyUp(KeyCode.Mouse1)) _itemController.PowerStopHold();
            if (Input.GetKeyDown(KeyCode.Mouse0)) _itemController.LeakStartHold();
            if (Input.GetKeyUp(KeyCode.Mouse0)) _itemController.LeakStopHold();
        }
    }

    private void TryUseLeft()
    {
        Debug.Log("Try use left");
        if (_inventory != null)
        {
            if (_inventory.IsProbsPlace&&_inventory.ItemFromLeftHand!=null)
            {
                Debug.Log("Try return");
                _inventory.ReturnPropsObjTo();
                return;
            }
            if (_useble == null) return;
            if(_useble is ItemPickUp pickUp)PickUp(pickUp);
            if (_useble is not TestPlase place) return;
            Debug.Log("Try use placer");
            _inventory.ReleasePropsObjTo(hitPos);
        }
        
    }

    private void StopUse()
    {
        if (!_buttonHolding) return;
        (_useble as RemoteButton)?.StopUse();
        _buttonHolding = false;
    }

    private void SetNewUseble(IInteractible useble)
    {
        if (_useble == null) StopUse();
        _useble = useble;
        UpdateUseIcon(_useble);
    }

    private void TryUse()
    {
        if (_useble == null) return;
        if (_useble is RemoteButton button)
        {
            if (button.ButtonType == RemoteButton.ButtonState.Hold) _buttonHolding = true;
            else button.Use();
            return;
        }
        if (_inventory == null) return;
        if(_useble is ItemPickUp pickUp)PickUp(pickUp);
    }

    void PickUp(ItemPickUp pickup)
    {
        if (_itemController == null)
        {
            _inventory.TryGetComponent(out GazAnalyzerController gazController);
            _itemController = gazController;
            if(_itemController != null) throw new Exception("Pickup controller could not be found in VRTestController");
        }
        if (pickup.ItemToHad == ItemPickUp.ItemHand.Right) _inventory.EqipToRightHand(pickup.gameObject);
        else _inventory.EqipToLeftHand(pickup.gameObject);
        pickup.Use();
    }
    
    
}