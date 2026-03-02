using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPosTrigger : MonoBehaviour
{
    public float   moveSpeed = 10;
    public Vector3 targetPos;

    // Start is called before the first frame update
    void Start() {
        targetPos = DataSetting.Instance.baffleMove.transform.localPosition;
        targetPos.y = -0.032f;
    }

    // Update is called once per frame
    void Update() { }

    private void OnTriggerEnter(Collider other) {
        // EventMgr.Instance.EventTrigger<Collider>(nameof(Main.EEventType.StartPosTriggerDo), other);
        if (other.CompareTag("CoupleBlock")) {
            DataSetting.Instance.baffleMove.MoveTo(targetPos, moveSpeed, BaffleMove.EMoveType.Uniform);
        }
    }
}