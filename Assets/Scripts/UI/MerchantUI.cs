using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MerchantUI : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private Transform ordersMenu;

    [SerializeField] private GameObject orderEntryPrefab;

    [SerializeField] private TMPro.TMP_Text sellAllBtn;
    [SerializeField] private TMPro.TMP_Text harpoonLevelText;
    [SerializeField] private TMPro.TMP_Text inventoryLevelText;

    private List<GameObject> currOrders = new List<GameObject>();

    private Merchant currMerchant;

    private void Awake()
    {
        SetMerchantMenu(false, null);
    }

    public void SetMerchantMenu(bool show, Merchant merchant = null)
    {
        menu.SetActive(show);

        currMerchant = merchant;

        if (currMerchant)
        {
            FillOrderMenu();
            UpdateOrderMenu();
            CameraController.Instance.SetCamLock(true);
        }
        else
        {
            foreach (GameObject o in currOrders) { Destroy(o); }
            currOrders.Clear();
        }
    }

    private void Update()
    {
        if (currMerchant)
            UpdateOrderMenu();

        harpoonLevelText.text = "Harpoon Damage: " + BoatUpgrades.Instance.spearDamage.Val;
        inventoryLevelText.text = "Inventory Size: " + BoatUpgrades.Instance.inventorySize.Val;
    }

    private void FillOrderMenu()
    {
        foreach (GameObject o in currOrders) { Destroy(o); }
        currOrders.Clear();

        FishType[] f = currMerchant.recievedFish.Keys.ToArray();
        for (int i = 0; i < f.Length; i++)
        {
            GameObject newEntry = Instantiate(orderEntryPrefab, ordersMenu);

            newEntry.transform.GetChild(0).GetComponent<Image>().sprite = currMerchant.desiredFishImages[i];
            newEntry.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text = Constants.GetFishName(f[i]);
            newEntry.transform.GetChild(2).GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, Random.Range(-5, 5));

            newEntry.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -20 - 70 * i, 0);

            currOrders.Add(newEntry);
        }
    }

    private void UpdateOrderMenu()
    {
        FishType[] f = currMerchant.recievedFish.Keys.ToArray();
        for (int i = 0; i < f.Length; i++)
        {
            currOrders[i].transform.GetChild(2).gameObject.SetActive(currMerchant.recievedFish[f[i]]);
        }
    }

    public void SellBtn()
    {
        GameUI.Instance.SetInventory(true, false, InventoryDisplayType.Sell, currMerchant.SellFish);
        GameUI.Instance.currWindow = UiWindow.Merchant;
        GameUI.Instance.SetCoins(true);

        sellAllBtn.text = "Sell all: " + currMerchant.TotalPrice();
    }

    public void CloseSell()
    {
        GameUI.Instance.SetInventory(false);
        GameUI.Instance.SetCoins(false);
        GameUI.Instance.currWindow = UiWindow.Merchant;
        Cursor.visible = true;
    }

    public void OrderBtn()
    {
        GameUI.Instance.SetInventory(true, false, InventoryDisplayType.Transfer, currMerchant.TryFillOrder);
        GameUI.Instance.currWindow = UiWindow.Merchant;
    }

    public void CloseOrder()
    {
        GameUI.Instance.SetInventory(false);
        GameUI.Instance.currWindow = UiWindow.Merchant;
        Cursor.visible = true;
    }

    public void LeaveBtn()
    {
        BoatController.Instance.SetBoatMode(BoatMode.Normal);
        CameraController.Instance.SetCamLock(false);
    }

    public void SellAllBtn()
    {
        currMerchant.SellAllFish();
        InventoryController.Instance.ClearInventory();
    }
}
