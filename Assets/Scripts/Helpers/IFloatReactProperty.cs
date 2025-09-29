using System;

namespace Helpers
{
    public interface IFloatReactProperty
    {
        event Action<float> OnSpeedChange;
        float CurrentSpeed { get; set; }
    }
}