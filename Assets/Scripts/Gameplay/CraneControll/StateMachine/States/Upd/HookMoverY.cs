using UnityEngine;

namespace Gameplay.CraneControll.StateMachine.States.Upd
{
    public class HookMoverY : BaseCraneMover
    {
        private Transform _hookCoilTransform;
        private float _rotateSpeed;

        public HookMoverY(Transform current, Vector3 moveTo, float moveSpeed = 1f, 
            Transform hookCoilHolder = null, float rotateSpeed = 1f) 
            : base(current, moveTo, moveSpeed)
        {
            _hookCoilTransform = hookCoilHolder;
            _rotateSpeed = rotateSpeed;
            InitAudio();
        }

        protected override void InitAudio()
        {
            if (_hookCoilTransform != null)
            {
                _audioSource=_hookCoilTransform.GetComponent<AudioSource>();
                if (_audioSource != null)
                {
                    _audioSource.playOnAwake = false;
                    _audioSource.loop = true;
                }
            }
        }

        protected override Vector3 GetTargetPosition()
        {
            return new Vector3(_currentTransform.position.x, _target.y, _currentTransform.position.z);
        }

        protected override void HandleAdditionalActions(float deltaTime)
        {
   
            if (_hookCoilTransform != null)
                _hookCoilTransform.Rotate(_rotateSpeed * deltaTime, 0, 0, Space.Self);
        }
    }
}