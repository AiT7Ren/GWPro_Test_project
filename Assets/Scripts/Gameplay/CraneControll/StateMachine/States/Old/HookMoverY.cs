using System;
using UnityEngine;

namespace Gameplay.CraneControll.StateMachine.States.Old
{
   public class HookMoverY:ICraneMoverState
   {
      public event Action OnTargetDistanation;
   
      private Transform _currentTransform;
      Transform _hookTransform;
      private Vector3 _target;
      private float _moveSpeed;
      private float _rotateSpeed;
      private AudioSource _audioSource;
      public HookMoverY(Transform current,Vector3 moveTo, float moveSpeed=1f,Transform hookHolder=null,float rotateSpeed=1f)
      {
         _currentTransform = current;
         _target = moveTo;
         _moveSpeed = moveSpeed;
         _hookTransform = hookHolder;
         _rotateSpeed = rotateSpeed;
         InitAudio(); //можно было бы и передать через конструктор, но как то уже много 
      }

      private void InitAudio()
      {
         _audioSource=_hookTransform.GetComponent<AudioSource>();
         _audioSource.playOnAwake = false;
         _audioSource.loop = true;
      }

      public void Tick(float deltaTime)
      {
         Debug.Log($"{_currentTransform.gameObject.name}tick to {_target}");
         _target.Set(_currentTransform.position.x,_target.y,_currentTransform.position.z);
         _currentTransform.position= Vector3.MoveTowards(_currentTransform.position, _target ,_moveSpeed*deltaTime);
         if(!_audioSource.isPlaying)_audioSource.Play();
         if(_currentTransform.position == _target)
         {
            OnTargetDistanation?.Invoke();
            Stop();
         }
         _hookTransform.transform.Rotate(_rotateSpeed * deltaTime, 0, 0, Space.Self);
      
      }

      public void Stop()
      {
         _audioSource.Stop();
      }

      public void ChangeMoveSpeed(float newSpeed)
      {
         _moveSpeed = newSpeed;
      }
  
   
   }
}