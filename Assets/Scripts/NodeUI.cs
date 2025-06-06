using UnityEngine;
using UnityEngine.UI;

public class NodeUI : MonoBehaviour
{
    public GameObject UI;

    public Text upgradeCost;
    public Text sellCost;
    public Button buttonUpg;

    private Node target;

    public void SetTarget(Node node)
    {
        target = node;

        transform.position = target.GetBuildPosition();

        sellCost.text = target.CurrentTurretConfig.SellCost.ToString() + "$";

        if (!target.isUpgraded)
        {
            upgradeCost.text = target.CurrentTurretConfig.UpgradeCost.ToString() + "$";
            buttonUpg.interactable = true;
        }
        else
        {
            upgradeCost.text = "MAX";
            buttonUpg.interactable = false;
        }
        

        UI.SetActive(true);
    }

    public void Hide()
    {
        UI.SetActive(false);
    }

    public void Upgrade()
    {
        target.UpgradeTurret();
        BuildManager.Instance.DeselectNode();
    }

    public void Sell()
    {
        target.SellTurret();
        BuildManager.Instance.DeselectNode();
    }
}
