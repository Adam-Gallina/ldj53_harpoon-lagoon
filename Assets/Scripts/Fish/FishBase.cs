using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PullResult { None, Broke, Caught }
public abstract class FishBase : MonoBehaviour
{
    public RangeF depth;
    [SerializeField] protected int health = 2;
    protected bool dead = false;

    [Header("Fish Vals")]
    [SerializeField] private FishValRanges valRanges;
    [HideInInspector] public FishVals vals;

    [Header("Image")]
    [SerializeField] protected RangeF scale;

    [Header("Swimming")]
    [SerializeField] protected RangeF swimSpeed;
    protected float speed;
    protected Vector3 minPos;
    protected Vector3 maxPos;
    protected int dir;
    protected float currDepth;
    protected Vector3 targetPos;

    [Header("Fishing")]
    [SerializeField] protected float pullStrength;
    [HideInInspector] public float ropeStress;
    [SerializeField] protected float ropeDestressSpeed;
    protected bool pulled = false;

    protected bool movingFromBoat = false;
    private float lastDist;

    private void Awake()
    {
        CalcFishVals();
    }

    private void CalcFishVals()
    {
        vals = new FishVals();
        vals.fishType = valRanges.fishType;
        float t = Random.Range(0f, 10f) / 10f;
        vals.size = Mathf.Round(MyMath.PercentRangeValF(valRanges.size, t) * 10) / 10;
        vals.valMod = valRanges.valMod;
        vals.image = valRanges.image;

        float s = MyMath.PercentRangeValF(scale, t);
        transform.localScale = new Vector3(s, s, 1);
    }

    public void SetSwimBounds(Vector3 minPos, Vector3 maxPos, float depth)
    {
        speed = MyMath.RandomRangeF(swimSpeed);

        this.minPos = minPos;
        this.maxPos = maxPos;

        dir = Random.Range(0, 2) == 0 ? 1 : -1;
        currDepth = depth;

        lastDist = Vector3.Distance(transform.position, BoatController.Instance.transform.position);

        UpdateTargetPos();
    }

    private void Update()
    {
        if (!dead)
        {
            UpdateSwimming();

            if (!pulled && ropeStress > 0)
                ropeStress -= ropeDestressSpeed * Time.deltaTime;
        } 
        else
        {
            transform.position = new Vector3(transform.position.x, currDepth, transform.position.z);
        }
    }

    private void LateUpdate()
    {
        lastDist = Vector3.Distance(transform.position, BoatController.Instance.transform.position);
    }

    protected virtual void UpdateSwimming()
    {
        if (Vector3.Distance(transform.position, targetPos) < .1f)
            UpdateTargetPos();

        targetPos.y = currDepth;
        transform.position = new Vector3(transform.position.x, currDepth, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        movingFromBoat = Vector3.Distance(transform.position, BoatController.Instance.transform.position) > lastDist;
    }

    public virtual PullResult PullFish(float speed)
    {
        pulled = true;

        currDepth += speed * Time.deltaTime * (dead ? Constants.DeadPullMod : 1);

        if (currDepth >= Constants.MinCatchDepth)
            return PullResult.Caught;

        if (movingFromBoat && !dead)
        {
            ropeStress += pullStrength * Time.deltaTime;

            if (ropeStress > BoatUpgrades.Instance.ropeStrength.Val)
                return PullResult.Broke;
        }

        return PullResult.None;
    }
    
    public virtual void StopPullFish()
    {
        pulled = false;
    }

    protected void UpdateTargetPos()
    {
        targetPos = GetTargetPos();
        transform.localScale = new Vector3(dir * Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        dir *= -1;
    }

    protected virtual Vector3 GetTargetPos()
    {
        RangeF t;
        t.minVal = dir == 1 ? 0 : 0.5f;
        t.maxVal = dir == 1 ? 0.5f : 1;

        return minPos + (maxPos - minPos) * MyMath.RandomRangeF(t) + Vector3.up * currDepth;
    }

    public void Damage(int amount)
    {
        health -= amount;

        if (health <= 0 && !dead)
            Death();
    }

    private void Death()
    {
        dead = true;
        transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, 1);
        movingFromBoat = false;
        GetComponent<Collider>().enabled = false;
    }
}

[System.Serializable]
public struct FishValRanges
{
    public FishType fishType;
    public RangeF size;
    public float valMod;
    public Sprite image;
}
[System.Serializable]
public struct FishVals
{
    public FishType fishType;
    public float size;
    public float valMod;
    public Sprite image;
}