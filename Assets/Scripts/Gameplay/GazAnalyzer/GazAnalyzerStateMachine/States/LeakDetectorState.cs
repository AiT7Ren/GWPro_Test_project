using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.GazAnalyzer.StateMachine.States
{
    public class LeakDetectorState:BaseGazeOnState
    {
        private Transform _airCollector;
        private Image _displayIndicator;
        public LeakDetectorState(Transform display,Transform airCollector,Image displayIndicator) : base(display)
        {
            _airCollector=airCollector;
            _displayIndicator = displayIndicator;
        }

        public override void Update(float deltaTime, Transform transform = null)
        {
            float distance = Vector3.Distance(transform.position,_airCollector.position);
            _displayIndicator.fillAmount = ConvertDistance(distance);
        }


        public float ConvertDistance(float distance)
        {
            return 1f - Mathf.Clamp01(distance / 10f);
        }
        
        
        
        
        
    }
}