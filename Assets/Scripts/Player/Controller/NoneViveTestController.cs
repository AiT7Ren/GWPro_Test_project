using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class NoneViveTestController : MonoBehaviour, IPlayerControllerIniter, IUseCallBack
{
    private const float Y_CAM_LIMIT = 89f;
    
    [SerializeField] private bool _VRControlImitation;
    
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _mouseSpeed;
    [SerializeField] private Transform _camHolder;
    private Transform _playerRoot;
    private CharacterController _cc;
    private Vector3 _moveDir;
    
    private Camera _mCam;
    private float _pitch;
    private float _yaw;
    
    private bool _buttonHolding;
    private IInteractible _interactible;
    private Vector3? _hitPos;
    
    [SerializeField] private LayerMask _mask;
    [SerializeField] private float _rayDistance = 2.5f;
    
    [SerializeField] private GazAnalyzerController _itemController;
    [SerializeField] private Inventory _inventory;
    
    private СontextHints _contextHints;
    [SerializeField] private bool _isNewInputSystem; 
    private IInputSystem _inputSystem;
    [SerializeField] private Transform _aimHelper;
    private CustomCursore _cursore;
    [SerializeField]private Texture2D _cursoreTexture;
    
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        OffOnCursore();
        SetActiveInputSystem();
    }
#endif
    private void OffOnCursore()
    {
        if (_VRControlImitation)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            _aimHelper.gameObject.SetActive(false);
            if (_cursore == null) _cursore = new CustomCursore(_cursoreTexture);
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
        SetActiveInputSystem();
    }

    public void IsToInventoryCallBack(bool leftHand)
    {
        if(_interactible.GetInteractiveType()!=IteractibleType.PickUp) return; 
        if (_interactible is not ItemPickUp item) return;
        if(leftHand)_inventory.EqipToLeftHand(item.gameObject);
        else _inventory.EqipToRightHand(item.gameObject);
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
    private void OnDisable()
    {
        (_inputSystem as IDisposable)?.Dispose();
    }
    private void CameraShoot()
    {
        var ray = new Ray(_mCam.transform.position, _mCam.transform.forward);
        ShootTest(ray);
    }
    private void ToMouseShoot()
    {
        var ray = _mCam.ScreenPointToRay(_inputSystem.GetMousePosition());
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
        if (_inputSystem.GetMouseDelta() == Vector2.zero) return;
        _yaw = Clamper(_yaw + _inputSystem.GetMouseDelta().x);
        _pitch = Clamper(_pitch - _inputSystem.GetMouseDelta().y, -Y_CAM_LIMIT, Y_CAM_LIMIT);
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
        if (_inputSystem.GetMoveInput()==Vector2.zero)
        {
            _cc.Move(Vector3.zero);
            return;
        }
        _moveDir = _playerRoot.forward * _inputSystem.GetMoveInput().y + _playerRoot.right * _inputSystem.GetMoveInput().x;
        _moveDir *= _moveSpeed;
        _cc.Move(_moveDir*Time.deltaTime);
    }
    private void InputRead()
    {
        _inputSystem.InputUpdateType();
        InputSignalIntepretator();
    }
    private void InputSignalIntepretator()
    {
        if (_inputSystem.GetRightTriggerPressed()) TryUseRight();
        if (_inputSystem.GetLeftTriggerPressed()) TryUseLeft();
        if (_inputSystem.GetLeftTriggerReleased()||_inputSystem.GetRightTriggerReleased()) StopUse();
        if (!_VRControlImitation && _itemController != null)
        {
            if (_inputSystem.GetRightMouseButtonPressed()) _itemController.PowerStartHold();
            if (_inputSystem.GetRightMouseButtonReleased()) _itemController.PowerStopHold();
            if (_inputSystem.GetLeftMouseButtonPressed()) _itemController.LeakStartHold();
            if (_inputSystem.GetLeftMouseButtonReleased()) _itemController.LeakStopHold();
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
            if (_interactible.GetInteractiveType() == IteractibleType.HoldButton) _buttonHolding = true;
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
        if(_interactible.GetInteractiveType() == IteractibleType.HoldButton)_buttonHolding=true;
    }
    private void SetActiveInputSystem()
    {
        (_inputSystem as IDisposable)?.Dispose();
        _inputSystem=_isNewInputSystem?new NewInputSystem():new OldInputSystem();
    }
    private void CCSettings()
    {
        if (_cc == null) throw new Exception($"character controller not found in NoneViveTestController");
        _cc.radius = 0.4f;
        _cc.height = 1.8f;
        _cc.center = new Vector3(0, _cc.height / 2f, 0);
    }
  
}