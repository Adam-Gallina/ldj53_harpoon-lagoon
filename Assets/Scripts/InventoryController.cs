using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance;

    [HideInInspector] public List<FishVals> inventory = new List<FishVals>();

    [SerializeField] private int startCoins = 10;
    [HideInInspector] public int coins = 0;

    private void Awake()
    {
        Instance = this;
        coins = startCoins;
    }

    public bool AddFish(FishVals fish)
    {
        if (inventory.Count < BoatUpgrades.Instance.inventorySize.Val)
        {
            inventory.Add(fish);
            return true;
        }

        return false;
    }

    public void RemoveFish(FishVals fish)
    {
        inventory.Remove(fish);
    }

    public void ClearInventory()
    {
        inventory.Clear();
    }
}
