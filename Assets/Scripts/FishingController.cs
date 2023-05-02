using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class FishingController : MonoBehaviour
{
    public static FishingController Instance;

    [SerializeField] private Transform targetIndicator;
    [SerializeField] private Transform targetWall;

    private FishBase hookedFish;
    private List<FishBase> spawnedFish = new List<FishBase>();

    [Header("Fish Spawning")]
    [SerializeField] private RangeF bounds;

    [Header("Spear Casting")]
    [SerializeField] private float spearCooldown = .5f;
    [SerializeField] public int castSpears = 0;
    private float nextSpear;

    private bool fishing = false;

    private BoatController bc;
    private InputController ic;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        bc = BoatController.Instance;
        ic = bc.GetComponent<InputController>();
    }

    public void StartFishing(FishShadow hitFish)
    {
        BoatController.Instance.SetBoatMode(BoatMode.Frozen);

        CameraController.Instance.SetFixedRotation(true, new Vector3(10, 90, 0), Constants.FixedRotationDur);
        CameraController.Instance.SetCameraOffset(new Vector3(0, -5f, -5));
        Cursor.lockState = CursorLockMode.None;

        FishSpawner.Instance.SetPools(false);
        FishSpawner.Instance.RemoveShadow(hitFish);

        RangeF hookedRange = hitFish.shadowFish.depth;
        hookedRange.minVal = MyMath.PercentRangeValF(hookedRange, 0.5f);
        hookedFish = SpawnFish(hitFish.shadowFish);
        SpawnAllFish(hitFish.ContainedFish, MyMath.RandomRangeI(hitFish.fishCount));

        hookedFish.Damage((int)BoatUpgrades.Instance.spearDamage.Val);
        Spear hookedSpear = Instantiate(bc.spearPrefab, hookedFish.transform);
        hookedSpear.transform.localEulerAngles = new Vector3(180, Random.Range(0, 360), Random.Range(-90, 90));
        hookedSpear.transform.localScale = new Vector3(2, 2, 2);

        GameUI.Instance.SetStress(hookedFish);
        GameUI.Instance.SetSpearCount(true);

        targetIndicator.gameObject.SetActive(true);
        targetIndicator.forward = bc.transform.right;
        targetWall.gameObject.SetActive(true);
        targetWall.up = -bc.transform.right;
        targetWall.position = bc.transform.position;

        castSpears = 0;

        fishing = true;
    }

    public void StopFishing()
    {
        Destroy(hookedFish.gameObject);
        foreach (FishBase f in spawnedFish)
            Destroy(f.gameObject);
        spawnedFish.Clear();

        CameraController.Instance.StopFixedRotation();
        CameraController.Instance.ClearCameraOffset();
        BoatController.Instance.SetBoatMode(BoatMode.Normal);
        GameUI.Instance.SetStress(null);
        GameUI.Instance.SetSpearCount(false);

        FishSpawner.Instance.SetPools(true);

        targetIndicator.gameObject.SetActive(false);
        targetWall.gameObject.SetActive(false);

        fishing = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            StopFishing();

        if (fishing)
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100, 1 << Constants.FishingWallLayer);
        Vector3 targetPoint = hit.point;
        targetIndicator.position = targetPoint;

        if (ic.cast.down && Time.time > nextSpear && castSpears < BoatUpgrades.Instance.spearCount.Val)
        {
            BoatController.Instance.spearLaunch.Play();

            nextSpear = Time.time + spearCooldown;
            castSpears++;
            Spear spear = Instantiate(bc.spearPrefab);
            spear.CastSpearLinear(bc.transform.position, 
                                targetPoint - bc.transform.position,
                                BoatUpgrades.Instance.spearSpeed.Val,
                                1 << Constants.FishLayer,
                                OnSpearHit);
        }

        if (ic.aim)
        {
            switch (hookedFish.PullFish(BoatUpgrades.Instance.pullSpeed.Val))
            {
                case PullResult.Caught:
                    InventoryController.Instance.AddFish(hookedFish.vals);
                    GameUI.Instance.SetCatchFish(hookedFish);
                    Cursor.visible = true;
                    fishing = false;
                    break;
                case PullResult.Broke:
                    GameUI.Instance.SetBreakFish(hookedFish);
                    Cursor.visible = true;
                    fishing = false;
                    break;
            }
        }
        else
        {
            hookedFish.StopPullFish();
        }
    }

    private void SpawnAllFish(FishBase[] fish, int count)
    {
        for (int i = 0; i < count; i++)
        {
            spawnedFish.Add(SpawnFish(fish[Random.Range(0, fish.Length)]));
        }
    }

    private FishBase SpawnFish(FishBase fish, RangeF? depth = null)
    {
        if (depth == null)
            depth = fish.depth;

        FishBase f = Instantiate(fish);

        Vector2 spawnPos = new Vector2(MyMath.RandomRangeF(bounds), MyMath.RandomRangeF(depth.Value));
        f.transform.position = bc.transform.position
                             + bc.transform.forward * spawnPos.x
                             + bc.transform.up * spawnPos.y;
        f.transform.forward = bc.transform.right;

        f.SetSwimBounds(bc.transform.position + bc.transform.forward * bounds.minVal,
                        bc.transform.position + bc.transform.forward * bounds.maxVal,
                        spawnPos.y );

        return f;
    }

    private bool OnSpearHit(Spear spear, Transform target)
    {
        FishBase fish = target.GetComponent<FishBase>();
        if (!fish)
            return false;

        if (spear.transform.position.y < target.position.y)
        {
            spear.CastSpearLinear(spear.transform.position,
                                  spear.transform.up,
                                  BoatUpgrades.Instance.spearSpeed.Val,
                                  1 << Constants.FishLayer,
                                  OnSpearHit);
            return false;
        }

        spear.transform.parent = target;
        spear.StopCastCoroutine();
        fish.Damage((int)BoatUpgrades.Instance.spearDamage.Val);

        return false;
    }
}
