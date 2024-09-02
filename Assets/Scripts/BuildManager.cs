using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;
    public Text MoneyCounter;
    private TurretBlueprint turretToBuild;
    private Node selectedNode;
    public NodeUI nodeUI;

    public static int NumOfTurrets = 0;

    public static int NumOfDefTurrets = 0;
    public static int NumOfRockTurrets = 0;
    public static int NumOfLasTurrets = 0;
    public bool canBuilt { get { return turretToBuild != null; } }
    public bool hasMoney { get { return PlayerStats.Money >= turretToBuild.cost; } }

    public GameObject buildEffect;
    public GameObject sellEffect;

    private void Awake()
    {
        Instance = this;
        MoneyCounter.text = PlayerStats.Money.ToString() +"$";

        NumOfTurrets = 0;
        NumOfDefTurrets = 0;
        NumOfRockTurrets = 0;
        NumOfLasTurrets = 0;
}
    public void SelectTurretToBuild(TurretBlueprint turret)
    {
        turretToBuild = turret;

        DeselectNode();
    }

    public TurretBlueprint GetTurretToBuild()
    {
        NumOfTurrets++;

        if(turretToBuild.Type == "Default")
        {
            NumOfDefTurrets++;
        }

        if (turretToBuild.Type == "Missle")
        {
            NumOfRockTurrets++;
        }

        if (turretToBuild.Type == "Laser")
        {
            NumOfLasTurrets++;
        }

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
}
