using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public TurretBlueprint DefaultTurret;
    public TurretBlueprint MissileTurret;
    public TurretBlueprint LaserTurret;
    public void SelectDefaultTurret()
    {
        Debug.Log("Def Turret selected");
        BuildManager.Instance.SelectTurretToBuild(DefaultTurret);
    }

    public void SelectMissileLauncher()
    {
        Debug.Log("Missile Turret selected");
        BuildManager.Instance.SelectTurretToBuild(MissileTurret);
    }

    public void SelectLaserTurret()
    {
        Debug.Log("Laser turret selected");
        BuildManager.Instance.SelectTurretToBuild(LaserTurret);
    }

}
