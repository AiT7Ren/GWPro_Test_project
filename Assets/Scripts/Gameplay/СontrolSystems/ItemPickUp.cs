using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ItemPickUp : MonoBehaviour,IInteractible
{
    
    [SerializeField]private Collider _collider;
    [SerializeField]private Rigidbody _rb;
    private bool _isPicked=false;
    private void Start()
    {
        if(_collider==null)_collider = GetComponent<Collider>();
        if(_rb==null)_rb = GetComponent<Rigidbody>();
        _isPicked = false;
    }

    public enum ItemHand
    {
        Left,
        Right
    } 
    public ItemHand ItemToHad = ItemHand.Left;
    
    public void Use()
    {
        if(!_isPicked)PickUp();
        else Drop();
    }
    private void PickUp()
    {
        gameObject.layer = 3;
        _rb.isKinematic = true;
        _rb.useGravity = false;
        
    }
    private void Drop()
    {
        gameObject.layer = 7;
        _rb.isKinematic = false;
        _rb.useGravity = true;
        
    }
}
