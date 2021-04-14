using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WarEntity : GameBehavior
{
    WarFactory originWarFactory;
    public WarFactory OriginWarFactory
    {
        get => originWarFactory;
        set
        {
            Debug.Assert(originWarFactory == null, "Redefined oriign factiry");
            originWarFactory = value;
        }
    }

    public void Recycle()
    {
        OriginWarFactory.ReClanim(this);
    }

}