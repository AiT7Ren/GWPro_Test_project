using UnityEngine;

namespace Gameplay.CraneControll.StateMachine.States.Upd
{
    public class BeamMoverZ : BaseCraneMover
    {
        public BeamMoverZ(Transform current, Vector3 moveTo, float moveSpeed = 1f)
            : base(current, moveTo, moveSpeed)
        {
            InitAudio();
        }

        protected override Vector3 GetTargetPosition()
        {
            return new Vector3(_currentTransform.position.x, _currentTransform.position.y, _target.z);
        }
    }
}