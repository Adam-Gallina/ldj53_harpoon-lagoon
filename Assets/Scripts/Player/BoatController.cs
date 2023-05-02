using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;

public enum BoatMode { Normal, Frozen, Aiming, Fishing }
public class BoatController : MonoBehaviour
{
    public static BoatController Instance;

    [SerializeField] private Transform model;
    [SerializeField] private Transform targetReticle;
    [SerializeField] private Transform spearGunBase;
    [SerializeField] private Transform spearGunBarrel;
    [SerializeField] private Transform spearSpawn;

    public Spear spearPrefab;
    private Spear currSpear;

    private bool canMove = true;
    private bool canCast = true;

    [Header("Movement")]
    [SerializeField] private float baseMoveSpeed;
    [SerializeField] private float baseTurnSpeed;
    private float moveMod = 1;
    private float turnMod = 1;

    [Header("Casting")]
    [SerializeField] private float maxAimDist;
    [SerializeField] private RangeF castHeight;
    [SerializeField] private RangeF castDur;
    [SerializeField] private float castHitRange;
    private bool casting = false;

    [Header("Model")]
    [SerializeField] private RangeF spearBarrelAimAng;

    [Header("Audio")]
    [SerializeField] private AudioClip engineIdle;
    [SerializeField] private AudioClip engineOn;
    [SerializeField] private AudioSource engine;
    public AudioSource spearLaunch;
    [SerializeField] private AudioSource spearSplash;
    [SerializeField] private AudioSource foghorn;

    private Merchant currInteraction;

    private InputController ic;
    private Rigidbody rb;

    private void Awake()
    {
        Instance = this;

        ic = GetComponent<InputController>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        currSpear = Instantiate(spearPrefab, spearSpawn);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Constants.InteractibleLayer)
        {
            currInteraction = other.GetComponent<Merchant>();
            GameUI.Instance.SetEIndicator(currInteraction.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == Constants.InteractibleLayer)
        {
            currInteraction = null;
            GameUI.Instance.SetEIndicator(null);
        }
    }

    void Update()
    {
        if (canMove)
        {
            CheckMovement();
            CheckInput();
        }
        if (canCast)
            CheckCast();

        engine.clip = (ic.left || ic.right || ic.up || ic.down) && canMove ? engineOn : engineIdle;
        if (!engine.isPlaying)
            engine.Play();
    }

    private void CheckMovement()
    {
        int x = 0, z = 0;

        if (ic.left)  x--;
        if (ic.right) x++;

        if (ic.up) z++;
        if (ic.down) z--;

        rb.AddTorque(model.up * x * baseTurnSpeed * turnMod * BoatUpgrades.Instance.boatSpeedMod.Val * Time.deltaTime);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
        rb.AddForce(model.forward * z * baseMoveSpeed * moveMod * BoatUpgrades.Instance.boatSpeedMod.Val * Time.deltaTime, ForceMode.VelocityChange);
    }
    private void CheckCast()
    {
        if (casting)
            return;

        if (ic.aim.down)
            SetBoatMode(BoatMode.Aiming);

        if (ic.aim)
        {
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100, 1 << Constants.WaterSurfaceLayer);

            Vector3 targetPoint = hit.point;
            float currAimDist = Vector3.Distance(transform.position, hit.point);
            if (currAimDist > maxAimDist)
            {
                targetPoint = transform.position + (targetPoint - transform.position).normalized * maxAimDist;
                currAimDist = maxAimDist;
            }

            spearGunBase.localEulerAngles = new Vector3(0, 0, Vector3.SignedAngle(Vector3.forward, targetPoint - spearGunBase.position, Vector3.up) + 180 - transform.localEulerAngles.y);
            spearGunBarrel.localEulerAngles = new Vector3(MyMath.PercentRangeValF(spearBarrelAimAng, currAimDist / maxAimDist), 0, 0);

            targetReticle.position = targetPoint;

            // Can't cast until previous spear lands
            if (ic.cast.down && currSpear)
            {
                float t = currAimDist / maxAimDist;

                spearLaunch.Play();
                
                currSpear.transform.parent = null;
                currSpear.CastSpearArc(currSpear.transform.position,
                                       targetPoint,
                                       MyMath.PercentRangeValF(castHeight, t),
                                       MyMath.PercentRangeValF(castDur, t),
                                       1 << Constants.FishLayer,
                                       OnSpearHit,
                                       OnSpearLand);


                casting = true;
            }
        }
        else
        {
            SetBoatMode(BoatMode.Normal);
        }
    }
    private void CheckInput()
    {
        if (ic.interact.down)
            currInteraction.OnInteract();

        if (ic.inventory.down)
            GameUI.Instance.ToggleInventory();

        if (ic.upgrades.down)
            GameUI.Instance.ToggleUpgrades();

        if (ic.horn.down)
            foghorn.Play();
        else if (ic.horn.up)
            foghorn.Stop();
    }

    public void SetBoatMode(BoatMode m)
    {
        canMove = m == BoatMode.Normal || m == BoatMode.Aiming;
        canCast = m == BoatMode.Normal || m == BoatMode.Aiming;

        if (m == BoatMode.Aiming)
            CameraController.Instance.SetFixedRotation(true, new Vector3(90, 0, 0), Constants.FixedRotationDur);
        else if (m == BoatMode.Normal)
            CameraController.Instance.StopFixedRotation();

        moveMod = m == BoatMode.Aiming ? 0.5f : 1;
        turnMod = m == BoatMode.Aiming ? 0.5f : 1;

        if (GameUI.Instance.currWindow != UiWindow.None)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = m == BoatMode.Aiming ? CursorLockMode.None : CursorLockMode.Locked;
        targetReticle.gameObject.SetActive(m == BoatMode.Aiming);
    }

    private bool OnSpearHit(Spear spear, Transform target)
    {
        casting = false;
        currSpear = Instantiate(spearPrefab, spearSpawn);

        FishShadow fish = target.GetComponentInParent<FishShadow>();
        if (fish)
        {
            FishingController.Instance.StartFishing(fish);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        spearSplash.Play();

        return true;
    }

    private bool OnSpearLand(Spear spear)
    {
        casting = false;
        currSpear = Instantiate(spearPrefab, spearSpawn);

        spear.StopAllCoroutines();
        spear.CastSpearLinear(spear.transform.position, Vector3.down, BoatUpgrades.Instance.spearSpeed.Val, 0);

        spearSplash.Play();

        return false;
    }
}
