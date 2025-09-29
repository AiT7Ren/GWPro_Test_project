using System;
using UnityEngine;

public interface IDangerZoneHolder
{
    event Action<bool> OnDangerZone;
    Transform GetNearlyZone(Transform target);
}