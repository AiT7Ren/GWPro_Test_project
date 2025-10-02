using System;
using System.Collections.Generic;
using UnityEngine;
using HTC.UnityPlugin.Vive;

public class VRTestPlayerController : MonoBehaviour, IPlayerControllerIniter, IUseCallBack
{
    private Vector3 _lControllerPos;
    private Vector3 _rControllerPos;
    private Quaternion _lControllerRot;
    private Quaternion _rControllerRot;
    
    [SerializeField] private LayerMask _useMask;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _rayDistance = 3f;
    [SerializeField] private float _teleportDistance = 10f;
    
    private RaycastHit _lHit;
    private RaycastHit _rHit;
    private RaycastHit[] _nonAllocatedHits = new RaycastHit[3];
    
    private IInteractible _lInteractible;
    private IInteractible _rInteractible;

    private bool _lHold;
    private bool _rHold;
    
    private bool _lRayHit;
    private bool _rRayHit;
    
    private Vector3? _lHitPos;
    private Vector3? _rHitPos;
    
    private Camera _mCam;
    private Transform _playerRoot;
    private Inventory _inventory;
    private СontextHints _contextHints;

    public void Init(Camera cam, Transform playerRoot, Inventory inventory = null, СontextHints contextHints = null)
    {
        _mCam = cam;
        _playerRoot = playerRoot;
        _inventory = inventory;
        _contextHints = contextHints;
        CamSetup(cam.transform);
    }

    private void CamSetup(Transform cam)
    {
        if (cam == null) throw new Exception("No camera set");
        cam.parent = this.transform;
        cam.localPosition = new Vector3(0, 1.6f, 0);
        
        VivePoseTracker cameraPoseTracker = cam.gameObject.AddComponent<VivePoseTracker>();
        cameraPoseTracker.viveRole.SetEx(ViveRoleProperty.New(DeviceRole.Hmd));
        cameraPoseTracker.origin = transform;
        
        MainBodyPhysicsSetup();
    }

    private void MainBodyPhysicsSetup()
    {
        CapsuleCollider playerCol = _playerRoot.gameObject.AddComponent<CapsuleCollider>();
        playerCol.radius = 0.4f;
        playerCol.height = 1.7f;
        playerCol.center = new Vector3(0, playerCol.height / 2f, 0);
        
        Rigidbody rb = _playerRoot.gameObject.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionX | 
                        RigidbodyConstraints.FreezePositionZ | 
                        RigidbodyConstraints.FreezeRotation;
    }

    private void Update()
    {
        GetControllerPosition();
        InputControllerRead();
        if (_inventory != null)
            _inventory.VRHandInventory(_lControllerPos, _rControllerPos);
        
        if (_lHold) _lInteractible?.Use();
        if (_rHold) _rInteractible?.Use();
    }
    private void FixedUpdate()
    {
        Caster();
    }
    private void GetControllerPosition()
    {
        _lControllerPos = VivePose.GetPose(HandRole.LeftHand).pos;
        _rControllerPos = VivePose.GetPose(HandRole.RightHand).pos;
        _lControllerRot = VivePose.GetPose(HandRole.LeftHand).rot;
        _rControllerRot = VivePose.GetPose(HandRole.RightHand).rot;
    }
    private void InputControllerRead()
    {
        if (ViveInput.GetPressDown(HandRole.LeftHand, ControllerButton.Trigger))
            TryUseLeft();
        
        if (ViveInput.GetPressUp(HandRole.LeftHand, ControllerButton.Trigger))
            StopUseLeft();
        
        if (ViveInput.GetPressDown(HandRole.LeftHand, ControllerButton.Bumper))
            TryTeleport();
        
        if (ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger))
            TryUseRight();
        
        if (ViveInput.GetPressUp(HandRole.RightHand, ControllerButton.Trigger))
            StopUseRight();
    }

    private void TryTeleport()
    {
        Ray lRay = new Ray(_lControllerPos, _lControllerRot * Vector3.forward);
        
        if (Physics.Raycast(lRay, out RaycastHit hit, _teleportDistance, _groundMask))
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

    private void TryUseLeft()
    {
        if (_lInteractible == null) return;
        
        if (_inventory != null)
        {
            if (!_inventory.IfItemOnHand(isLeft: true) && _inventory.HaveObject(isLeft: true))
                _inventory.ReturnPropsObjTo();
            
            if (_inventory.IfItemOnHand(isLeft: true) && _inventory.HaveObject(isLeft: true) && _lHitPos != null)
                _inventory.ReleasePropsObjTo(_lHitPos.Value);
        }
        
        _lInteractible.Use(this);
        if (_lInteractible.GetInteractiveType() == IteractibleType.HoldButton) 
            _lHold = true;
    }

    private void TryUseRight()
    {
        if (_rInteractible == null) return;
        _rInteractible.Use(this);
        if (_rInteractible.GetInteractiveType() == IteractibleType.HoldButton) 
            _rHold = true;
    }

    private void StopUseLeft()
    {
        _lHold = false;
        (_lInteractible as RemoteButton)?.StopUse();
    }

    private void StopUseRight()
    {
        _rHold = false;
        (_rInteractible as RemoteButton)?.StopUse();
    }

    private void Caster()
    {
        BaseRayCast(
            _lControllerPos, 
            _lControllerRot,
            ref _lHit,
            ref _lRayHit,
            ref _lHitPos,
            ref _lInteractible,
            SetNewLeftInteractible
        );
        
        BaseRayCast(
            _rControllerPos,
            _rControllerRot, 
            ref _rHit,
            ref _rRayHit,
            ref _rHitPos,
            ref _rInteractible,
            SetNewRightInteractible
        );
    }

    private void BaseRayCast(
        Vector3 controllerPos,
        Quaternion controllerRot,
        ref RaycastHit hit,
        ref bool rayHit,
        ref Vector3? hitPos,
        ref IInteractible currentInteractible,
        Action<IInteractible> setNewInteractible)
    {
        Ray ray = new Ray(controllerPos, controllerRot * Vector3.forward);
        bool hasHit = Physics.Raycast(ray, out RaycastHit newHit, _rayDistance, _useMask);
        
        hitPos = null;
        IInteractible newInteractible = null;
        
        if (hasHit)
        {
            newInteractible = newHit.collider.GetComponent<IInteractible>();
            if (newInteractible?.GetInteractiveType() == IteractibleType.Place)
                hitPos = newHit.point;
            
            if (!rayHit || hit.collider != newHit.collider)
            {
                hit = newHit;
            }
            rayHit = true;
        }
        else
        {
            rayHit = false;
        }
        
        if (currentInteractible != newInteractible)
            setNewInteractible(newInteractible);
    }

    private void SetNewLeftInteractible(IInteractible interactible)
    {
        if (_lHold)
        {
            _lHold = false;
            (_lInteractible as RemoteButton)?.StopUse();
        }
        
        _lInteractible = interactible;
        
        if (_contextHints != null)
        {
            if (_lInteractible == null)
                _contextHints.SetNewHint(null);
            else
                _contextHints.SetNewHint(_lInteractible.GetInteractiveType());
        }
    }

    private void SetNewRightInteractible(IInteractible interactible)
    {
        if (_rHold)
        {
            _rHold = false;
            (_rInteractible as RemoteButton)?.StopUse();
        }
        
        _rInteractible = interactible;
    }
    public void IsToInventoryCallBack(bool leftHand)
    {
        IInteractible interactible = leftHand ? _lInteractible : _rInteractible;
        
        if (interactible?.GetInteractiveType() != IteractibleType.PickUp) return;
        if (interactible is not ItemPickUp item) return;
        
        if (leftHand)
            _inventory.EqipToLeftHand(item.gameObject);
        else
            _inventory.EqipToRightHand(item.gameObject);
    }
}