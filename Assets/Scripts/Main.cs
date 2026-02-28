using System;
using System.Collections;
using System.Collections.Generic;
using HighlightPlus;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public partial class Main : MonoBehaviour
{
    public InteractionType interactionType = InteractionType.VirtualHands; // 交互类型

    public StateType statusType = StateType.Prepared; // 当前 couple 状态

    public float maxSpeed = 1; // 初速度

    public BlockSpringCouple.EBlock block = BlockSpringCouple.EBlock.A; // 给哪个物块初速度

    public float moveTime = 0; // 移动时间

    public Vector3 baffleTargetPos; // baffle 目标位置

    public float xAxisUnit = 0.01f;

    public TextMeshPro textPullVelocity;

    private Button _button;
    private Slider _slider;

    private float _lastSliderValue; // 用于记录 sliderValue 变化量

    public Grabbable cubeGrab;
    public Grabbable knobGrab;

    public HandGrabInteractable cubeGrabInteractable;
    public PokeInteractable     buttonPressInteractable;
    public HandGrabInteractable knobGrabInteractable;

    public InteractableUnityEventWrapper cubeGrabEvent;
    public InteractableUnityEventWrapper buttonPressEvent;
    public InteractableUnityEventWrapper knobGrabEvent;

    public KnobRotate knobRotate;
    public KnobAngle  knobAngle;

    public HighlightEffect blockBEffect;
    public HighlightEffect knobEffect;
    public HighlightEffect buttonEffect;
    public HighlightEffect menuEffect;

    public Transform rightHandAnchorDetached;

    public Vector3 pullHandStartPos;
    public Vector3 pullHandLastPos; // 手上一帧的位置 x
    public Vector3 pullHandNowPos;  // 手该帧的位置 x

    public OneGrabTranslateTransformer blockBTranslateTransformer;
    public OneGrabRotateTransformer    knobRotateTransformer;

    [Range(0, 2)]
    public float angleRatio = 0.2f; // 旋钮旋转对应滑块移动的距离

    private PreparedState    _preparedState;
    private PullingState     _pullingState;
    private RunningState     _runningState;
    private ControllingState _controllingState;
    private FinishedState    _finishedState;

    private void Awake() {
        // _slider          = DataSetting.GetComponent<Slider>("Canvas/Slider");
        // _button          = DataSetting.GetComponent<Button>("Canvas/Button");
        textPullVelocity = DataSetting.GetComponent<TextMeshPro>("Objects/LabTable/Track/Couple/BlockB/TextPullVelocity");
        blockBEffect     = DataSetting.GetComponent<HighlightEffect>("Objects/LabTable/Track/Couple/BlockB/Model/Block");
        knobEffect       = DataSetting.GetComponent<HighlightEffect>("Objects/LabTable/Base/KnobObj/Up");
        buttonEffect     = DataSetting.GetComponent<HighlightEffect>("Objects/LabTable/Base/ButtonObj/Button/Visuals/ButtonVisual");
        menuEffect       = DataSetting.GetComponent<HighlightEffect>("ButtonMenu");

        cubeGrab             = DataSetting.GetComponent<Grabbable>("Objects/LabTable/Track/Couple/BlockB");
        cubeGrabInteractable = DataSetting.GetComponent<HandGrabInteractable>("Objects/LabTable/Track/Couple/BlockB/HandGrabInteractable");
        cubeGrabEvent        = DataSetting.GetComponent<InteractableUnityEventWrapper>("Objects/LabTable/Track/Couple/BlockB/HandGrabInteractable");

        buttonPressInteractable = DataSetting.GetComponent<PokeInteractable>("Objects/LabTable/Base/ButtonObj/Button");
        buttonPressEvent        = DataSetting.GetComponent<InteractableUnityEventWrapper>("Objects/LabTable/Base/ButtonObj/Button");

        knobGrab             = DataSetting.GetComponent<Grabbable>("Objects/LabTable/Base/KnobObj/Up");
        knobGrabInteractable = DataSetting.GetComponent<HandGrabInteractable>("Objects/LabTable/Base/KnobObj/Up/HandGrabInteractable");
        knobGrabEvent        = DataSetting.GetComponent<InteractableUnityEventWrapper>("Objects/LabTable/Base/KnobObj/Up/HandGrabInteractable");
        knobAngle            = DataSetting.GetComponent<KnobAngle>("Objects/LabTable/Base/KnobObj");
        knobRotate           = DataSetting.GetComponent<KnobRotate>("Objects/LabTable/Base/KnobObj");

        rightHandAnchorDetached = DataSetting.GetComponent<Transform>("OVRCameraRig/TrackingSpace/RightHandAnchorDetached");

        blockBTranslateTransformer = DataSetting.GetComponent<OneGrabTranslateTransformer>("Objects/LabTable/Track/Couple/BlockB");
        knobRotateTransformer      = DataSetting.GetComponent<OneGrabRotateTransformer>("Objects/LabTable/Base/KnobObj/Up");
    }

    // Start is called before the first frame update
    void Start() {
        _preparedState    = new PreparedState(this);
        _pullingState     = new PullingState(this);
        _runningState     = new RunningState(this);
        _controllingState = new ControllingState(this);
        _finishedState    = new FinishedState(this);

        // 注册事件
        EventMgr.Instance.AddListener(nameof(MainEventType.EnterPreparedStatus), _preparedState.Enter, this, GetType());
        EventMgr.Instance.AddListener(nameof(MainEventType.EnterPullingStatus), _pullingState.Enter, this, GetType());
        EventMgr.Instance.AddListener(nameof(MainEventType.EnterRunningStatus), _runningState.Enter, this, GetType());
        EventMgr.Instance.AddListener(nameof(MainEventType.EnterControllingStatus), _controllingState.Enter, this, GetType());
        EventMgr.Instance.AddListener(nameof(MainEventType.EnterFinishedStatus), _finishedState.Enter, this, GetType());

        // _button.onClick.AddListener(OnButtonPress);
        // _slider.onValueChanged.AddListener(SliderValueChanged);
        PressInput.Instance.onReleased.AddListener(OnButtonPress);
        PullInput.Instance.onReleased.AddListener(OnSpringPullExist);
        baffleTargetPos  = DataSetting.Instance.baffleMove.transform.localPosition;
        pullHandStartPos = DataSetting.Instance.blockB.transform.position;

        InitInteractionOnStart(interactionType);

        _preparedState.Enter();
    }

    public void InitInteractionOnStart(InteractionType type) {
        bool isVirtualHands = interactionType == InteractionType.VirtualHands;

        Debug.Log($"init interaction on start: {isVirtualHands}");

        cubeGrabInteractable.enabled = isVirtualHands;
        cubeGrab.enabled             = isVirtualHands;
        cubeGrabEvent.enabled        = isVirtualHands;

        buttonPressInteractable.enabled = isVirtualHands;
        buttonPressEvent.enabled        = isVirtualHands;

        // knobGrabInteractable.enabled = isVirtualHands;
        // knobGrab.enabled = isVirtualHands;

        // knobGrabEvent.enabled = isVirtualHands;
        knobRotate.enabled = !isVirtualHands;

        switch (type) {
            case InteractionType.VirtualHands:
                
                knobGrabEvent.WhenSelect.AddListener(OnKnobRotate);
                angleRatio = 1f;
                break;
            case InteractionType.RealObjects:
                Destroy(knobGrab);
                // knobGrab.enabled = false;
                // knobRotateTransformer.Constraints.MaxAngle.Constrain = true;
                // knobRotateTransformer.Constraints.MinAngle.Constrain = true;
                angleRatio = 0.2f;
                break;
            default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    // Update is called once per frame
    void Update() {
        switch (statusType) {
            case StateType.Prepared:
                _preparedState.Update();
                break;
            case StateType.Pulling:
                _pullingState.Update();
                break;
            case StateType.Running:
                _runningState.Update();
                break;
            case StateType.Controlling:
                _controllingState.Update();
                break;
            case StateType.Paused: // 暂未实现
                break;
            case StateType.Finished:
                _finishedState.Update();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            OnButtonPress();
        }
    }

    public void ExistCurrentStatus() {
        switch (statusType) {
            case StateType.Prepared:
                _preparedState.Exist();
                break;
            case StateType.Pulling:
                _pullingState.Exist();
                break;
            case StateType.Running:
                _runningState.Exist();
                break;
            case StateType.Controlling:
                _controllingState.Exist();
                break;
            case StateType.Paused: // 暂未实现
                break;
            case StateType.Finished:
                _finishedState.Exist();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// 按钮按下的触发事件
    /// </summary>
    public void OnButtonPress() {
        if (statusType == StateType.Controlling) {
            // Controlling --> Running
            EventMgr.Instance.EventTrigger(nameof(MainEventType.EnterRunningStatus));
        }
        else if (statusType is StateType.Finished or StateType.Running) {
            // Finished --> Prepared
            // Running  --> Prepared
            EventMgr.Instance.EventTrigger(nameof(MainEventType.EnterPreparedStatus));
        }
    }

    /// <summary>
    /// 旋钮旋转的触发事件
    /// </summary>
    public void OnKnobRotate() {
        if (statusType == StateType.Running) {
            // Running --> Controlling
            EventMgr.Instance.EventTrigger(nameof(MainEventType.EnterControllingStatus));
        }
    }

    /// <summary>
    /// 物块拉动时的触发事件
    /// </summary>
    public void OnSpringPullEnter() {
        textPullVelocity.enabled = true;

        // Running --> Controlling
        EventMgr.Instance.EventTrigger(nameof(MainEventType.EnterPullingStatus));
    }

    /// <summary>
    /// 物块被松开时的触发事件
    /// </summary>
    public void OnSpringPullExist() {
        if (statusType == StateType.Pulling) {
            float moveDis = pullHandNowPos.x - pullHandStartPos.x;

            float speed = interactionType switch {
                InteractionType.VirtualHands => moveDis / blockBTranslateTransformer.Constraints.MaxZ.Value * maxSpeed,
                InteractionType.RealObjects  => PullInput.Instance.NormedMaxValue * maxSpeed,
                _                            => throw new ArgumentOutOfRangeException()
            };

            // couple：给 block 设置初速度
            DataSetting.Instance.couple.SetPreSpeed(block, speed);

            // panel：更新面板信息
            DataSetting.Instance.panel.SetStartSpeed(speed);

            PullInput.Instance.IsPulling = false;

            // Prepared --> Running
            EventMgr.Instance.EventTrigger(nameof(MainEventType.EnterRunningStatus));
        }
    }

    public void SliderValueChanged(float value) {
        if (statusType == StateType.Running) {
            // Running --> Controlling
            EventMgr.Instance.EventTrigger(nameof(MainEventType.EnterControllingStatus));
        }

        if (statusType == StateType.Controlling) {
            // 处于用户控制状态时，才执行以下逻辑
            float deltaValue = value - _lastSliderValue; // 记录 value 变化量
            _lastSliderValue = value;

            if (deltaValue == 0) return; // 没有变化，不做处理（写不写这句都行，因为放在了 SliderValueChanged 函数中）

            // 两帧之间的时间变化
            float lastTime = moveTime;
            float nowTime  = moveTime + deltaValue;
            moveTime = nowTime <= 0 ? 0 : nowTime;

            DataSetting.Instance.couple.SetMoveTime(moveTime); // 确保 nowTime >= 0

            // DataSetting.Instance.velocityGraph.SetHighlightPoint(moveTime,
            //                                                      DataSetting.Instance.blockA.CalculateVelocity(moveTime), 0);
            // DataSetting.Instance.velocityGraph.SetHighlightPoint(moveTime,
            //                                                      DataSetting.Instance.blockB.CalculateVelocity(moveTime), 1);

            DataSetting.Instance.graphMgr.SetHighlightPoint(moveTime);

            if (lastTime >= 0 || nowTime >= 0) {
                // 确保 lastTime - nowTime 中间有 >=0 的点需要绘制
                // graph：进行绘制
                float startTime = (int) (lastTime / xAxisUnit) * xAxisUnit;
                float endTime   = (int) (nowTime / xAxisUnit) * xAxisUnit;
                float deltaTime = Mathf.Sign(nowTime - lastTime) * xAxisUnit;
                for (float t = startTime;
                     t >= 0 && ((deltaValue > 0 && t < endTime) || (deltaValue < 0 && t > endTime));
                     t += deltaTime) {
                    // 线性采样，绘制图像
                    // DataSetting.Instance.velocityGraph.AddValue(t, DataSetting.Instance.blockA.CalculateVelocity(t), 0);
                    // DataSetting.Instance.velocityGraph.AddValue(t, DataSetting.Instance.blockB.CalculateVelocity(t), 1);

                    DataSetting.Instance.graphMgr.AddTime(t);
                }

                DataSetting.Instance.graphMgr.DrawGraph();
            }
        }
    }
}

/// <summary>
/// 所有事件
/// </summary>
public enum MainEventType
{
    EnterPreparedStatus,
    EnterPullingStatus,
    EnterRunningStatus,
    EnterControllingStatus,
    EnterFinishedStatus,
}

public enum InteractionType
{
    VirtualHands,
    RealObjects,
}