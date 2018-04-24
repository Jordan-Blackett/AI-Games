using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieChaseManager : MonoBehaviour {

    public GameObject zombiePrefab;
    public GameObject player;

    private int[] layers = new int[] { 7, 5, 5, 1 }; //1 input and 1 output
    private List<NeuralNetwork> nets;
    private int populationSize = 50;
    public int generationNumber = 0;
    private bool isTraning = false;

    private List<Zombie_NN> zombieList = null;

    // UI
    public float highestScore = 0;
    public float highestScoreGen = 0;

    public float[] debugValue = new float[50];

    void Timer()
    {
        isTraning = false;
    }

    void Update() {
        if (isTraning == false)
        {
            if (generationNumber == 0)
            {
                InitZombieNeuralNetworks();
            }
            else
            {
                // Sort the units of the current population by their fitness ranking
                nets.Sort();

                for (int i = 0; i < populationSize; i++)
                {
                    debugValue[i] = nets[i].GetFitness();
                }

                if (nets[nets.Count - 1].GetFitness() > highestScore)
                {
                    highestScore = nets[nets.Count - 1].GetFitness();
                    highestScoreGen = generationNumber;
                }

                //for (int i = 0; i < populationSize / 2; i++)
                //{
                //    nets[i] = new NeuralNetwork(nets[i + (populationSize / 2)]);
                //    nets[i].Mutate();
                //too lazy to write a reset neuron matrix values method....so just going to make a deepcopy lol
                //    nets[i + (populationSize / 2)] = new NeuralNetwork(nets[i + (populationSize / 2)]);
                //}

                for (int i = 0; i < populationSize; i++)
                {
                    // Select the top 20 units (winners) and pass them directly on to the next population
                    if (i >= 30)
                    {
                    }
                    // Create 5 offspring as a crossover product of two best winners
                    else if (i < 30 && i >= 25)
                    {
                        float[] parentA = nets[0].WeigtsToArray();
                        float[] parentB = nets[1].WeigtsToArray();
                        float[] offspring = nets[i].crossOver(parentA, parentB);
                        nets[i].ReadWeights(offspring);
                    }
                    // create 15 offsprings as a crossover products of two random winners
                    else if (i < 25 && i >= 10)
                    {
                        int randomIndex = Random.Range(39, 49);
                        int randomIndex2 = Random.Range(39, 49);
                        float[] parentA = nets[randomIndex].WeigtsToArray();
                        float[] parentB = nets[randomIndex2].WeigtsToArray();
                        float[] offspring = nets[i].crossOver(parentA, parentB);
                        nets[i].ReadWeights(offspring);
                    }
                    // create 10 offsprings as a direct copy of two random winners + random mutate
                    else if (i < 10)
                    {
                        int randomIndex = Random.Range(39, 49);
                        float[] parentA = nets[randomIndex].WeigtsToArray();
                        nets[i].ReadWeights(parentA);
                        nets[i].Mutate();
                    }

                    nets[i].SetFitness(0f);
                }

                // apply random mutations on each offspring to add some variations
            }

            generationNumber++;

            isTraning = true;
            Invoke("Timer", 15f);
            InitZombies();
        }
    }

    private void InitZombies()
    {
        // Clear Zombies
        if (zombieList != null)
        {
            for (int i = 0; i < zombieList.Count; i++)
            {
                GameObject.Destroy(zombieList[i].gameObject);
            }
        }

        zombieList = new List<Zombie_NN>();

        for (int i = 0; i < populationSize; i++)
        {
            //Zombie_NN zombieChaser = ((GameObject)Instantiate(zombiePrefab, new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), 0), zombiePrefab.transform.rotation)).GetComponent<Zombie_NN>();
            Zombie_NN zombieChaser = ((GameObject)Instantiate(zombiePrefab, new Vector3(0, 0, 0), zombiePrefab.transform.rotation)).GetComponent<Zombie_NN>();
            zombieChaser.Init(nets[i], player.transform);
            zombieList.Add(zombieChaser);
        }
    }

    void InitZombieNeuralNetworks()
    {
        //population must be even, just setting it to 20 incase it's not
        if (populationSize % 2 != 0)
        {
            populationSize = 50;
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
