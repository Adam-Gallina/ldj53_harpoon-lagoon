using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatUpgrades : MonoBehaviour
{
    public static BoatUpgrades Instance;

    private void Awake()
    {
        Instance = this;
    }

    [Header("Player Unlocks")]
    public Upgrade boatSpeedMod;
    public Upgrade spearCount;
    public Upgrade spearSpeed;
    public Upgrade ropeStrength;
    public Upgrade pullSpeed;

    [Header("NPC Unlocks")]
    public Upgrade spearDamage;
    public Upgrade inventorySize;
}

[System.Serializable]
public class Upgrade
{
    public float Val { get { return val[level]; } }
    public int Price { get { return price[level]; } }

    public float[] val;
    public int[] price;
    [HideInInspector] public int level = 0;

    public bool CanUpgrade()
    {
        if (level >= price.Length)
            return false;

        return InventoryController.Instance.coins >= price[level];
    }

    public void DoUpgrade()
    {
        if (level < val.Length - 1)
        {
            InventoryController.Instance.coins -= price[level];
            level++;
        }
    }
}