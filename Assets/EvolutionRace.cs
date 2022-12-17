using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionRace : MonoBehaviour
{
    public int nCars;

    [Range(1, 20)]
    public float speed = 10;

    private List<Car> cars;

    private bool inSimulation;
    private int stepCounter;

    List<Collider> walls;

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

        InitWalls();
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
        }
    }

    private void Simulate()
    {
        inSimulation = false;

        foreach (Car car in cars)
        {
            if (car.HasHitTrack(walls)) continue;
            inSimulation = true;

            car.TakeStep(stepCounter, Time.deltaTime * speed);
        }
        stepCounter++;
    }

    private void Evolution()
    {
        CalcFitness();
        SortPopulation();
        NewMoves();
        ResetCars();

        inSimulation = true;
        stepCounter = 1;
    }

    private void CalcFitness()
    {
        // TODO
        foreach (Car car in cars)
        {
            car.CalcFitness();
        }
    }

    private void SortPopulation()
    {
        // TODO
    }

    private void NewMoves()
    {
        // TODO
    }

    private void ResetCars()
    {
        foreach (Car car in cars)
        {
            car.ResetPosition(transform);
        }
    }
}
