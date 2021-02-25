using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPoint : MonoBehaviour
{

    public Enemy Enemy { get; private set; }

    public Vector3 Position => transform.position;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        Enemy = transform.root.GetComponent<Enemy>();
        Debug.Assert(Enemy != null, "Taret point without enemy root", this);
        Debug.Assert(GetComponent<SphereCollider>() != null, "Target point without sphere collider!", this);
    }


}
