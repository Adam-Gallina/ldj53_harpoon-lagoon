using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private Button speedModBtn;
    [SerializeField] private TMPro.TMP_Text speedModText;
    [SerializeField] private Button spearCountBtn;
    [SerializeField] private TMPro.TMP_Text spearCountText;
    [SerializeField] private Button spearSpeedBtn;
    [SerializeField] private TMPro.TMP_Text spearSpeedText;
    [SerializeField] private Button ropeStrengthBtn;
    [SerializeField] private TMPro.TMP_Text ropeStrengthText;
    [SerializeField] private Button pullSpeedBtn;
    [SerializeField] private TMPro.TMP_Text pullSpeedText;

    private void Update()
    {
        speedModBtn.interactable = BoatUpgrades.Instance.boatSpeedMod.CanUpgrade();
        speedModText.text = "Boat Speed: " + (BoatUpgrades.Instance.boatSpeedMod.level == 3 ? "MAX" : BoatUpgrades.Instance.boatSpeedMod.Price);
        
        spearCountBtn.interactable = BoatUpgrades.Instance.spearCount.CanUpgrade();
        spearCountText.text = "Harpoon Count: " + (BoatUpgrades.Instance.spearCount.level == 3 ? "MAX" : BoatUpgrades.Instance.spearCount.Price);

        spearSpeedBtn.interactable = BoatUpgrades.Instance.spearSpeed.CanUpgrade();
        spearSpeedText.text = "Harpoon Speed: " + (BoatUpgrades.Instance.spearSpeed.level == 3 ? "MAX" : BoatUpgrades.Instance.spearSpeed.Price);

        ropeStrengthBtn.interactable = BoatUpgrades.Instance.ropeStrength.CanUpgrade();
        ropeStrengthText.text = "Rope Strength: " + (BoatUpgrades.Instance.ropeStrength.level == 3 ? "MAX" : BoatUpgrades.Instance.ropeStrength.Price);

        pullSpeedBtn.interactable = BoatUpgrades.Instance.pullSpeed.CanUpgrade();
        pullSpeedText.text = "Pull Speed: " + (BoatUpgrades.Instance.pullSpeed.level == 3 ? "MAX" : BoatUpgrades.Instance.pullSpeed.Price);
    }

    public void SpeedModBtn()
    {
        if (BoatUpgrades.Instance.boatSpeedMod.CanUpgrade())
            BoatUpgrades.Instance.boatSpeedMod.DoUpgrade();
    }

    public void SpearCountBtn()
    {
        if (BoatUpgrades.Instance.spearCount.CanUpgrade())
            BoatUpgrades.Instance.spearCount.DoUpgrade();
    }

    public void SpearSpeedBtn()
    {
        if (BoatUpgrades.Instance.spearSpeed.CanUpgrade())
            BoatUpgrades.Instance.spearSpeed.DoUpgrade();
    }

    public void RopeStrengthBtn()
    {
        if (BoatUpgrades.Instance.ropeStrength.CanUpgrade())
            BoatUpgrades.Instance.ropeStrength.DoUpgrade();
    }

    public void PullSpeedBtn()
    {
        if (BoatUpgrades.Instance.pullSpeed.CanUpgrade())
            BoatUpgrades.Instance.pullSpeed.DoUpgrade();
    }
}
