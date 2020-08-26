using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowerBombing
{
    public class CreateMyFlowers : Flower
    {
        GameObject centerFlower;
        float Hue, Saturation, Value;

        public CreateMyFlowers() { 
            Debug.Log("1st CreateMyFlowers Constructor Called");

        }

        public override void InstantiateFlower(int myFlowerTexIndex,int FlowerIndex, int flowerVarietyCount, float randomness, int count, int index, float radius, Transform parent)
        {
            var param = FlowerInstantiateParameters.Instance;
            var speciesParam = param.m_FlowerSpeciesParamPreset[FlowerIndex].SpeciesParam;
            //param.text.text = theFlower.name;

            float Angle = 360 * Mathf.Deg2Rad;
            Vector3 pos;
            for (int i = 0; i < count; i++)
            {
                float _z = 0 + Mathf.Sin(Angle / count * i) * radius;
                float _x = 0 + Mathf.Cos(Angle / count * i) * radius;
                pos = new Vector3(_x, 0, _z);
                centerFlower = Instantiate(param.m_Flowers[FlowerIndex], pos, param.m_Flowers[FlowerIndex].transform.rotation); //param.m_Flowers[FlowerIndex]
                centerFlower.transform.parent = parent;
                Vector3 lookAtRef = param.LookAtTarget.transform.position;
                lookAtRef = new Vector3(lookAtRef.x, pos.y + speciesParam.LookAtYOffset, lookAtRef.z);
                centerFlower.GetComponent<PlantFlower>().target = lookAtRef;
                centerFlower.GetComponent<PlantFlower>().isMyFlower = true;
                centerFlower.GetComponent<PlantFlower>().RotateY = UnityEngine.Random.Range(-param.selfRotationRndRange, param.selfRotationRndRange);
                centerFlower.GetComponent<PlantFlower>().myFlowerTexIndex = myFlowerTexIndex;

                Color.RGBToHSV(param.HSVKey, out Hue, out Saturation, out Value);
                centerFlower.GetComponent<PlantFlower>().saturation = Saturation;
                centerFlower.GetComponent<PlantFlower>().hue = Hue + Random.Range(-param.hueRange, param.hueRange);
            }
            //base.InstantiateFlower(FlowerIndex, flowerVarietyCount, randomness, toplayerCount, index, radius, parent);
        }
    }
}