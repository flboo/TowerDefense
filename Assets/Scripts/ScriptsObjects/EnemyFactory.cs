using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyFactory : GameObjectFactory
{
    [SerializeField]
    Enemy prefab = default;
    [SerializeField, FloatRangeSlider(0.5f, 2.0f)]
    private FloatRange scale = new FloatRange(1f);
    [SerializeField, FloatRangeSlider(-0.4f, 0.4f)]
    FloatRange pathOffset = new FloatRange(0f);
    [SerializeField, FloatRangeSlider(0.2f, 5f)]
    FloatRange speed = new FloatRange(0);

    public Enemy Get()
    {
        Enemy instance = CreateGameObjectInstance(prefab);
        instance.OriginFactory = this;
        instance.Initialize(scale.RangeValueRange, speed.RangeValueRange, pathOffset.RangeValueRange);
        return instance;
    }

    public void ReClanim(Enemy enemy)
    {
        Debug.Assert(enemy.OriginFactory == this, "wrong factory reclaimd ! ");
        Destroy(enemy.gameObject);
    }

}