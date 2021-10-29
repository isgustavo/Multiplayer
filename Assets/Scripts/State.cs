using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class State
{
    protected Transform transform;

    protected State (Transform transform)
    {
        this.transform = transform;
    }

    public virtual void OnEnterState (State previousState = null) { }
    public virtual void UpdateState () { }
    public virtual void FixedUpdateState () { }
    public virtual void LateUpdateState () { }
    public virtual void OnLeaveState () { }
}
