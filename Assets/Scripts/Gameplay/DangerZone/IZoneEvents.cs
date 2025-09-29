using System;
using UnityEngine;

public interface IZoneEvents
{
    event Action<bool> OnEnterToZone;
    public Transform GetTransform();
}