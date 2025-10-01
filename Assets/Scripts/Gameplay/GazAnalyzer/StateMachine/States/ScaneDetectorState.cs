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
           if(transform != null)UICancilateWith3dArrow(transform);
        }
        private void UICanculate(Transform target)
        {
            Vector3 distV3 = target.position - _dispaly.position; //<= наверно лучше передавать корнивой обьект 
            float distF =  distV3.magnitude;
            _indicatorText.text = distF.ToString("F1"); 
            float angleRot = Mathf.Atan2(distV3.x, distV3.z) * Mathf.Rad2Deg+90f; 
            _indicatorArrow.rotation= Quaternion.Euler(0,0,angleRot);
        }

        private void UICancilateWith3dArrow(Transform target)
        {
            Vector3 distV3 = target.position - _dispaly.position;
            float distF = distV3.magnitude;
            _indicatorText.text = distF.ToString("F1");

            Vector3 directionToTarget = target.position - _indicatorArrow.position;
            // float angleX = -Mathf.Atan2(directionToTarget.y, Mathf.Sqrt(directionToTarget.x * directionToTarget.x + directionToTarget.z * directionToTarget.z)) * Mathf.Rad2Deg;
            // float angleY = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
            directionToTarget.x = directionToTarget.x;
            directionToTarget.y=  directionToTarget.y;
            directionToTarget.z = directionToTarget.z;
            _indicatorArrow.rotation = Quaternion.LookRotation(directionToTarget);
        }
        
        
        
    }
}