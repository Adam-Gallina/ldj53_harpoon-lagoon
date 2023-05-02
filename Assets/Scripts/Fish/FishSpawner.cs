using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public static FishSpawner Instance;

    [Header("Spawning")]
    [SerializeField] private RangeF spawnRadius;
    [SerializeField] private int maxSpawnedPools;
    [SerializeField] private float maxPoolLifetime;

    [Header("Fish Pools")]
    [SerializeField] private RangeI fishCount;
    [SerializeField] private PoolShadow[] poolShadows;
    private int maxSpawnChance;
    private float nextSpawn;
    private bool poolsActive = true;

    private List<FishPool> pools = new List<FishPool>();

    private void Awake()
    {
        Instance = this;

        foreach (PoolShadow p in poolShadows)
            maxSpawnChance += p.spawnChance;
    }

    private void Start()
    {
        RangeF tempRange = spawnRadius;
        tempRange.minVal = 0;
        for (int i = 0; i < maxSpawnedPools / 3; i++)
        {
            Vector2 pos = Random.insideUnitCircle;
            Vector3 dir = new Vector3(pos.x, 0, pos.y).normalized;
            pools.Add(SpawnPool(BoatController.Instance.transform.position + dir * MyMath.RandomRangeF(tempRange)));
        }
    }

    void Update()
    {
        if (poolsActive)
        {
            CheckPools();

            if (pools.Count < maxSpawnedPools && Time.time > nextSpawn)
            {
                nextSpawn = Time.time + 3;
                Vector3 dir = BoatController.Instance.transform.forward * 3 + BoatController.Instance.transform.right * Random.Range(-10, 10);
                pools.Add(SpawnPool(BoatController.Instance.transform.position + dir.normalized * MyMath.RandomRangeF(spawnRadius)));
            }

            UpdatePools();
        }
    }

    private FishPool SpawnPool(Vector3 position)
    {
        FishPool newPool = new FishPool(position);

        int c = Random.Range(fishCount.minVal, fishCount.maxVal);
        for (int i = 0; i < c; i++)
        {
            newPool.AddFish(Instantiate(GetRandomFish()));
        }

        return newPool;
    }

    private FishShadow GetRandomFish()
    {
        int n = Random.Range(0, maxSpawnChance);

        foreach (PoolShadow p in poolShadows)
        {
            if ((n -= p.spawnChance) < 0)
                return p.shadow;
        }

        return poolShadows[0].shadow;
    }

    private void UpdatePools()
    {
        if (poolsActive)
            foreach (FishPool fs in pools)
                fs.UpdateFish();
    }

    private void CheckPools()
    {
        List<FishPool> despawn = new List<FishPool>();
        foreach (FishPool pool in pools)
        {
            if (pool.spawnTime + maxPoolLifetime < Time.time ||
                Vector3.Distance(pool.poolCenter, BoatController.Instance.transform.position) > spawnRadius.maxVal)
            {
                despawn.Add(pool);
                continue;
            }
        }

        foreach (FishPool pool in despawn)
        {
            foreach (FishShadow fish in pool.fish)
                Destroy(fish.gameObject);
            pools.Remove(pool);
        }
    }

    public void SetPools(bool active)
    {
        poolsActive = active;

        foreach (FishPool p in pools)
        {
            foreach (FishShadow f in p.fish)
            {
                f.gameObject.SetActive(active);
            }
        }
    }

    public void RemoveShadow(FishShadow fish)
    {
        foreach (FishPool p in pools)
            if (p.RemoveFish(fish))
                break;

        Destroy(fish.gameObject);
    }
}

public class FishPool
{
    public List<FishShadow> fish = new List<FishShadow>();

    public Vector3 poolCenter;

    public float spawnTime;

    public FishPool(Vector3 position)
    {
        poolCenter = position;
        spawnTime = Time.time;
    }

    public void AddFish(FishShadow f)
    {
        fish.Add(f);
        f.SetShadow(poolCenter);
    }

    public void UpdateFish()
    {
        foreach (FishShadow f in fish)
            f.UpdateShadow();
    }

    public bool RemoveFish(FishShadow f)
    {
        if (fish.Contains(f))
        {
            fish.Remove(f);
            return true;
        }

        return false;
    }
}

[System.Serializable]
public struct PoolShadow
{
    public FishShadow shadow;
    public int spawnChance;
}