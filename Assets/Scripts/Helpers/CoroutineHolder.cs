using System;
using UnityEngine;
namespace Helpers
{
    public class CoroutineHolder : MonoBehaviour
    {
        public static CoroutineHolder Instance;
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else
            {
                throw new Exception($"Instance already created {gameObject.name}");
            }
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}