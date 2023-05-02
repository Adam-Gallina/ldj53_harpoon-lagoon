using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FishShadow : MonoBehaviour
{
    [SerializeField] protected Transform shadowImage;

    [Header("Contained Fish")]
    public FishBase shadowFish;
    [SerializeField] private FishBase[] containedFish;
    public FishBase[] ContainedFish { get { return containedFish; } }
    public RangeI fishCount;

    [Header("Shadow")]
    [SerializeField] private RangeF shadowRadius;
    [SerializeField] private RangeF shadowSpeed;
    [SerializeField] private RangeF shadowDepth;
    protected float shadowRotSpeed;

    public void SetShadow(Vector3 pos)
    {
        transform.position = pos;

        shadowImage.localPosition = new Vector3(0, 
            0,//Random.Range(shadowDepth.minVal, shadowDepth.maxVal), 
            Random.Range(shadowRadius.minVal, shadowRadius.maxVal));

        transform.eulerAngles = pos + new Vector3(0, Random.Range(0, 360), 0);
        shadowRotSpeed = Random.Range(shadowSpeed.minVal, shadowSpeed.maxVal) * (Random.Range(0, 2) == 1 ? 1 : -1);

        shadowImage.GetComponentInChildren<CapsuleCollider>().center = new Vector3(0, 0, 2.5f + shadowImage.localPosition.y);

        if (shadowRotSpeed < 0)
            shadowImage.localScale = new Vector3(-1, 1, 1);
    }

    public virtual void UpdateShadow()
    {
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + shadowRotSpeed * Time.deltaTime, 0);
    }
}
