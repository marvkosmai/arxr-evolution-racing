using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class EvolutionRace : MonoBehaviour
{
    public int nCars;

    [Range(1, 30)]
    public float speed = 10;

    [Range(0f, 1f)]
    public float elitsPercent = 0.2f;

    [Range(0f, 1f)]
    public float mutationRate = 0.1f;

    private List<Car> cars;

    private bool inSimulation;
    private int stepCounter;

    public int population;

    private List<Collider> walls;

    private GameObject center;

    public GameObject txt_val_Iteration;
    private int iteration = 1;

    // Start is called before the first frame update
    void Start()
    {
        cars = new List<Car>();

        for (int i = 0; i < nCars; i++)
        {
            cars.Add(new Car(transform));
        }

        inSimulation = true;
        stepCounter = 1;

        population = 1;

        InitWalls();

        center = GameObject.FindGameObjectWithTag("Center");
    }

    private void InitWalls()
    {
        GameObject track = GameObject.FindGameObjectWithTag("Track");
        walls = new List<Collider>();
        foreach (Transform child in track.transform)
        {
            walls.Add(child.gameObject.GetComponent<Collider>());
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (inSimulation)
        {
            Simulate();
        }

        if (!inSimulation)
        {
            Evolution();
            txt_val_Iteration.GetComponent<TextMeshProUGUI>().text = ++iteration + "";
        }

    }

    private void Simulate()
    {
        inSimulation = false;

        var maxFitnessCar = cars[0];
        foreach (Car car in cars)
        {
            if (car.HasHitTrack(walls) != Car.CrashType.notCrashed) continue;
            inSimulation = true;

            car.TakeStep(stepCounter, Time.deltaTime * speed);
            car.CalcFitness(center, transform.gameObject);
            car.model.GetComponent<Renderer>().material.color = Color.white;

            if (car.fitness > maxFitnessCar.fitness){
                maxFitnessCar = car;
            }
        }
        maxFitnessCar.model.GetComponent<Renderer>().material.color = Color.green;
        stepCounter++;

        if(!inSimulation && cars.Any(x => x.crashed == Car.CrashType.crashedRaceTarget))
            inSimulation = true;
    }

    private void Evolution()
    {
        CalcFitness();
        SortPopulation();
        NewMoves();
        ResetCars();

        inSimulation = true;
        stepCounter = 1;
        population++;
    }

    private void CalcFitness()
    {
        foreach (Car car in cars)
        {
            car.CalcFitness(center, transform.gameObject);
        }
    }

    private void SortPopulation()
    {
        cars.Sort((Car a, Car b) => b.fitness.CompareTo(a.fitness));
    }

    private void NewMoves()
    {
        List<List<Move>> newPopulation = new List<List<Move>>();

        int elits = (int) (nCars * elitsPercent);
        for (int i = 0; i < elits; i++)
        {
            newPopulation.Add(cars[i].moves);
        }  

        for (int i = elits; i < nCars; i++)
        {
            newPopulation.Add(Mutate(ChildMoveset()));
        }

        for (int i = 0; i < nCars; i++)
        {
            cars[i].SetMoves(newPopulation[i]);
        }
    }

    private List<Move> ChildMoveset()
    {
        return CombineMoves(TournamentParent(), TournamentParent());
    }

    private List<Move> Mutate(List<Move> moveset)
    {
        float ageFactor = 0.1f;
        for (int i = 0; i < moveset.Count; i++)
        {
            float ageBasedProb = 1f - (i * ageFactor / moveset.Count);
            if (mutationRate * ageBasedProb < Random.Range(0f, 1f)) continue;

            moveset[i] = Car.RandomMove();
        }

        return moveset;
    }

    private Car TournamentParent()
    {
        Car p1 = cars[Random.Range(0, cars.Count)];
        Car p2 = cars[Random.Range(0, cars.Count)];

        return p1.fitness > p2.fitness ? p1 : p2;
    }

    private List<Move> CombineMoves(Car p1, Car p2)
    {
        List<Move> newMoves = new List<Move>();

        int minCount = Mathf.Min(p1.moves.Count, p2.moves.Count);
        int maxCount = Mathf.Max(p1.moves.Count, p2.moves.Count);

        int point = Random.Range(0, minCount);

        bool switchParent = p1.moves.Count > p2.moves.Count;

        for (int i = 0; i < maxCount; i++)
        {
            if (i < point)
            {
                newMoves.Add(!switchParent ? p1.moves[i] : p2.moves[i]);
            } else
            {
                newMoves.Add(switchParent ? p1.moves[i] : p2.moves[i]);
            }
        }

        return newMoves;
    }

    private void ResetCars()
    {
        foreach (Car car in cars)
        {
            car.ResetPosition(transform);
        }
    }
}
