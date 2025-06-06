using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;
    public Text MoneyCounter;
    private BaseTurretConfig turretToBuild;
    private Node selectedNode;
    public NodeUI nodeUI;

    public int NumOfTurrets { get; private set; } = 0;
    public bool canBuilt => turretToBuild != null;
    public bool hasMoney => PlayerStats.Money >= turretToBuild.Cost;

    private List<BaseTurretConfig> activeTurretConfigs = new();

    public GameObject buildEffect;
    public GameObject sellEffect;

    private void Awake()
    {
        Instance = this;
        MoneyCounter.text = PlayerStats.Money +"$";
    }
    public void SelectTurretToBuild(BaseTurretConfig turret)
    {
        turretToBuild = turret;

        DeselectNode();
    }

    public BaseTurretConfig GetTurretToBuild()
    {
        NumOfTurrets++;
        RegisterTurretBuilt(turretToBuild);

        return turretToBuild;
    }

    public void SelectNode(Node node)
    {
        if(selectedNode == node)
        {
            DeselectNode();
            return;
        }
        
        selectedNode = node;
        turretToBuild = null;

        nodeUI.SetTarget(node);
    }

    public void DeselectNode()
    {
        selectedNode = null;
        nodeUI.Hide();
    }
    
    public void RegisterTurretBuilt(BaseTurretConfig config)
    {
        if (config != null)
        {
            activeTurretConfigs.Add(config);
        }
    }
    
    public void UnregisterTurret(BaseTurretConfig config)
    {
        if (config != null)
        {
            activeTurretConfigs.Remove(config);
        }
    }

    public List<BaseTurretConfig> GetActiveTurretConfigs()
    {
        return new List<BaseTurretConfig>(activeTurretConfigs);
    }
}
