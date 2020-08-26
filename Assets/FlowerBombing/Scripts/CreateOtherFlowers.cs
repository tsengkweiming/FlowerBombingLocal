using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowerBombing
{
    public class CreateOtherFlowers : Flower
    {
        float Hue, Saturation, Value;

        public CreateOtherFlowers() : base("starts to create other flower")
        {
            Debug.Log("1st CreateOtherFlowers Constructor Called");
        }

        public override void InstantiateFlower(int myFlowerTexIndex,int myFlowerIndex, int flowerVarietyCount, float randomness, int toplayerCount, int index, float radius, Transform parent)
        {
            var param = FlowerInstantiateParameters.Instance;

            float Angle = 360 * Mathf.Deg2Rad;
            Vector3 pos;
            int x = toplayerCount;
            int turns = index / x == 0 ? 1 : index / x == 1 ? 2 : index / x == 2 || index / x == 3 ? 3 : 0;
            float Rad = turns < 3 ? Mathf.Pow(param.radiusReducePerTurn, turns) * radius : radius;
            int i = index - x * (turns - 1);
            int div = turns == 1 || turns == 2 ? x : 2 * x;
            float angleOff = Angle / x;
            angleOff = turns == 1 ? 0 : angleOff / (2 * (turns - 1));
            float rndOffset = UnityEngine.Random.Range(-param.angleOffsetRndRange, param.angleOffsetRndRange) * Mathf.Deg2Rad;
            float _z = 0 + Mathf.Sin(Angle / div * i + angleOff + rndOffset) * Rad;
            float _x = 0 + Mathf.Cos(Angle / div * i + angleOff + rndOffset) * Rad;
            pos = new Vector3(_x, param.turnsYOffset * (turns), _z);

            int rndIndex;
            rndIndex = indexWithRandomness(myFlowerIndex, randomness, flowerVarietyCount);

            //flower init
            var flower = Instantiate(param.m_Flowers[rndIndex], pos, param.transform.rotation);
            flower.transform.parent = parent;
            Vector3 lookAtRef = param.LookAtTarget.transform.position;
            lookAtRef = new Vector3(lookAtRef.x, pos.y + param.sFLookAtYOffset + Mathf.Pow((turns - 1), 2) * 0.08f, lookAtRef.z);
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
            float animTime = turns == 3 && blosomProb > 1 - param.Layer3ScatterProbability ? param.Layer3ScatterOffsetTime : turns == 3 ? param.Layer3ScatterOffsetTime/3 : turns == 2 && blosomProb > param.Layer2ScatterProbability ? param.Layer2ScatterOffsetTime : 0;
            flower.GetComponent<PlantFlower>().offsetTime = animTime + param.Anim_OffsetTime;
            flower.GetComponent<PlantFlower>().saturation = Saturation;
            flower.GetComponent<PlantFlower>().hue = Hue + Random.Range(-param.hueRange, param.hueRange);

            //Debug.Log("texProb " + texProb + ",  blosomProb " + blosomProb);
            //base.InstantiateFlower();
        }
        int indexWithRandomness(int FlowerIndex, float randomness, int listCount)
        {  //blending 0->FlowerIndex, 0.5->RandomIndex, 1 -> only other //randomness = 0.8 -> average (total 5 variety)
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
