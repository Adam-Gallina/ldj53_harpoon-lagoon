using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UiWindow { None, Inventory, Merchant, Upgrades }
public class GameUI : MonoBehaviour
{
    public static GameUI Instance;

    [HideInInspector] public UiWindow currWindow = UiWindow.None;

    [SerializeField] private InventoryUI invUI;
    [SerializeField] private MerchantUI merchantUI;
    [SerializeField] private GameObject coinUI;
    [SerializeField] private RectTransform eIndicator;
    private Transform eTarget;
    [SerializeField] private Vector3 eOffset;
    [SerializeField] private GameObject stressUI;
    [SerializeField] private GameObject inventoryWarning;

    [SerializeField] private TMPro.TMP_Text coinText;
    [SerializeField] private TMPro.TMP_Text stressText;
    private FishBase currFish;

    public GameObject upgradeUI;

    [SerializeField] private GameObject breakMenu;
    [SerializeField] private TMPro.TMP_Text breakText;
    [SerializeField] private GameObject catchMenu;
    [SerializeField] private Image catchImage;
    [SerializeField] private TMPro.TMP_Text catchNameText;
    [SerializeField] private TMPro.TMP_Text catchSizeText;

    [SerializeField] private GameObject spearUI;
    [SerializeField] private TMPro.TMP_Text spearText;

    private void Awake()
    {
        Instance = this;

        SetCoins(false);
        SetEIndicator(null);
        SetStress(null);
        SetBreakFish(null);
        SetCatchFish(null);
        SetInventoryWarning(false);
        SetSpearCount(false);
    }

    private void Update()
    {
        coinText.text = "Coins: " + InventoryController.Instance.coins;

        if (currFish)
            stressText.text = "Rope stress: " + Mathf.Round(currFish.ropeStress / BoatUpgrades.Instance.ropeStrength.Val * 100) + "%";

        spearText.text = "Harpoons: " + ((int)BoatUpgrades.Instance.spearCount.Val - FishingController.Instance.castSpears);

        SetInventoryWarning(coinUI.activeSelf ? false : InventoryController.Instance.inventory.Count >= BoatUpgrades.Instance.inventorySize.Val);

        if (eTarget)
            eIndicator.position = Camera.main.WorldToScreenPoint(eTarget.position) + eOffset;
    }

    public void SetSpearCount(bool show)
    {
        spearUI.SetActive(show);
    }

    public void ToggleUpgrades()
    {
        bool invVis = currWindow == UiWindow.Upgrades;

        if (currWindow != UiWindow.None)
            if (!CloseCurrWindow())
                return;

        SetUpgrades(!invVis);
    }
    public void SetUpgrades(bool show)
    {
        currWindow = show ? UiWindow.Upgrades : UiWindow.None;

        upgradeUI.SetActive(show);
        SetCoins(show);

        Cursor.visible = show;
    }

    public void SetCoins(bool show)
    {
        coinUI.SetActive(show);
    }

    public void SetStress(FishBase fish = null)
    {
        stressUI.SetActive(fish != null);
        currFish = fish;
    }

    public void SetInventoryWarning(bool show)
    {
        inventoryWarning.SetActive(show);
    }

    public void SetEIndicator(Transform target)
    {
        eIndicator.gameObject.SetActive(target != null);
        eTarget = target;
    }

    public void ToggleInventory()
    {
        bool invVis = currWindow == UiWindow.Inventory;

        if (currWindow != UiWindow.None)
            if (!CloseCurrWindow())
                return;

        SetInventory(!invVis);
    }
    public void SetInventory(bool show, bool closeBtn = true, InventoryDisplayType display = InventoryDisplayType.Discard, Func<FishVals, bool> cb = null)
    {
        currWindow = show ? UiWindow.Inventory : UiWindow.None;
        invUI.SetInventoryMenu(show, closeBtn, display, cb);

        Cursor.visible = show;
    }

    public void CloseMerchant()
    {
        SetMerchant(false);
    }
    public void SetMerchant(bool show, Merchant merchant = null)
    {
        if (currWindow != UiWindow.None)
            CloseCurrWindow();

        currWindow = show ? UiWindow.Merchant : UiWindow.None;
        merchantUI.SetMerchantMenu(show, merchant);

        Cursor.visible = show;
    }

    private bool CloseCurrWindow()
    {
        switch (currWindow)
        {
            case UiWindow.Inventory:
                SetInventory(false);
                break;
            case UiWindow.Merchant:
                return false;
            case UiWindow.Upgrades:
                SetUpgrades(false);
                break;
        }

        currWindow = UiWindow.None;

        Cursor.visible = false;

        return true;
    }

    public void SetBreakFish(FishBase f = null)
    {
        breakMenu.SetActive(f != null);
        if (f)
            breakText.text = Constants.GetFishName(f.vals.fishType) + " got away";
    }

    public void SetCatchFish(FishBase f = null)
    {
        catchMenu.SetActive(f != null);
        if (f)
        {
            catchImage.sprite = f.vals.image;
            catchNameText.text = "You caught a " + Constants.GetFishName(f.vals.fishType) + "!";
            catchSizeText.text = "Size: " + Mathf.Round(f.vals.size * 10) / 10 + "in";
        }
    }
}
