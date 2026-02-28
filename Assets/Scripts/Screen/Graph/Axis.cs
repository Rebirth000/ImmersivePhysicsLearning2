using Shapes;
using TMPro;
using UnityEngine;


public abstract class Axis : ImmediateModeShapeDrawer
{
    public Rectangle rectangle; // 画坐标轴的矩形
    public Triangle  triangle;  // 画坐标轴箭头的三角形

    public float maxIndex = 0; // 坐标轴上界值
    public float minIndex = 0; // 坐标轴下界值

    public bool         isGridShown     = true;
    public Color        gridLineColor   = new Color(0.5f, 0.5f, 0.5f, 0.8f);
    [Range(0, 0.1f)]
    public float        gridLineWidth  = 0.01f;

    public abstract float WorldLength { get; } // 坐标轴在 World 中的长度

    public bool          isTextShown = true; // 坐标轴文本是否显示
    public float         fontSize    = 2;    // 字体大小
    public TextAlign     textAlign;          // 字体对齐方式
    public Color         textColor;          // 字体颜色

    [Range(-0.5f, 0.5f)]
    public float valueTextOffset = 0; // 坐标值文本偏移量

    // 记录起始坐标轴范围
    [SerializeField]
    private float _startMaxIndex;
    [SerializeField]
    private float _startMinIndex;

    protected virtual void Awake() { }

    public virtual void Start() {
        _startMaxIndex = maxIndex;
        _startMinIndex = minIndex;
        useCullingMasks = true; // 使用遮罩剔除
    }

    /// <summary>
    /// 向坐标轴添加新值，并更新坐标轴范围
    /// </summary>
    /// <param name="value">新值</param>
    public abstract void ReceiveNewValue(float value);

    /// <summary>
    /// 根据 value 获取对应的坐标轴位置 (pos, 0) 或 (0, pos)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public float GetPosFromValue(float value) {
        return -WorldLength / 2 + WorldLength * (value - minIndex) / (maxIndex - minIndex);
    }

    /// <summary>
    /// 判断 value 是否超出坐标轴范围
    /// </summary>
    /// <param name="value"></param>
    /// <returns>超出上界，返回 1；超出下界，返回 -1；在范围内，返回 0</returns>
    public int IsOutOfRange(float value) {
        if (value > maxIndex) return 1;
        if (value < minIndex) return -1;
        return 0;
    }

    public void Show() {
        isTextShown = true;
        gameObject.SetActive(true);
    }

    public void Hide() {
        isTextShown = false;
        gameObject.SetActive(false);
    }

    public void ShowGrid() {
        isGridShown = true;
    }

    public void HideGrid() {
        isGridShown = false;
    }

    public virtual void Reset() {
        maxIndex = _startMaxIndex;
        minIndex = _startMinIndex;
    }
}