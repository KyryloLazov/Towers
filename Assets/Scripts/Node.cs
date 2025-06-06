using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class Node : MonoBehaviour
{
    [SerializeField] private bool _isActive;
    
    [SerializeField] private Color _hoverColor;
    [SerializeField] private Color _noMoneyColor;
    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _inActiveColor;
    private Color _defaultColor;
    private Renderer rend;
    public Vector3 PosOffset;
    
    private GameObject _currentTurret;
    [FormerlySerializedAs("turretBlueprint")] [HideInInspector]
    public BaseTurretConfig CurrentTurretConfig;
    [HideInInspector]
    public bool isUpgraded = false;
    

    private void Start()
    {
        rend = GetComponent<Renderer>();

        _defaultColor = _isActive ? _activeColor : _inActiveColor;
        
        rend.material.color = _defaultColor;
    }

    public Vector3 GetBuildPosition()
    {
        return transform.position + PosOffset;
    }

    private void OnMouseEnter()
    {
        if (!_isActive) return;
        
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
            rend.material.color = _noMoneyColor;
        }
        else
        {
            rend.material.color = _hoverColor;
        }
    }

    private void OnMouseExit()
    {
        rend.material.color = _defaultColor;
    }

    private void OnMouseDown()
    {
        if (!_isActive) return;
        
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }       
        if(_currentTurret != null)
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

    public void BuildTurret(BaseTurretConfig blueprint)
    {
        if (!_isActive) return;
        
        if (PlayerStats.Money < blueprint.Cost)
        {
            Debug.Log("Not enough money!");
            return;
        }
        PlayerStats.Stats.Spend(blueprint.Cost);

        GameObject effect = Instantiate(BuildManager.Instance.buildEffect, GetBuildPosition(), Quaternion.identity);
        GameObject turret = Instantiate(blueprint.TurretPrefab, GetBuildPosition(), Quaternion.identity);
        _currentTurret = turret;

        CurrentTurretConfig = blueprint;
        
        Destroy(effect.gameObject, 5f);
    }

    public void UpgradeTurret()
    {
        if (PlayerStats.Money < CurrentTurretConfig.UpgradeCost)
        {
            Debug.Log("Not enough money!");
            return;
        }

        if (!CurrentTurretConfig.IsUpgradable) return;
        
        PlayerStats.Stats.Spend(CurrentTurretConfig.UpgradeCost);

        GameObject effect = Instantiate(BuildManager.Instance.buildEffect, GetBuildPosition(), Quaternion.identity);
        
        BuildManager.Instance.UnregisterTurret(CurrentTurretConfig);
        Destroy(_currentTurret);

        GameObject turret = Instantiate(CurrentTurretConfig.UpgradedPrefab, GetBuildPosition(), Quaternion.identity);
        _currentTurret = turret;
        
        CurrentTurretConfig = _currentTurret.GetComponent<BaseTurret>().Config;
        BuildManager.Instance.RegisterTurretBuilt(CurrentTurretConfig);
        
        isUpgraded = true;

        Destroy(effect.gameObject, 5f);
    }

    public void SellTurret()
    {
        PlayerStats.Stats.Add(CurrentTurretConfig.SellCost);

        GameObject effect = Instantiate(BuildManager.Instance.sellEffect, GetBuildPosition(), Quaternion.identity);

        Destroy(effect.gameObject, 5f);
        Destroy(_currentTurret);
        BuildManager.Instance.UnregisterTurret(CurrentTurretConfig);
        CurrentTurretConfig = null;
    }
}
