using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Mathematics;
using static Unity.Mathematics.math;

public class HumanBehaviourParameter : SingletonMonoBehaviour<HumanBehaviourParameter>
{
    [Header("Parameters")]
    public Color[] HumanColor;
    public float DistanceWSJudgeNewHuman = 0.1f;

    public int MaxHumanBehaviourDataListNum = 64;

    public float MaxVelocityWS = 1.0f;

    public float VelocityMagnitudeLerpAmpScale = 1.0f;

    public float WallThresholdVelocityDivision = 0.2f;

    public float ThresholdVelocityWalk = 0.2f;

    public float ThresholdVelocityRun = 0.25f;

    public float MaterialPointRadiusWS = 0.02f;
    public float GravityRadiusAmplitude = 2.0f;
    public float GravityPowerAmplitude = 3.0f;

    public float GravityRadiusIncRateWhileStaying = 1.0f;
    public float GravityPowerIncRateWhileStaying = 1.0f;

    public float GravityRadiusDesRateWhileWalking = 1.0f;
    public float GravityPowerDesRateWhileWalking = 1.0f;

    public float GravityRadiusDesRateWhileRunning = 2.0f;
    public float GravityPowerDesRateWhileRunning = 2.0f;

    public float TimeDurationToDead = 2.0f;

    public float GravityRadiusIncRateWhileRunning = 2.0f;
    public float GravityLerpAmpWhileStaying = 0.01f;
    public float GravityLerpAmpWhileWalking = 1.0f;
    public float GravityLerpAmpWhileRunning = 1.0f;
    public float ExpectedPosIncAmp;
    public float NewPosFilterDistance;
    public float InteractionGravityIncWhileBigBang = -20;
    public float InteractionGravityIncLeavingBigBang = 15;

    public float StayingMinRad = 0.2f;
    public float StayingMaxRad = 1;
    public float WalkingMinRad = 0.2f;
    public float WalkingMaxRad = 1;
    public float RunningMinRad = 0.1f;
    public float RunningMaxRad = 2;
    public float2 SensoringRangeY = float2(0, 1);

    [Header("Debug")]

    public bool IsDrawDebugObject = true;

    public float DebugObjectScale = 1.0f;

    public Color StateStayColor = Color.cyan;

    public Color StateWalkColor = Color.yellow;

    public Color StateBigBangColor = Color.red;

    public Color StateRunColor = Color.magenta;

}
