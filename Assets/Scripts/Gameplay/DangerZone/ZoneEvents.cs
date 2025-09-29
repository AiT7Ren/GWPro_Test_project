using System;
using UnityEngine;

public class ZoneEvents : MonoBehaviour, IZoneEvents
{
    public event Action<bool> OnEnterToZone;
    public Transform GetTransform()
    {
        return this.transform;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")   //лучше переделать на getComponent
        {
            OnEnterToZone?.Invoke(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            OnEnterToZone?.Invoke(false);
        }
    }
}
