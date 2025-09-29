using UnityEngine;
using UnityEngine.Events;
namespace Helpers
{
    public class EventProper
    {
        
    }
    [System.Serializable]
    public class BoolUnityEvent : UnityEvent<bool> {}
    [System.Serializable]
    public class FloatUnityEvent : UnityEvent<float> {}
    [System.Serializable]
    public class StringUnityEvent : UnityEvent<string> {}
}