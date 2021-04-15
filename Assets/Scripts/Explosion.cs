using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : WarEntity
{
    [SerializeField]
    AnimationCurve opacityCurve = default;
    [SerializeField]
    AnimationCurve scaleCurve = default;
    static int colorPropetyID = Shader.PropertyToID("_Color");
    static MaterialPropertyBlock propertyBlock;

    [SerializeField, Range(0f, 1f)]
    float duration = 0.5f;
    float age;
    float scale;
    MeshRenderer meshRender;

    void Awake()
    {
        meshRender = GetComponent<MeshRenderer>();
        Debug.Assert(meshRender != null, "Explosion without renderer");
    }

    public void Initialize(Vector3 position, float blastRadius, float damage = 0f)
    {
        if (damage > 0)
        {
            TargetPoint.FillBuffer(position, blastRadius);
            for (int i = 0; i < TargetPoint.BufferedCount; i++)
            {
                TargetPoint.GetBuffered(i).Enemy.ApplyDamage(damage);
            }
        }
        transform.localPosition = position;
        // transform.localScale = Vector3.one * (2f * blastRadius);
        scale = 2f * blastRadius;
    }

    public override bool GameUpdate()
    {
        age += Time.deltaTime;
        if (age >= duration)
        {
            OriginWarFactory.ReClanim(this);
            return false;
        }

        if (propertyBlock == null)
        {
            propertyBlock = new MaterialPropertyBlock();
        }

        float t = age / duration;
        Color c = Color.clear;
        c.a = opacityCurve.Evaluate(t);
        propertyBlock.SetColor(colorPropetyID, c);
        meshRender.SetPropertyBlock(propertyBlock);
        transform.localScale = Vector3.one * (scale * scaleCurve.Evaluate(t));
        return true;
    }




}
