using TMPro;
using UnityEngine;

namespace Gameplay.GazAnalyzer.StateMachine.States
{
    public class ScaneDetectorState:BaseGazeOnState
    {
        private TextMeshProUGUI _indicatorText;
        private RectTransform  _indicatorArrow;

        public ScaneDetectorState(Transform display,RectTransform indicatorArrow,TextMeshProUGUI indicatorText) : base(display)
        {
            _indicatorArrow= indicatorArrow;
            _indicatorText = indicatorText;
        }

        public override void Update(float deltaTime, Transform transform = null)
        {
           if(transform != null)UICanculate(transform);
        }
        private void UICanculate(Transform target)
        {
            Vector3 distV3 = _dispaly.position - target.position; //<= наверно лучше передавать корнивой обьект 
            float distF =  distV3.magnitude;
            _indicatorText.text = distF.ToString("F1");
            float angleRot = Mathf.Atan2(distV3.x, distV3.z) * Mathf.Rad2Deg+90f; 
            _indicatorArrow.rotation= Quaternion.Euler(0,0,angleRot);
        }
        
        
    }
}