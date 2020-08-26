using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GPUDuneSystem;

namespace FlowerBombing
{
    public class FlowerManager : MonoBehaviour
    {
        CreateMyFlowers myFlower;
        CreateOtherFlowers otherFlower;
        CreatePetalFlowers petalFlowers;
        [SerializeField, Disable]
        [Range(1, 30)]
        int surroundedAmount = 11;

        [Header("Debug")]
        [SerializeField]
        bool flowerCreationSwitcher = false;

        [Header("Private Variables")]

        [SerializeField]
        float createTimer;
        [SerializeField]
        float finishInstantiateSec = 5.0f;
        [Range(0, 4)]
        [SerializeField]
        int theFlowerIndex;
        int indexLength;
        Coroutine flowerCreationCo;
        GameObject theFlower;
        [SerializeField]
        int myFlowerTexIndex;

        [SerializeField, Disable]
        float sceneTimer = 0;
        // Start is called before the first frame update
        void Start()
        {
            var param = FlowerInstantiateParameters.Instance;

            surroundedAmount = 4 * param.topLayerAmount;
            indexLength = param.m_Flowers.Length;
            theFlowerIndex = UnityEngine.Random.Range(0, indexLength);
            //theFlower = param.m_Flowers[theFlowerIndex];

            myFlower = new CreateMyFlowers();
            otherFlower = new CreateOtherFlowers();
            petalFlowers = new CreatePetalFlowers();

            CreateOrRecreateMyFlower();
            CreateOrRecreateSurroundedFlowers();
            CreateOrRecreatePetals();
        }

        // Update is called once per frame
        void Update()
        {
            var param = FlowerInstantiateParameters.Instance;            
            finishInstantiateSec = (surroundedAmount) * param.instantiateTimeStep;

            if (createTimer > finishInstantiateSec)
            {
                // finish instantiation
                flowerCreationSwitcher = false;
                createTimer = 0;
                Debug.Log("Finished");
            }
            createTimer += Time.deltaTime * (flowerCreationSwitcher ? 1 : 0);

            sceneTimer += Time.deltaTime;
            if (sceneTimer >= param.secToStop)
            {
                PlantFlower.stopMotion = 0;
                GPUDune.stopMotion = 0;
            }
            else {
                PlantFlower.stopMotion = 1;
                GPUDune.stopMotion = 1;
            }
        }

        public void CreateOrRecreateMyFlower()
        {
            var param = FlowerInstantiateParameters.Instance;
            foreach (Transform child in param._myFlower.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            theFlower = param.m_Flowers[theFlowerIndex];
            param.text.text = theFlower.name;
            myFlowerTexIndex = Random.Range(0, 10);
            myFlower.InstantiateFlower(myFlowerTexIndex, theFlowerIndex, indexLength, param.varietyRandomness, param.myFlowerAmount, 0, param.myFlowerRadius, param._myFlower.transform);

        }

        public void CreateOrRecreateSurroundedFlowers()
        {
            var param = FlowerInstantiateParameters.Instance;
            if(flowerCreationCo != null) StopCoroutine(flowerCreationCo);
            foreach (Transform child in param._surroundedFlower.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            flowerCreationSwitcher = true;
            createTimer = 0;
            surroundedAmount = 4 * param.topLayerAmount;
            int[] rndList = rndIndexList(surroundedAmount);
            flowerCreationCo = StartCoroutine(CreateOtherFlowerCoroutine(rndList, 0)); //only one thread can exist
        }

        IEnumerator CreateOtherFlowerCoroutine(int[] indexList, int index)
        {
            while (flowerCreationSwitcher)
            {
                var param = FlowerInstantiateParameters.Instance;
                yield return new WaitForSeconds(param.instantiateTimeStep);
                otherFlower.InstantiateFlower(myFlowerTexIndex, theFlowerIndex, indexLength, param.varietyRandomness, param.topLayerAmount, indexList[index], param.surroundedFlowerRadius, param._surroundedFlower.transform);
                index++;
            }
        }

        public void CreateOrRecreatePetals()
        {
            var param = FlowerInstantiateParameters.Instance;
            foreach (Transform child in param._petalFlower.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            theFlower = param.m_Flowers[theFlowerIndex];
            param.text.text = theFlower.name;
            int[] rndList = rndIndexList(param.petalLayerAmount);
            for (int i = 0; i < param.petalLayerAmount; i++)
            {
                petalFlowers.InstantiateFlower(myFlowerTexIndex, theFlowerIndex, indexLength, param.varietyRandomness, param.petalLayerAmount/2, rndList[i], param.petalFlowerRadius, param._petalFlower.transform);
            
            }
        }
        public void setGlobalTimer() {
            sceneTimer = 0;
        }

        int[] rndIndexList(int countInList)
        {
            int maxIndex = countInList - 1; //max is the max index of the array elements
            int minIndex = 0;

            int count = maxIndex - minIndex + 1;
            int[] rndIndexList = new int[count];//new array

            for (int i = 0; i < count; i++)//Start array creation
            {
                //Generate random number
                int j = UnityEngine.Random.Range(0, i + 1);

                rndIndexList[i] = rndIndexList[j];
                rndIndexList[j] = minIndex + i;
            }

            return rndIndexList;
        }
    }
}