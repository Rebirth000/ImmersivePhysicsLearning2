using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPress : MonoBehaviour
{
    public Transform visual;

    public float moveSpeed   = 10f; // 移动速度
    public float maxDistance = 0.02f;

    public Vector3 targetPos; // 目标位置

    void Awake() {
        visual = DataSetting.GetComponentFromChild<Transform>(transform, "Button/Visuals");
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        targetPos = PressInput.Instance.NormedValue * maxDistance * Vector3.forward;
        visual.localPosition = Vector3.Lerp(visual.localPosition, targetPos, moveSpeed * Time.deltaTime);
    }
}
