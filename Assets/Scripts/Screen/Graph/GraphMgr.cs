using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GraphMgr : MonoBehaviour
{
    public GraphDrawer velocityDrawer;
    public GraphDrawer momentumDrawer;
    public GraphDrawer kineticEnergyDrawer;

    public BarMgr velocityBar;
    public BarMgr momentumBar;
    public BarMgr kineticEnergyBar;

    private void Awake() {
        velocityDrawer      = DataSetting.GetComponentFromChild<GraphDrawer>(transform, "VelocityGraph");
        momentumDrawer      = DataSetting.GetComponentFromChild<GraphDrawer>(transform, "MomentumGraph");
        kineticEnergyDrawer = DataSetting.GetComponentFromChild<GraphDrawer>(transform, "KineticEnergyGraph");

        kineticEnergyBar = DataSetting.GetComponentFromChild<BarMgr>(transform, "ValueBar/KineticEnergyBar");
    }

    public void AddTime(float time) {
        BlockMove         blockA = DataSetting.Instance.blockA;
        BlockMove         blockB = DataSetting.Instance.blockB;
        BlockSpringCouple couple = DataSetting.Instance.couple;

        velocityDrawer.AddValue(time, blockA.CalculateVelocity(time), 0);
        velocityDrawer.AddValue(time, blockB.CalculateVelocity(time), 1);

        float momentumA = blockA.CalculateMomentum(time);
        float momentumB = blockB.CalculateMomentum(time);
        momentumDrawer.AddValue(time, momentumA, 0);
        momentumDrawer.AddValue(time, momentumB, 1);
        momentumDrawer.AddValue(time, momentumA + momentumB, 2);

        float kineticEnergyA = blockA.CalculateKineticEnergy(time);
        float kineticEnergyB = blockB.CalculateKineticEnergy(time);
        float deltaL         = (blockA.CalculatePos(time) - blockB.CalculatePos(time)) / couple.moveRatio;
        float springEnergy   = couple.k * deltaL * deltaL / 2;
        kineticEnergyDrawer.AddValue(time, kineticEnergyA, 0);
        kineticEnergyDrawer.AddValue(time, kineticEnergyB, 1);
        kineticEnergyDrawer.AddValue(time, springEnergy, 2);
        // kineticEnergyDrawer.AddValue(time, kineticEnergyA + kineticEnergyB + springEnergy, 3);
    }

    public void SetHighlightPoint(float time) {
        BlockMove         blockA = DataSetting.Instance.blockA;
        BlockMove         blockB = DataSetting.Instance.blockB;
        BlockSpringCouple couple = DataSetting.Instance.couple;

        velocityDrawer.SetHighlightPoint(time, blockA.CalculateVelocity(time), 0);
        velocityDrawer.SetHighlightPoint(time, blockB.CalculateVelocity(time), 1);

        float momentumA = blockA.CalculateMomentum(time);
        float momentumB = blockB.CalculateMomentum(time);
        momentumDrawer.SetHighlightPoint(time, momentumA, 0);
        momentumDrawer.SetHighlightPoint(time, momentumB, 1);
        momentumDrawer.SetHighlightPoint(time, momentumA + momentumB, 2);

        float kineticEnergyA = blockA.CalculateKineticEnergy(time);
        float kineticEnergyB = blockB.CalculateKineticEnergy(time);
        float deltaL         = (blockA.CalculatePos(time) - blockB.CalculatePos(time)) / couple.moveRatio;
        float springEnergy   = couple.k * deltaL * deltaL / 2;
        kineticEnergyDrawer.SetHighlightPoint(time, kineticEnergyA, 0);
        kineticEnergyDrawer.SetHighlightPoint(time, kineticEnergyB, 1);
        kineticEnergyDrawer.SetHighlightPoint(time, springEnergy, 2);
        // kineticEnergyDrawer.SetHighlightPoint(time, kineticEnergyA + kineticEnergyB + springEnergy, 3);
    }

    public void DrawGraph() {
        velocityDrawer.DrawGraph();
        momentumDrawer.DrawGraph();
        kineticEnergyDrawer.DrawGraph();
    }

    public void ShowHighlightPoints() {
        velocityDrawer.ShowHighlightPoints();
        momentumDrawer.ShowHighlightPoints();
        kineticEnergyDrawer.ShowHighlightPoints();
    }

    public void HideHighlightPoints() {
        velocityDrawer.HideHighlightPoints();
        momentumDrawer.HideHighlightPoints();
        kineticEnergyDrawer.HideHighlightPoints();
    }

    public void ShowGraph(GraphType type) {
        var drawers = new[] { velocityDrawer, momentumDrawer, kineticEnergyDrawer };
        var bars    = new[] { velocityBar, momentumBar, kineticEnergyBar };
        for (int i = 0; i < drawers.Length; i++) {
            if (i == (int) type) {
                drawers[i].Show();
            }
            else {
                drawers[i].Hide();
                bars[i]?.Hide();
            }
        }
    }

    public void ShowBar() {
        Main main = DataSetting.Instance.main;
        if (main.statusType is StateType.Prepared or StateType.Pulling) 
            return;
        var  bars = new[] { velocityBar, momentumBar, kineticEnergyBar };
        for (int i = 1; i < bars.Length; i++) {
            if (i == (int) DataSetting.Instance.panel.panelType) {
                bars[i]?.Show();
                Debug.Log("show bar");
            }
            else {
                bars[i]?.Hide();
            }
        }
    }

    public void HideBar() {
        var bars = new[] { velocityBar, momentumBar, kineticEnergyBar };
        for (int i = 1; i < bars.Length; i++) {
            if (i == (int) DataSetting.Instance.panel.panelType) {
                bars[i]?.Hide();
            }
        }
    }

    public void Reset() {
        velocityDrawer.Reset();
        momentumDrawer.Reset();
        kineticEnergyDrawer.Reset();
    }

    public void Freeze() {
        velocityDrawer.Freeze();
        momentumDrawer.Freeze();
        kineticEnergyDrawer.Freeze();
    }
}

public enum GraphType
{
    Velocity,
    Momentum,
    KineticEnergy
}