using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyDebug : MonoBehaviour
{
    public FishVals testFish;
    public FishVals testFish2;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            InventoryController.Instance.AddFish(testFish);
            testFish.size += 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            InventoryController.Instance.AddFish(testFish2);
            testFish2.size += 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            InventoryController.Instance.coins += 100;
        }
    }
}
