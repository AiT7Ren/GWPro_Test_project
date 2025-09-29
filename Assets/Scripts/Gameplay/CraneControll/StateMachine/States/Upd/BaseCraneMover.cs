using System;
using UnityEngine;

namespace Gameplay.CraneControll.StateMachine.States.Upd
{
    public abstract class BaseCraneMover : ICraneMoverState
    {
        public event Action OnTargetDistanation;

        protected Transform _currentTransform;
        protected Vector3 _target;
        protected float _moveSpeed;
        protected AudioSource _audioSource;
        protected BaseCraneMover(Transform current, Vector3 moveTo, float moveSpeed)
        {
            _currentTransform = current;
            _target = moveTo;
            _moveSpeed = moveSpeed;
        }

        protected virtual void InitAudio()
        {
            if (_currentTransform != null)
            {
                _audioSource = _currentTransform.GetComponent<AudioSource>();
                if (_audioSource != null)
                {
                    _audioSource.playOnAwake = false;
                    _audioSource.loop = true;
                }
            }
        }

        protected abstract Vector3 GetTargetPosition();

        public virtual void Tick(float deltaTime)
        {
            _target = GetTargetPosition();
            _currentTransform.position =
                Vector3.MoveTowards(_currentTransform.position, _target, _moveSpeed * deltaTime);

            PlayAudio();
            HandleAdditionalActions(deltaTime);

            if (Vector3.Distance(_currentTransform.position, _target) < 0.01f)
            {
                OnTargetDistanation?.Invoke();
                Stop();
            }
        }

        protected virtual void PlayAudio()
        {
            if (_audioSource != null && !_audioSource.isPlaying)
                _audioSource.Play();
        }

        protected virtual void HandleAdditionalActions(float deltaTime)
        {
            //что б не переопределять тик
        }

        public virtual void Stop()
        {
            if (_audioSource != null)
                _audioSource.Stop();
        }

        public virtual void ChangeMoveSpeed(float newSpeed)
        {
            _moveSpeed = newSpeed;
        }
    }
}
