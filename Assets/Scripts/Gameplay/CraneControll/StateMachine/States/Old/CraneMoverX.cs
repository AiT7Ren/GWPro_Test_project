using System;
using UnityEngine;

namespace Gameplay.CraneControll.StateMachine.States.Old
{
   public class CraneMoverX:ICraneMoverState
   {
      public event Action OnTargetDistanation;

      private Transform _currentTransform;
      private Vector3 _target;
      private float _moveSpeed;
      private AudioSource _audioSource;

      public CraneMoverX(Transform current,Vector3 moveTo, float moveSpeed = 1f)
      {
         _currentTransform = current;
         _target = moveTo;
         _moveSpeed = moveSpeed;
         InitAudio();
      }

      private void InitAudio()
      {
         _audioSource=_currentTransform.GetComponent<AudioSource>();
         _audioSource.playOnAwake = false;
         _audioSource.loop = true;
      }
      public void Tick(float deltaTime)
      {
         Debug.Log($"{_currentTransform.gameObject.name}tick to {_target}");
         if(!_audioSource.isPlaying)_audioSource.Play();
         _target.Set(_target.x,_currentTransform.position.y,_currentTransform.position.z);
         _currentTransform.position= Vector3.MoveTowards(_currentTransform.position, _target ,_moveSpeed*deltaTime);
         if (_currentTransform.position == _target)
         {
            OnTargetDistanation?.Invoke();
            Stop();
         }
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