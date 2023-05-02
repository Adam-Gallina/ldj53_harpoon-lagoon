using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    public List<Sprite> desiredFishImages;
    [SerializeField] private List<FishType> desiredFishTypes;
    public Dictionary<FishType, bool> recievedFish = new Dictionary<FishType, bool>();

    bool invLast = false;

    private void Awake()
    {
        foreach (FishType f in desiredFishTypes)
            recievedFish.Add(f, false);
    }

    public void OnInteract()
    {
        GameUI.Instance.SetMerchant(true, this);
        BoatController.Instance.SetBoatMode(BoatMode.Frozen);
        GameUI.Instance.SetEIndicator(null);
    }

    public bool SellFish(FishVals fish)
    {
        InventoryController.Instance.coins += (int)(fish.size * 10 * fish.valMod);
        return true;
    }

    public void SellAllFish()
    {
        foreach (FishVals f in InventoryController.Instance.inventory)
            SellFish(f);
    }

    public bool TryFillOrder(FishVals fish)
    {
        // Fish is not a correct type
        if (!recievedFish.ContainsKey(fish.fishType))
            return false;

        // Fish has already been provided
        if (recievedFish[fish.fishType])
            return false;

        recievedFish[fish.fishType] = true;

        if (invLast)
            BoatUpgrades.Instance.spearDamage.DoUpgrade();
        else
            BoatUpgrades.Instance.inventorySize.DoUpgrade();
        invLast = !invLast;

        return true;
    }

    public int TotalPrice()
    {
        int total = 0;
        foreach (FishVals f in InventoryController.Instance.inventory)
            total += (int)(f.size * 10 * f.valMod);
        return total;
    }
}
