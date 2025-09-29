using System;
using System.Collections.Generic;
using Gameplay.CraneControll.StateMachine.States.Old;
using UnityEngine;

public class CraneStateBulder
{
    private Dictionary<string, CraneMoverX> _moveStates;
    public CraneStateBulder(Transform hook,Transform craneBase,Transform beamHolder,Transform ULFClamp,Transform DRBClamp,float LRSpeed,float BFSpeed)
    {
        if(hook == null)throw new Exception("The hook is null");
        CreateState(hook, craneBase, beamHolder, ULFClamp, DRBClamp, LRSpeed, BFSpeed);
    }
    
    private void CreateState(Transform hook,Transform craneBase,Transform beamHolder,Transform ULFClamp,Transform DRBClamp,float LRSpeed,float BFSpeed)
    {
        _moveStates.Add(CraneStateName.HOOK_UP,new CraneMoverX(hook,new Vector3(hook.position.x, ULFClamp.position.y,hook.position.z)));
        _moveStates.Add(CraneStateName.HOOK_DOWN,new CraneMoverX(hook,new Vector3(hook.position.x, DRBClamp.position.y,hook.position.z)));

        _moveStates.Add(CraneStateName.CRANE_LEFT,new CraneMoverX(craneBase,new Vector3(ULFClamp.position.x,craneBase.position.y,craneBase.position.z), LRSpeed));
        _moveStates.Add(CraneStateName.CRANE_RIGHT, new CraneMoverX(craneBase, new Vector3(DRBClamp.position.x, craneBase.position.y,craneBase.position.z), LRSpeed));

        _moveStates.Add(CraneStateName.CRANE_FORWARD,new CraneMoverX(beamHolder,new Vector3(beamHolder.position.x,beamHolder.position.y, ULFClamp.position.z), BFSpeed));
        _moveStates.Add(CraneStateName.CRANE_BACKWARD, new CraneMoverX(beamHolder, new Vector3(beamHolder.position.x, beamHolder.position.y, DRBClamp.position.z), BFSpeed));
    }

    public Dictionary<string, CraneMoverX> GetStates()
    {
        return _moveStates;
    }
}