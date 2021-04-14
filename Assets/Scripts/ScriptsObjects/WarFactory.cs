using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class WarFactory : GameObjectFactory
{

    [SerializeField]
    private Shell shellPrefab = default;
    public Shell Shell => Get(shellPrefab);

    T Get<T>(T prefab) where T : WarEntity
    {
        T instance = CreateGameObjectInstance(prefab);
        instance.OriginWarFactory = this;
        return instance;
    }

    public void ReClanim(WarEntity entity)
    {
        Debug.Assert(entity.OriginWarFactory == this, "Wrong factory reclacimed!");
        Destroy(entity.gameObject);
    }

}
