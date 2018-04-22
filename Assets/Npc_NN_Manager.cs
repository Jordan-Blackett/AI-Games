using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc_NN_Manager : MonoBehaviour {

    private int[] layers = new int[] { 19, 10, 10, 5 }; //1 input and 1 output
    private List<NeuralNetwork> nets;
    private int populationSize = 8;
    private int generationNumber = 0;
    private bool isTraning = false;

    private List<Npc_NN> npcList = null;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void InitNPC()
    {
        //if (boomerangList != null)
        //{
        //    for (int i = 0; i < boomerangList.Count; i++)
        //    {
        //        GameObject.Destroy(boomerangList[i].gameObject);
        //    }

        //}

        //boomerangList = new List<Boomerang>();

        for (int i = 0; i < populationSize; i++)
        {
            //Boomerang boomer = ((GameObject)Instantiate(boomerPrefab, new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), 0), boomerPrefab.transform.rotation)).GetComponent<Boomerang>();
            //boomer.Init(nets[i], hex.transform);
            //boomerangList.Add(boomer);
        }
    }

    void InitNPCNeuralNetworks()
    {
        //population must be even, just setting it to 20 incase it's not
        if (populationSize % 2 != 0)
        {
            populationSize = 20;
        }

        nets = new List<NeuralNetwork>();

        for (int i = 0; i < populationSize; i++)
        {
            NeuralNetwork net = new NeuralNetwork(layers);
            net.Mutate();
            nets.Add(net);
        }
    }
}
