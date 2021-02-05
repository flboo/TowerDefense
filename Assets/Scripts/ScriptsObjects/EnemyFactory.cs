using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyFactory : GameObjectFactory
{
    [SerializeField]
    Enemy prefab = default;

    public Enemy Get()
    {
        Enemy instance = CreateGameObjectInstance(prefab);
        instance.OriginFactory = this;
        return instance;
    }

    public void ReClanim(Enemy enemy)
    {
        Debug.Assert(enemy == this, "wrong factory reclaim");
        Destroy(enemy.gameObject);
    }


}
