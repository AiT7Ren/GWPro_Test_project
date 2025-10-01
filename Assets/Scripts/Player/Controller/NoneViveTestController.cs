using System;
using UnityEngine;

public class NoneViveTestController : MonoBehaviour, IPlayerControllerIniter, IUseCallBack
{
    [SerializeField] private bool _VRControlImitation;
    private CharacterController _cc;
    private Vector2 _mouseInput;
    private Vector2 _moveInput;
    private Vector3 _moveDir;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _mouseSpeed;
    [SerializeField] private Transform _camHolder;
    private Transform _playerRoot;
    private Camera _mCam;
    private float _pitch;
    private float _yaw;
    private IInteractible _interactible;
    [SerializeField] private LayerMask _mask;
    [SerializeField] private Transform _aimHelper;
    private const float ZERO_FOR_INPUT = 0.001f;
    private const float Y_CAM_LIMIT = 89f;
    private bool _buttonHolding;
    [SerializeField] private GazAnalyzerController _itemController;
    [SerializeField] private Inventory _inventory;

    private Vector3? _hitPos;
    [SerializeField] private float _rayDistance = 2.5f;
    [SerializeField] private Transform _tutorialText;
    private СontextHints _contextHints;
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

    public void Init(Camera cam, Transform playerRoot, Inventory inventory = null,СontextHints contextHints = null)
    {
        _inventory = inventory;
        _playerRoot = playerRoot;
        _mCam = cam;
        _contextHints= contextHints;
        _cc = _playerRoot.gameObject.AddComponent<CharacterController>();
        CCSettings();
        if (_camHolder == null) _camHolder = transform.GetChild(0).transform;
        cam.transform.parent = _camHolder;
        cam.transform.localPosition = Vector3.zero;
        _pitch = _camHolder.localRotation.eulerAngles.x;
        _yaw = transform.rotation.eulerAngles.y;
    }

    void CCSettings()
    {
        if (_cc == null) throw new Exception($"character controller not found in NoneViveTestController");
        _cc.radius = 0.4f;
        _cc.height = 1.8f;
        _cc.center = new Vector3(0, _cc.height / 2f, 0);
    }

    private void Update()
    {
        InputRead();
        Move();
        if (_buttonHolding)_interactible?.Use();
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
        _hitPos = null;
        IInteractible newInteractible = null;
        if (Physics.Raycast(ray, out var hit, _rayDistance, _mask))
        {
            newInteractible = hit.collider.gameObject.GetComponent<IInteractible>();
            if(newInteractible.GetInteractiveType()==IteractibleType.Place)_hitPos=hit.point;
        }
        if (_interactible != newInteractible) SetNewUseble(newInteractible);
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
        _moveDir = _moveDir.normalized * Time.deltaTime;
        _cc.Move(_moveDir * _moveSpeed);
    }

    private void InputRead()
    {
        _moveInput.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        _mouseInput.Set(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        if (Input.GetKeyDown(KeyCode.E)) TryUseRight();
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
        if (_inventory != null)
        {
            if(!_inventory.IfItemOnHand(isLeft:true)&&_inventory.HaveObject(isLeft:true))_inventory.ReturnPropsObjTo();
            if(_inventory.IfItemOnHand(isLeft:true)&&_inventory.HaveObject(isLeft:true)&&_hitPos!=null)_inventory.ReleasePropsObjTo(_hitPos.Value);
            if (_interactible == null) return;
            _interactible?.Use(this);
        }
    }
    private void StopUse()
    {
        _buttonHolding = false; 
        (_interactible as RemoteButton)?.StopUse();
    }

    private void SetNewUseble(IInteractible useble)
    {
        if (_buttonHolding) StopUse();
        _interactible = useble;
        
        if(_interactible==null)_contextHints.SetNewHint(null);
        else _contextHints.SetNewHint(_interactible.GetInteractiveType());
    }

    private void TryUseRight()
    {
        if (_interactible == null) return;
        _interactible.Use(this);
    }

    public void IsHoldCallback(bool set)
    {
        _buttonHolding = set;
    }
    public void IsToInventoryCallBack(bool leftHand)
    {
        if(_interactible.GetInteractiveType()!=IteractibleType.PickUp) return; 
        if (_interactible is not ItemPickUp item) return;
        if(leftHand)_inventory.EqipToLeftHand(item.gameObject);
        else _inventory.EqipToRightHand(item.gameObject);
    }
}