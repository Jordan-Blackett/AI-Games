using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterManager : MonoBehaviour {

    public GameObject fighterPrefab;
    public GameObject player;

    private int[] layers = new int[] { 8, 5, 5, 4 }; //1 input and 1 output
    private List<NeuralNetwork> nets;
    private int populationSize = 50;
    public int generationNumber = 0;
    private bool isTraning = false;

    private List<Fighter_NN> fighterList = null;

    // UI
    public float highestScore = 0;
    public float highestScoreGen = 0;

    void Timer()
    {
        isTraning = false;
    }

    void Update()
    {
        if (isTraning == false)
        {
            if (generationNumber == 0)
            {
                InitFighterNeuralNetworks();
            }
            else
            {
                // Sort the units of the current population by their fitness ranking
                nets.Sort();

                //if (nets[nets.Count - 1].GetFitness() > highestScore)
                //{
                //    highestScore = nets[nets.Count - 1].GetFitness();
                //    highestScoreGen = generationNumber;
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

                // TODO: apply random mutations on each offspring to add some variations
            }

            generationNumber++;

            isTraning = true;
            Invoke("Timer", 5f);
            InitFighters();
        }
    }

    private void InitTrainingLevel()
    {

    }

    private void InitFighters()
    {
        // Clear Fighters
        if (fighterList != null)
        {
            for (int i = 0; i < fighterList.Count; i++)
            {
                GameObject.Destroy(fighterList[i].gameObject);
            }
        }

        fighterList = new List<Fighter_NN>();

        for (int i = 0; i < populationSize; i++)
        {
            Fighter_NN fighter = ((GameObject)Instantiate(fighterPrefab, new Vector3(0, 0, 0), fighterPrefab.transform.rotation)).GetComponent<Fighter_NN>();
            fighter.Init(nets[i], player.transform);
            fighterList.Add(fighter);
        }
    }

    void InitFighterNeuralNetworks()
    {
        nets = new List<NeuralNetwork>();

        for (int i = 0; i < populationSize; i++)
        {
            NeuralNetwork net = new NeuralNetwork(layers);
            nets.Add(net);
        }
    }
}
