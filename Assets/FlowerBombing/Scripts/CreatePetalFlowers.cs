using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowerBombing
{
    public class CreatePetalFlowers : Flower
    {
        float Hue, Saturation, Value;

        public CreatePetalFlowers() {
            Debug.Log("CreatePetalFlowers Constructor Called");
        }

        public override void InstantiateFlower(int myFlowerTexIndex, int myFlowerIndex, int flowerVarietyCount, float randomness, int count, int index, float radius, Transform parent)
        {
            var param = FlowerInstantiateParameters.Instance;
            float Angle = 360 * Mathf.Deg2Rad;
            Vector3 pos;
            int x = count;
            int turns = index / x == 0 ? 4 : index / x == 1 ? 5 : 0;
            float Rad = turns > 1 ? Mathf.Pow(0.97f, turns) * radius : radius;
            
            int i = index - x * (turns - 4);
            int div = x;// turns == 2 ? x : 2 * x;
            float angleOff = Angle / x / ((6 - turns));
            float rndOffset = UnityEngine.Random.Range(-param.angleOffsetRndRange * 2, param.angleOffsetRndRange * 2) * Mathf.Deg2Rad;
            float _z = 0 + Mathf.Sin(Angle / div * i + angleOff + rndOffset) * Rad;
            float _x = 0 + Mathf.Cos(Angle / div * i + angleOff + rndOffset) * Rad;
            pos = new Vector3(_x, param.turnsYOffset * (turns ), _z);// 0.5f

            int rndIndex;
            rndIndex = indexWithRandomness(myFlowerIndex, randomness, flowerVarietyCount);

            //flower init
            var flower = Instantiate(param.m_Flowers[rndIndex], pos, param.transform.rotation);
            flower.transform.localScale = param.petalScale;
            flower.transform.parent = parent;
            Vector3 lookAtRef = param.LookAtTarget.transform.position;
            lookAtRef = new Vector3(lookAtRef.x, pos.y + param.sFLookAtYOffset + Mathf.Pow((turns - 3), 2) * 0.1f, lookAtRef.z);
            flower.GetComponent<PlantFlower>().target = lookAtRef;
            flower.GetComponent<PlantFlower>().RotateY = UnityEngine.Random.Range(-param.selfRotationRndRange, param.selfRotationRndRange);
            flower.GetComponent<PlantFlower>().myFlowerTexIndex = myFlowerTexIndex;

            float texProb = Random.Range(0.0f, 1.0f);
            if (rndIndex == myFlowerIndex && texProb >= param.textureRandomness)
            {
                flower.GetComponent<PlantFlower>().isCreateMyFlower = true;
            }
            float blosomProb = Random.Range(0.0f, 1.0f);

            Color.RGBToHSV(param.HSVKey, out Hue, out Saturation, out Value);

            flower.GetComponent<PlantFlower>().turns = turns;
            flower.GetComponent<PlantFlower>().FPS = param.petalsFPS;
            float animTime = Random.Range(10.0f, 14.0f);
            flower.GetComponent<PlantFlower>().offsetTime = animTime + param.Anim_OffsetTime * 1.0f;
            flower.GetComponent<PlantFlower>().saturation = Saturation;
            flower.GetComponent<PlantFlower>().hue = Hue + Random.Range(-param.hueRange, param.hueRange);
        }

        int indexWithRandomness(int FlowerIndex, float randomness, int listCount)
        {   //blending 0->FlowerIndex, 0.5->RandomIndex, 1 -> only other //randomness = 0.8 -> average
            int index = 0;
            float totalIncrement = 0;
            float[] weights;
            weights = new float[listCount]; //number of things
            for (int i = 0; i < listCount; i++)
            {
                weights[i] = (randomness) / (listCount - 1);
            }
            weights[FlowerIndex] = 1 - (listCount - 1) * (randomness) / (listCount - 1);

            float randVal = UnityEngine.Random.Range(0.0f, 1.0f);
            for (index = 0; index < weights.Length; index++)
            {
                totalIncrement += weights[index];
                if (totalIncrement >= randVal) break;
            }
            return index;
        }
    }

}