using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour
{
    public Color hoverColor;
    public Color noMoneyColor;
    private Color defaultColor;
    private Renderer rend;
    public Vector3 PosOffset;

    [HideInInspector]
    public GameObject turret;
    [HideInInspector]
    public TurretBlueprint turretBlueprint;
    [HideInInspector]
    public bool isUpgraded = false;
    

    private void Start()
    {       
        rend = GetComponent<Renderer>();
        defaultColor = rend.material.color;
    }

    public Vector3 GetBuildPosition()
    {
        return transform.position + PosOffset;
    }

    private void OnMouseEnter()
    {
        if(EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (!BuildManager.Instance.canBuilt)
        {
            return;
        }
        if (!BuildManager.Instance.hasMoney)
        {
            rend.material.color = Color.red;
        }
        else
        {
            rend.material.color = hoverColor;
        }
    }

    private void OnMouseExit()
    {
        rend.material.color = defaultColor;
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }       
        if(turret != null)
        {
            BuildManager.Instance.SelectNode(this);
            return;
        }
        if (!BuildManager.Instance.canBuilt)
        {
            return;
        }
        BuildTurret(BuildManager.Instance.GetTurretToBuild());
    }

    public void BuildTurret(TurretBlueprint blueprint)
    {
        if (PlayerStats.Money < blueprint.cost)
        {
            Debug.Log("Not enough money!");
            return;
        }
        PlayerStats.Stats.Spend(blueprint.cost);

        GameObject effect = Instantiate(BuildManager.Instance.buildEffect, GetBuildPosition(), Quaternion.identity);
        GameObject _turret = Instantiate(blueprint.prefab, GetBuildPosition(), Quaternion.identity);
        turret = _turret;

        turretBlueprint = blueprint;
        
        Destroy(effect.gameObject, 5f);
    }

    public void UpgradeTurret()
    {
        if (PlayerStats.Money < turretBlueprint.upgradeCost)
        {
            Debug.Log("Not enough money!");
            return;
        }
        PlayerStats.Stats.Spend(turretBlueprint.upgradeCost);

        GameObject effect = Instantiate(BuildManager.Instance.buildEffect, GetBuildPosition(), Quaternion.identity);

        Destroy(turret);

        GameObject _turret = Instantiate(turretBlueprint.upgradedPrefab, GetBuildPosition(), Quaternion.identity);
        turret = _turret;

        isUpgraded = true;

        Destroy(effect.gameObject, 5f);
    }

    public void SellTurret()
    {
        PlayerStats.Stats.Add(turretBlueprint.getSellCost);

        GameObject effect = Instantiate(BuildManager.Instance.sellEffect, GetBuildPosition(), Quaternion.identity);

        Destroy(effect.gameObject, 5f);
        Destroy(turret);
        turretBlueprint = null;
    }
}
