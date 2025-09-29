using System;
using System.Collections.Generic;
using UnityEngine;

public class DangerZoneHolder : MonoBehaviour, IDangerZoneHolder
{
   public static DangerZoneHolder Instance;
   private List<IZoneEvents> _dangersZones;
   public event Action<bool> OnDangerZone;

   private void Awake()
   {
      if (Instance == null) Instance = this;
      else throw new Exception("DangerZone holder already exists");
      Init();
   }
  private  void Init()
   {
      _dangersZones = new List<IZoneEvents>();
      var zones = GetComponentsInChildren<IZoneEvents>();
      if(zones.Length==0||zones==null) throw new Exception("No zone on scene");
      _dangersZones.AddRange(zones);
      foreach (var zone in _dangersZones)
      {
         zone.OnEnterToZone += HashDangerZoneEvent;
      }
   }

   public Transform GetNearlyZone(Transform target)
   {
      float nearestDistance=Mathf.Infinity;
      int nearestIndex = 0;
      if (_dangersZones.Count > 0)
      {
         for (int i = 0; i < _dangersZones.Count; i++)
         {
            float tempNearestDistance = Vector3.Distance(_dangersZones[i].GetTransform().position, target.position);
            if (tempNearestDistance < nearestDistance)
            {
               nearestDistance = tempNearestDistance;
               nearestIndex = i;
            }
         }
      }
      return _dangersZones[nearestIndex]!=null?_dangersZones[nearestIndex].GetTransform() :throw new Exception("DangerZone not found list outRange ");
   }
   private void HashDangerZoneEvent(bool isInDanger)
   {
      OnDangerZone?.Invoke(isInDanger);
   }
   
   private void OnDestroy()
   {
      foreach (var zone in _dangersZones)
      {
         zone.OnEnterToZone -= HashDangerZoneEvent;
      }
   }
}
