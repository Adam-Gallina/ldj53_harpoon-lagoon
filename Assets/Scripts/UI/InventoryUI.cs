using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum InventoryDisplayType { None, Discard, Sell, Transfer }
public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private RectTransform scrollRectContent;
    [SerializeField] private GameObject closeBtn;

    [SerializeField] private GameObject inventoryEntryPrefab;

    private List<GameObject> currInv = new List<GameObject>();
    private InventoryDisplayType currDisplayType = InventoryDisplayType.None;
    private Func<FishVals, bool> currCallback = null;

    private void Awake()
    {
        SetInventoryMenu(false, true, currDisplayType, currCallback);
    }

    private void Update()
    {
        if (scrollRectContent.childCount != InventoryController.Instance.inventory.Count)
            ReloadInventory();
    }

    public void SetInventoryMenu(bool show, bool showCloseBtn = true, InventoryDisplayType display = InventoryDisplayType.Discard, Func<FishVals, bool> cb = null)
    {
        menu.SetActive(show);
        closeBtn.SetActive(showCloseBtn);

        if (display == InventoryDisplayType.Discard)
            cb = (FishVals val) => { return true; };

        if (currDisplayType != display || currCallback != cb)
        {
            currDisplayType = display;
            currCallback = cb;
            ReloadInventory();
        }
    }

    private void ReloadInventory()
    {
        currInv.ForEach(item => { Destroy(item); });
        currInv.Clear();

        foreach (FishVals v in InventoryController.Instance.inventory)
            AddInventoryEntry(v);
    }

    private void AddInventoryEntry(FishVals val)
    {
        RectTransform rt = Instantiate(inventoryEntryPrefab, scrollRectContent).GetComponent<RectTransform>();

        rt.GetChild(0).GetChild(0).GetComponent<Image>().sprite = val.image;
        rt.GetChild(0).GetChild(1).GetComponent<TMPro.TMP_Text>().text = Constants.GetFishName(val.fishType);
        rt.GetChild(0).GetChild(2).GetComponent<TMPro.TMP_Text>().text = val.size + "in";
        rt.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { OnInvButtonSelected(val); });
        rt.GetChild(1).GetChild(0).GetComponent<TMPro.TMP_Text>().text = GetInvDisplayBtnText(currDisplayType);

        currInv.Add(rt.gameObject);
        scrollRectContent.sizeDelta = new Vector2(0, 75 * currInv.Count);
        rt.anchoredPosition = new Vector3(0, -75 * (currInv.Count - 1), 0);
    }

    private void OnInvButtonSelected(FishVals val)
    {
        if (currCallback?.Invoke(val) == true)
            InventoryController.Instance.RemoveFish(val);
    }

    public static string GetInvDisplayBtnText(InventoryDisplayType displayType)
    {
        switch (displayType)
        {
            case InventoryDisplayType.Transfer:
                return "Give";
            case InventoryDisplayType.Discard:
                return "Drop";
            case InventoryDisplayType.Sell:
                return "Sell";
        }

        return "404";
    }
}
