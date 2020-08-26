using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowerBombing { 

    [CreateAssetMenu(fileName = "FSP_", menuName = "FlowerBombing/Create Flower Species Param Asset", order = 1)]
    public class CreateFlowerSpeciesParam : ScriptableObject
    {
        public FlowerSpecies SpeciesParam;
        [System.Serializable]
        public class FlowerSpecies {
            public SpeciesType Type;
            public float MyFlowerRadius;
            public float LookAtYOffset;
            public float Hue;
        }

        [System.Serializable]
        public enum SpeciesType
        {
            Asamafuuro = 0,
            Higotai = 1,
            Hoteiran = 2,
            Kikyou = 3,
            Ooakabana = 4,
            Test = 10
        };
    }
}