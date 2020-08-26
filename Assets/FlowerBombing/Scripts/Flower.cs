using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowerBombing { 
    public class Flower : MonoBehaviour
    {

        public Flower() {
            Debug.Log("1st Flower Constructor Called");
        }

        public Flower(string str)
        {
            Debug.Log(str);

        }
        public virtual void InstantiateFlower(int myFlowerTexIndex, int FlowerIndex, int flowerVarietyCount, float randomness, int toplayerCount, int index, float radius, Transform parent) {
            Debug.Log("Flower Instantiated!");

        }


    }
}