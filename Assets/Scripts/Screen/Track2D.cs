using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;
using UnityEngine.Serialization;

// [ExecuteAlways]
public class Track2D : MonoBehaviour
{
    public Transform track3D;

    public float track2DLength;
    public float track3DLength;

    public Transform glassBase3D;
    public Transform woodBase3D;

    public Rectangle glassBase2D;
    public Rectangle woodBase2D;

    public Transform couple3D;
    public Transform spring2D;

    public Transform blockA3D;
    public Transform blockA2D;
    public Transform blockB3D;
    public Transform blockB2D;

    public Arrow forceArrowA;
    public Arrow velocityArrowA;
    public Arrow forceArrowB;
    public Arrow velocityArrowB;

    public Vector3 springStartScale;
    public float   springStartLength;

    private void Awake() {
        track3D = DataSetting.GetComponent<Transform>("Objects/LabTable/Track");

        glassBase3D = DataSetting.GetComponentFromChild<Transform>(track3D, "TrackModel/GlassBase");
        woodBase3D  = DataSetting.GetComponentFromChild<Transform>(track3D, "TrackModel/WoodBase");

        glassBase2D = DataSetting.GetComponentFromChild<Rectangle>(transform, "TrackSprite_2D/GlassBase_2D");
        woodBase2D  = DataSetting.GetComponentFromChild<Rectangle>(transform, "TrackSprite_2D/WoodBase_2D");

        couple3D = DataSetting.GetComponentFromChild<Transform>(track3D, "Couple");
        spring2D = DataSetting.GetComponentFromChild<Transform>(transform, "Couple_2D/Spring_2D");
        blockA3D = DataSetting.GetComponentFromChild<Transform>(track3D, "Couple/BlockA");
        blockB3D = DataSetting.GetComponentFromChild<Transform>(track3D, "Couple/BlockB");
        blockA2D = DataSetting.GetComponentFromChild<Transform>(transform, "Couple_2D/BlockA_2D");
        blockB2D = DataSetting.GetComponentFromChild<Transform>(transform, "Couple_2D/BlockB_2D");

        forceArrowA    = DataSetting.GetComponentFromChild<Arrow>(transform, "Couple_2D/BlockA_2D/ForceArrow");
        forceArrowB    = DataSetting.GetComponentFromChild<Arrow>(transform, "Couple_2D/BlockB_2D/ForceArrow");
        velocityArrowA = DataSetting.GetComponentFromChild<Arrow>(transform, "Couple_2D/BlockA_2D/VelocityArrow");
        velocityArrowB = DataSetting.GetComponentFromChild<Arrow>(transform, "Couple_2D/BlockB_2D/VelocityArrow");
    }

    private void Start() {
        float glassLength = glassBase3D.localScale.z;
        float woodLength  = woodBase3D.localScale.z;
        track3DLength                       = glassLength + woodLength;
        glassBase2D.Width                   = glassLength / track3DLength * track2DLength;
        woodBase2D.Width                    = woodLength / track3DLength * track2DLength;
        glassBase2D.transform.localPosition = Vector3.right * woodBase2D.Width / 2;
        woodBase2D.transform.localPosition  = Vector3.left * glassBase2D.Width / 2;
        blockA2D.localPosition              = Vector3.right * (couple3D.localPosition.z + blockA3D.localPosition.z) / track3DLength * track2DLength;
        blockB2D.localPosition              = Vector3.right * (couple3D.localPosition.z + blockB3D.localPosition.z) / track3DLength * track2DLength;

        springStartScale  = spring2D.localScale;
        springStartLength = Mathf.Abs(blockA2D.localPosition.x - blockB2D.localPosition.x) - 0.3f;
        Debug.Log($"springStartLength: {springStartLength}");
    }


    private void Update() {
        blockA2D.localPosition = Vector3.right * (couple3D.localPosition.z + blockA3D.localPosition.z) / track3DLength * track2DLength;
        blockB2D.localPosition = Vector3.right * (couple3D.localPosition.z + blockB3D.localPosition.z) / track3DLength * track2DLength;
        spring2D.localPosition = Vector3.right * (blockA2D.localPosition.x + blockB2D.localPosition.x) / 2;
        float dis = Mathf.Abs(blockA2D.localPosition.x - blockB2D.localPosition.x) - 0.3f;
        spring2D.localScale = new Vector3(springStartScale.x, springStartScale.y * dis / springStartLength,
                                          springStartScale.z);

        bool isShown = DataSetting.Instance.couple.IsShowArrows;
        forceArrowA.gameObject.SetActive(isShown);
        forceArrowB.gameObject.SetActive(isShown);
        velocityArrowA.gameObject.SetActive(isShown);
        velocityArrowB.gameObject.SetActive(isShown);

        if (isShown) {
            forceArrowA.SetValue(DataSetting.Instance.blockA.MoveForce, DataSetting.Instance.blockA.MaxForce);
            forceArrowB.SetValue(DataSetting.Instance.blockB.MoveForce, DataSetting.Instance.blockB.MaxForce);
            velocityArrowA.SetValue(DataSetting.Instance.blockA.MoveVelocity, DataSetting.Instance.blockA.MaxVelocity);
            velocityArrowB.SetValue(DataSetting.Instance.blockB.MoveVelocity, DataSetting.Instance.blockB.MaxVelocity);
        }
    }
}