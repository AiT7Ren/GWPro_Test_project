using System;
using UnityEngine;
namespace Stuff
{
    public class AutoRopeBuilder : MonoBehaviour
    {
        [SerializeField] private Transform _startPoint;
        [SerializeField] private Transform _endPoint;
        [SerializeField] private float _ropeRadius = 0.03f;
        [SerializeField] private Material _ropeMaterial;
        [SerializeField] private Color _color = Color.gray;
        [SerializeField] private bool _drawGizmos = true;
      
        private GameObject _ropeObject;
        private SpringJoint _springJoint;
       

        private void Start()
        {
            if (_ropeObject != null)
            {
                Destroy(_ropeObject);
                _ropeObject = null;
            }
            CreateRope();
        }
        private void Update()
        {
            UpdateRope();
        }
        [ContextMenu("CreateRope")]
        private void CreateRope()
        {
            if (_startPoint == null || _endPoint == null) throw new Exception("No rope ankers, cant create rope");
            _ropeObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            _ropeObject.name = "Wire";
            _ropeObject.transform.parent = transform;
            Destroy(_ropeObject.GetComponent<Collider>());
            Renderer renderer = _ropeObject.GetComponent<Renderer>();
            if (_ropeMaterial != null) renderer.material = _ropeMaterial;
            UpdateRope();
        }
        private void UpdateRope()
        {
            if (_ropeObject == null) return;
            Vector3 startPos = _startPoint.position;
            Vector3 endPos = _endPoint.position;

            Vector3 center = (startPos + endPos) * 0.5f;
            _ropeObject.transform.position = center;
            Vector3 direction = endPos - startPos;
            if (direction != Vector3.zero)
            {
                _ropeObject.transform.rotation = Quaternion.LookRotation(direction);
                _ropeObject.transform.Rotate(90, 0, 0);
            }
            var distance = Vector3.Distance(startPos, endPos);
            _ropeObject.transform.localScale = new Vector3(_ropeRadius * 2, distance * 0.5f, _ropeRadius * 2);
        }
        private void OnDrawGizmos()
        {
            if (_drawGizmos)
                if (_startPoint != null && _endPoint != null)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(_startPoint.position, _endPoint.position);

                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(_startPoint.position, 0.1f);
                    Gizmos.DrawWireSphere(_endPoint.position, 0.1f);
                }
        }
    }
}