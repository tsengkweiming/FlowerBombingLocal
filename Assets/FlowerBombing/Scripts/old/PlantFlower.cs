using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlantFlower : MonoBehaviour
{
    [Header("Resources")]

    public Texture2D[] m_FlowerTex;
    [Disable]
    public int myFlowerTexIndex;
    MaterialPropertyBlock _flowerBlock;
    Renderer _renderer;
    [SerializeField, Disable]
    float time, dt;
    Color color;
    int flowerTexIndex;
    [SerializeField]
    [Range(-1.0f, 2.0f)]
    float YAxisOffSet = 0;
    public Vector3 target;
    [Range(-1, 1)]
    public int dir = 1;
    public bool isMyFlower = false;
    public bool isCreateMyFlower = false;
    public float RotateY;
    public float offsetTime;
    public float saturation;
    public float hue;
    public int turns = 0; //get set limit range
    [Range(5, 15)]
    public float FPS = 5; //get set limit range
    public static int stopMotion = 1;

    float scatterRate;

    //public MaterialPropertyBlock Block
    //{
    //    get
    //    {
    //        if (_flowerBlock == null)
    //        {
    //            var rndr = GetComponent<Renderer>();
    //            _flowerBlock = new MaterialPropertyBlock();
    //        }
    //        return _flowerBlock;
    //    }
    //}
    void Init()
    {
        this.time = 0;
        this.flowerTexIndex = Random.Range(0, m_FlowerTex.Length);
    }

    // Start is called before the first frame update
    void Start()
    {
        //transform.rotation = Quaternion.Euler(this.transform.rotation.x, rotY, this.transform.rotation.z);

        Init();
        _flowerBlock = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();
        _renderer.GetPropertyBlock(_flowerBlock);
        _flowerBlock.SetTexture("_FlowerTex", isMyFlower || isCreateMyFlower ? m_FlowerTex[myFlowerTexIndex] : m_FlowerTex[flowerTexIndex]);
        _renderer.SetPropertyBlock(_flowerBlock);
        scatterRate = Random.Range(0.0f, 1.0f);
        //RotateY = 0;// Random.Range(-90.0f, 90.0f);
        //transform.rotation = Quaternion.Euler(0, RotateY, 0);

        //transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = transform.position - (target + new Vector3(0, YAxisOffSet, 0));
        Quaternion rotation = Quaternion.LookRotation(direction * dir);
        transform.rotation = rotation;
        transform.rotation *= Quaternion.Euler(0, RotateY, 0);

        dt = Time.deltaTime;
        this.time += dt * stopMotion;
        _renderer.GetPropertyBlock(_flowerBlock);
        _flowerBlock.SetFloat("_StepTime", isMyFlower ? 20 : this.time);
        _flowerBlock.SetInt("_Turns", turns);
        _flowerBlock.SetFloat("_ScatterRate", scatterRate);
        //_flowerBlock.SetFloat("_Saturation", isMyFlower ? 0 : 0.3f);
        _flowerBlock.SetFloat("_Saturation", isMyFlower ? saturation : saturation);
        _flowerBlock.SetFloat("_Hue", isMyFlower ? hue : hue);
        _flowerBlock.SetFloat("_AnimTex_OffsetTime", offsetTime);
        _flowerBlock.SetFloat("_AnimTex_FPS", FPS);
        _flowerBlock.SetVector("_AnimTex_AnimEnd", new Vector4(250/FPS, 250, 0, 0));
        _renderer.SetPropertyBlock(_flowerBlock);
        //Debug.Log("stopMotion: " + stopMotion);

    }
}
