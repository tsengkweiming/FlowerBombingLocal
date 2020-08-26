using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine.UI;

namespace FlowerBombing.old {
    public class FlowersManager : MonoBehaviour
    {
        [Header("Flower Group")]
        public GameObject _myFlower;
        public GameObject _surroundedFlower;

        [Header("Resources")]
        public GameObject LookAtTarget;
        public GameObject[] m_Flowers;
        [Header("Instantiate Bounds")]
        public CircleCollider2D[] flowerBounds;
        [SerializeField]
        [Range(0, 0.5f)]
        float myFlowerRadius = 1.0f;
        [SerializeField]
        [Range(0, 3)]
        float surroundedFlowerRadius = 0.8f;

        [Header("Instantiate Parameters")]
        [SerializeField]
        [Range(1, 5)]
        int myFlowerAmount = 3;
        [SerializeField]
        [Range(2, 6)]
        int topLayerAmount = 4;
        [SerializeField, Disable]
        [Range(1, 30)]
        int surroundedAmount = 11;
        [SerializeField]
        [Range(0.01f, 2)]
        float instantiateTimeStep = 1.0f;
        [SerializeField]
        float mFLookAtYOffset = 0;
        [SerializeField]
        float sFLookAtYOffset = 0;
        [SerializeField]
        [Range(0, 1)]
        float varietyRandomness = 1.0f;
        [SerializeField]
        [Range(0, 1)]
        float radiusReducePerTurn = 0.6f;
        [SerializeField]
        [Range(-2, 0)]
        float turnsYOffset = -0.7f;
        [SerializeField]
        [Range(0, 30)]
        float angleOffsetRndRange = 10f;
        [SerializeField]
        [Range(0, 90)]
        float selfRotationRndRange = 10f;

        [Header("Debug")]
        [SerializeField]
        bool flowerCreationSwitcher = false;
        bool myFlowerCreationSwitcher = false;
        [SerializeField]
        int rndIndex = 0;
        [SerializeField]
        float finishInstantiateSec = 5.0f;
        [SerializeField]
        float createTimer;
        [SerializeField]
        [Range(0, 1)]
        float offsetPerTurn = 0.6f;
        public Text text;
        [Range(0, 4)]
        [SerializeField]
        int theFlowerIndex;

        [Header("Angles")]

        [Header("Private Variables")]
        int indexLength;
        GameObject centerFlower;
        Coroutine flowerCreationCo;
        GameObject theFlower;

        struct flower {
            public FlowersType Type;
            bool isMyFlower;
            Vector3 target;
            float radius;
        }
        [System.Serializable]
        public enum FlowersType
        {
            Asamafuuro = 0,
            Higotai = 1,
            Hoteiran = 2,
            Kikyou = 3,
            Ooakabana = 4,
            Rnd = 5
        };
        struct surroundedFlower { 
        
        }
        // Start is called before the first frame update
        void Start()
        {
            surroundedAmount = 4 * topLayerAmount;
            indexLength = m_Flowers.Length;
            theFlowerIndex = UnityEngine.Random.Range(0, indexLength);
            theFlower = m_Flowers[theFlowerIndex];
            //centerFlower = Instantiate(theFlower, this.transform.position, theFlower.transform.rotation);
            //centerFlower.transform.parent = _myFlower.transform;
            InstantiateMyFlowers(theFlower, myFlowerAmount, myFlowerRadius, LookAtTarget,_myFlower.transform);

            flowerCreationSwitcher = true;
            int[] rndList = rndIndexList(surroundedAmount);

            flowerCreationCo = StartCoroutine(CreateOtherFlowerCoroutine(rndList, 0));
            finishInstantiateSec = (surroundedAmount) * instantiateTimeStep;
        }

        void InstantiateMyFlowers(GameObject flowerGO, int count, float radius, GameObject LookAtTarget, Transform parent) {
            text.text = theFlower.name;

            float Angle = 360 * Mathf.Deg2Rad;
            Vector3 pos;
            for (int i = 0; i < count; i++) {
                float _z = transform.position.z + Mathf.Sin(Angle / count * i) * radius;
                float _x = transform.position.x + Mathf.Cos(Angle / count * i) * radius;
                pos = new Vector3(_x, 0, _z);
                centerFlower = Instantiate(flowerGO, pos, flowerGO.transform.rotation);
                centerFlower.transform.parent = parent;
                Vector3 lookAtRef = LookAtTarget.transform.position;
                lookAtRef = new Vector3(lookAtRef.x, pos.y + mFLookAtYOffset, lookAtRef.z);
                centerFlower.GetComponent<PlantFlower>().target = lookAtRef;
                centerFlower.GetComponent<PlantFlower>().isMyFlower = true;
                centerFlower.GetComponent<PlantFlower>().RotateY = UnityEngine.Random.Range(-selfRotationRndRange, selfRotationRndRange);

            }
        }

        void InstantiateOtherFlowers(int myFlowerIndex, int flowerVarietyCount,float randomness, int toplayerCount, int index, float radius, Transform parent) {
            float Angle = 360 * Mathf.Deg2Rad;
            Vector3 pos;
            int x = toplayerCount;
            int turns = index / x == 0 ? 3 : index / x == 1 ? 2 : index / x == 2 || index / x == 3 ? 1 : 0;
            float Rad = turns > 1 ? Mathf.Pow(radiusReducePerTurn, turns) * radius : radius;
            int i = index - x * (3 - turns);
            int div = turns == 3 || turns == 2 ? x : 2 * x;
            float angleOff = Angle / x;
            angleOff = turns == 3 ? 0 : angleOff / (2 * (3 - turns));
            float rndOffset = UnityEngine.Random.Range(-angleOffsetRndRange, angleOffsetRndRange) * Mathf.Deg2Rad;
            float _z = transform.position.z + Mathf.Sin(Angle / div * i + angleOff + rndOffset) * Rad;
            float _x = transform.position.x + Mathf.Cos(Angle / div * i + angleOff + rndOffset) * Rad;
            pos = new Vector3(_x, turnsYOffset * (4 - turns), _z);

            //rndIndex = UnityEngine.Random.Range(0, indexLength);
            rndIndex = indexWithRandomness(myFlowerIndex, randomness, flowerVarietyCount);

            //flower init
            var flower = Instantiate(m_Flowers[rndIndex], pos, transform.rotation);
            flower.transform.parent = parent;
            Vector3 lookAtRef = LookAtTarget.transform.position;
            lookAtRef = new Vector3(lookAtRef.x, pos.y + sFLookAtYOffset + Mathf.Pow((3 - turns), 2) * 0.08f, lookAtRef.z);
            flower.GetComponent<PlantFlower>().target = lookAtRef;
            flower.GetComponent<PlantFlower>().RotateY = UnityEngine.Random.Range(-selfRotationRndRange, selfRotationRndRange);
        }

        // Update is called once per frame
        void Update()
        {
            surroundedAmount = 4 * topLayerAmount;
            finishInstantiateSec = (surroundedAmount) * instantiateTimeStep;

            if (createTimer > finishInstantiateSec) {
                // finish instantiation
                flowerCreationSwitcher = false;
                createTimer = 0;
                Debug.Log("Finished");
            }
            createTimer += Time.deltaTime * (flowerCreationSwitcher ? 1 : 0);

        }

        public void RecreateMyFlower() {
            foreach (Transform child in _myFlower.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            theFlower = m_Flowers[theFlowerIndex];
            InstantiateMyFlowers(theFlower, myFlowerAmount, myFlowerRadius, LookAtTarget,_myFlower.transform);

        }

        IEnumerator CreateOtherFlowerCoroutine(int[] indexList, int index)
        {
            while (flowerCreationSwitcher)
            {
                yield return new WaitForSeconds(instantiateTimeStep);
                InstantiateOtherFlowers(theFlowerIndex, indexLength, varietyRandomness, topLayerAmount, indexList[index], surroundedFlowerRadius, _surroundedFlower.transform);
                index++;
            }
        }

        public void CreateOrRecreateSurroundedFlowers() {
            StopCoroutine(flowerCreationCo);
            foreach (Transform child in _surroundedFlower.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            flowerCreationSwitcher = true;
            createTimer = 0;
            int[] rndList = rndIndexList(surroundedAmount);
            flowerCreationCo = StartCoroutine(CreateOtherFlowerCoroutine(rndList, 0)); //only one thread can exist
        }

        int[] rndIndexList(int countInList) {
            int maxIndex = countInList - 1; //max is the max index of the array elements
            int minIndex = 0;

            int count = maxIndex - minIndex + 1;
            int[]  rndIndexList = new int[count];//new array

            for (int i = 0; i < count; i++)//Start array creation
            {
                //Generate random number
                int j = UnityEngine.Random.Range(0, i + 1);

                rndIndexList[i] = rndIndexList[j];
                rndIndexList[j] = minIndex + i;
            }

            return rndIndexList;
        }

        int indexWithRandomness(int myFlowerIndex, float randomness, int listCount) {  //blending 0->myFlowerIndex, 1->RandomIndex
            int index = 0;
            float totalIncrement = 0;
            float[] weights;
            weights = new float[listCount]; //number of things
            for (int i = 0; i < listCount; i++) {
                weights[i] =  (randomness) / listCount;
            }
            weights[myFlowerIndex] = 1 - (listCount - 1) * (randomness) / listCount;

            float randVal = UnityEngine.Random.Range(0.0f, 1.0f);
            for (index = 0; index < weights.Length; index++)
            {
                totalIncrement += weights[index];
                if (totalIncrement >= randVal) break;
            }
            return index;
        }
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(new Vector3(0, -17.6f, 0), new Vector3(0, 17.6f, 0));
            Gizmos.DrawLine(new Vector3(-17.6f, 0, 0), new Vector3(17.6f, 0, 0));
            Gizmos.DrawLine(new Vector3(0, 0, -17.6f), new Vector3(0, 0, 17.6f));

            Gizmos.color = Color.yellow;
            flowerBounds[0].radius = myFlowerRadius;
            flowerBounds[1].radius = surroundedFlowerRadius;

            for (int i = 0; i < flowerBounds.Length; i++) {
                UnityEditor.Handles.color = new Color(i, 1, 0, 1);
                UnityEditor.Handles.DrawWireDisc(flowerBounds[i].transform.position, Vector3.up, flowerBounds[i].radius);

            }
        }
    }

}