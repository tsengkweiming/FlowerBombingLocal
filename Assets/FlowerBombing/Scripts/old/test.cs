using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public int countInList;
    public int[] rndIndexList;//new array

    // Start is called before the first frame update
    void Start()
    {
        int maxIndex = countInList - 1; //max is the max index of the array elements
        int minIndex = 0;

        int count = maxIndex - minIndex + 1;
        rndIndexList = new int[count];//new array

        for (int i = 0; i < count; i++)//Start array creation
        {
            //Generate random number
            int j = UnityEngine.Random.Range(0, i + 1);

            rndIndexList[i] = rndIndexList[j];
            rndIndexList[j] = minIndex + i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
