using System;

namespace Helpers
{
    public class FloatReactProperty : IFloatReactProperty
    {
        private float _currentSpeed;
        public event Action<float> OnSpeedChange;

        public FloatReactProperty(float speed)
        {
            _currentSpeed = speed;
        }
        public float CurrentSpeed
        {
            get => _currentSpeed;
            set
            {
                if (_currentSpeed != value)
                {
                    if (value <= 0)
                    {
                        throw new Exception("CurrentSpeed not been 0 and less" );
                    }
                    _currentSpeed = value;
                    OnSpeedChange?.Invoke(_currentSpeed);
                }
            }
        }
    }
}