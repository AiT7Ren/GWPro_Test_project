using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAnimation : MonoBehaviour
{
   [SerializeField]private float minScale = 0.5f;
   [SerializeField]private float maxScale = 1.5f;
   private bool toLess;
   private float _currentTarget;
   private float _currentScaleY;
   [SerializeField] private float SpeedChande;
   [SerializeField] private Transform _childTransform; 
   [SerializeField] private float rotateSpeed;
   private void Update()
   {
      float tick = Time.deltaTime;
      _currentScaleY=Mathf.MoveTowards(_currentScaleY,_currentTarget,SpeedChande*tick);
      if (_currentScaleY == _currentTarget)
      {
         _currentTarget = toLess?minScale : maxScale;
         toLess = !toLess;
      }
      transform.localScale=new Vector3(transform.localScale.x,_currentScaleY,transform.localScale.z);
      _childTransform.Rotate(0,rotateSpeed*Time.deltaTime,0,Space.Self);
   }
}
