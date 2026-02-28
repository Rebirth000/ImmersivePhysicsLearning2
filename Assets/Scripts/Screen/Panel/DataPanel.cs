using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteAlways]
public class DataPanel : MonoBehaviour
{
    public VelocityPanel      velocityPanel;
    public MomentumPanel      momentumPanel;
    public KineticEnergyPanel kineticEnergyPanel;
    public ConstantPanel      constantPanel;

    public GraphMgr graphMgr;

    private PanelType _lastPanelType;
    public  PanelType panelType;

    private void Awake() {
        velocityPanel      = DataSetting.GetComponentFromChild<VelocityPanel>(transform, "VelocityPanel");
        momentumPanel      = DataSetting.GetComponentFromChild<MomentumPanel>(transform, "MomentumPanel");
        kineticEnergyPanel = DataSetting.GetComponentFromChild<KineticEnergyPanel>(transform, "KineticEnergyPanel");
        constantPanel      = DataSetting.GetComponentFromChild<ConstantPanel>(transform, "ConstantPanel");

        graphMgr = DataSetting.GetComponentFromChild<GraphMgr>(transform.parent, "GraphDrawers");

        // 避免 panel 被失活而无法调用 Awake()
        velocityPanel.Awake();
        momentumPanel.Awake();
        kineticEnergyPanel.Awake();
    }

    private void Update() {
        if (panelType != _lastPanelType) {
            switch (panelType) {
                case PanelType.Velocity:
                    ShowVelocityPanel();
                    break;
                case PanelType.Momentum:
                    ShowMomentumPanel();
                    break;
                case PanelType.KineticEnergy:
                    ShowKineticEnergyPanel();
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            _lastPanelType = panelType;
        }
    }

    public void SetStartSpeed(float speed) {
        constantPanel.SetStartSpeed(speed);
        momentumPanel.SetStartMomentum(speed);
    }

    public void ShowPanel(PanelType type) {
        velocityPanel.gameObject.SetActive(type == PanelType.Velocity);
        momentumPanel.gameObject.SetActive(type == PanelType.Momentum);
        kineticEnergyPanel.gameObject.SetActive(type == PanelType.KineticEnergy);
    }

    public void ShowVelocityPanel() {
        graphMgr.ShowGraph(GraphType.Velocity);
        ShowPanel(PanelType.Velocity);
        panelType = PanelType.Velocity;
    }

    public void ShowMomentumPanel() {
        graphMgr.ShowGraph(GraphType.Momentum);
        ShowPanel(PanelType.Momentum);
        panelType = PanelType.Momentum;
    }

    public void ShowKineticEnergyPanel() {
        graphMgr.ShowGraph(GraphType.KineticEnergy);
        ShowPanel(PanelType.KineticEnergy);
        panelType = PanelType.KineticEnergy;
    }
}

public enum PanelType
{
    Velocity,
    Momentum,
    KineticEnergy
}