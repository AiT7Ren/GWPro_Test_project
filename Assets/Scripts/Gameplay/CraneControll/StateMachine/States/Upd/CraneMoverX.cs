using UnityEngine;

namespace Gameplay.CraneControll.StateMachine.States.Upd
{
    public class CraneMoverX:BaseCraneMover
    {
        public CraneMoverX(Transform current, Vector3 moveTo, float moveSpeed = 1f)
            : base(current, moveTo, moveSpeed)
        {
            InitAudio();
        }

        protected override Vector3 GetTargetPosition()
        {
            return new Vector3(_target.x, _currentTransform.position.y, _currentTransform.position.z);
        }
    }
}