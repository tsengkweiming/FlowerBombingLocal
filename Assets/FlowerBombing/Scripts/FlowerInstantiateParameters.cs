using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FlowerBombing;

public class FlowerInstantiateParameters : SingletonMonoBehaviour<FlowerInstantiateParameters>
{
    [Header("Flower Group")]
    public GameObject _myFlower;
    public GameObject _surroundedFlower;
    public GameObject _petalFlower;

    [Header("Resources")]
    public GameObject LookAtTarget;
    public GameObject[] m_Flowers;
    [Header("Instantiate Bounds")]
    public CircleCollider2D[] flowerBounds;
    [Range(0, 0.5f)]
    public float myFlowerRadius = 1.0f;
    [Range(0, 3)]
    public float surroundedFlowerRadius = 0.8f;
    [Range(0, 3)]
    public float petalFlowerRadius = 1.5f;

    [Header("Instantiate Parameters")]
    [Range(1, 5)]
    public int myFlowerAmount = 3;
    [Range(2, 6)]
    public int topLayerAmount = 4;
    [Range(10, 30)]
    public int petalLayerAmount = 16;

    [Range(0.1f, 2)]
    public float instantiateTimeStep = 1.0f;
    public float mFLookAtYOffset = 0;
    public float sFLookAtYOffset = 0;
    [Range(0, 1)]
    public float varietyRandomness = 1.0f;
    [Range(0, 1)]
    public float textureRandomness = 1.0f;
    [Range(0, 1)]
    public float radiusReducePerTurn = 0.6f;
    [Range(-2, 0)]
    public float turnsYOffset = -0.7f;
    [Range(0, 30)]
    public float angleOffsetRndRange = 10f;
    [Range(0, 90)]
    public float selfRotationRndRange = 10f;
    [Range(0, 1)]
    public float hueRange = 0.025f;
    public Color HSVKey;
    public List<CreateFlowerSpeciesParam> m_FlowerSpeciesParamPreset;
    [Range(0, 1)]
    public float Layer3ScatterProbability = 0.65f;
    [Range(0, 1)]
    public float Layer2ScatterProbability = 0.5f;
    [Range(0, 10)]
    public float Layer3ScatterOffsetTime = 9;
    [Range(-5, 5)]
    public float Layer2ScatterOffsetTime = -3;
    [Range(0, 50)]
    public float Anim_OffsetTime = 0;
    [Range(5, 15)]
    public float petalsFPS = 5;
    public Vector3 petalScale = new Vector3(0.5f, 0.5f, 0.5f);

    [Range(2, 10)]
    public int secToStop = 5;

    [Header("Debug")]
    public Text text;

    //[SerializeField]
    //bool flowerCreationSwitcher = false;
    void OnDrawGizmos()
    {
        Gizmos.color = Color.grey;

        Gizmos.DrawLine(new Vector3(-35.8f, 0, 0), new Vector3(35.8f, 0, 0));
        Gizmos.DrawLine(new Vector3(0, 0, -35.8f), new Vector3(0, 0, 35.8f));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(0, -35.8f, 0), new Vector3(0, 35.8f, 0));

        Gizmos.DrawLine(new Vector3(-35.8f, -4.5f, 0), new Vector3(35.8f, -4.5f, 0));
        Gizmos.DrawLine(new Vector3(0, -4.5f, -35.8f), new Vector3(0, -4.5f, 35.8f));

        Gizmos.color = Color.yellow;
        flowerBounds[0].radius = myFlowerRadius;
        flowerBounds[1].radius = surroundedFlowerRadius;
        flowerBounds[2].radius = petalFlowerRadius;

        for (int i = 0; i < flowerBounds.Length; i++)
        {
            UnityEditor.Handles.color = new Color(i, 1, 0, 1);
            UnityEditor.Handles.DrawWireDisc(flowerBounds[i].transform.position, Vector3.up, flowerBounds[i].radius);

        }
    }
}
