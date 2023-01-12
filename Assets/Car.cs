using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car
{
    public List<Move> moves;

    public float fitness;

    public GameObject model;

    public CrashType crashed;

    public Renderer renderer;

    public Collider collider;

    public enum CrashType
    {
        notCrashed,
        crashed,
        crashedRaceTarget
    }

    public Car(Transform start)
    {
        moves = new List<Move>();

        fitness = 0;

        model = GameObject.CreatePrimitive(PrimitiveType.Cube);
        model.transform.position = start.position;
        model.transform.rotation = start.rotation;

        model.name = "Car " + model.GetInstanceID();

        renderer = model.GetComponent<Renderer>();
        collider = model.GetComponent<Collider>();

        crashed = CrashType.notCrashed;
    }

    public void TakeStep(int step, float delta)
    {
        if (step > moves.Count)
        {
            moves.Add(RandomMove());
        }

        Move move = moves[step - 1];

        switch (move)
        {
            case Move.FORWRD: Forward(delta); break;
            case Move.LEFT: Left(delta); break;
            case Move.RIGHT: Right(delta); break;
        }
    }

    public void CalcFitness(GameObject center, GameObject start)
    {
        // TODO
        fitness = AngleAroundCenter(center, start);
    }

    private float AngleAroundCenter(GameObject center, GameObject start)
    {
        Vector3 from = start.transform.position - center.transform.position;
        Vector3 to = model.transform.position - center.transform.position;

        float angle = Vector3.Angle(from, to);
        float sign = Mathf.Sign(Vector3.Dot(center.transform.up, Vector3.Cross(from, to)));
        if (sign < 0)
        {
            angle = 360 - angle;
        }
        return angle;
    }

    public void ResetPosition(Transform start)
    {
        model.transform.position = start.position + start.forward * 2;
        model.transform.rotation = start.rotation;

        renderer.material.color = Color.white;

        crashed = CrashType.notCrashed;
    }

    private void Forward(float delta)
    {
        model.transform.position += model.transform.forward * delta;
    }

    private void Left(float delta)
    {
        model.transform.RotateAround(model.transform.position, Vector3.up, 15 * delta);
        Forward(delta);
    }

    private void Right(float delta)
    {
        model.transform.RotateAround(model.transform.position, Vector3.up, -15 * delta);
        Forward(delta);
    }

    public CrashType HasHitTrack(List<Collider> walls)
    {
        if (crashed != Car.CrashType.notCrashed) return crashed;

        foreach (Collider wall in walls)
        {
            if (collider.bounds.Intersects(wall.bounds))
            {
                if (wall.name == "RaceEnd")
                {
                    crashed = CrashType.crashedRaceTarget;
                    renderer.material.color = Color.blue;
                }
                else
                {
                    crashed = CrashType.crashed;
                    renderer.material.color = Color.red;
                }
                return crashed;
            }
        }

        return crashed;
    }

    public static Move RandomMove()
    {
        int move = Random.Range(0, 3);
        switch (move)
        {
            case 0: return Move.FORWRD;
            case 1: return Move.LEFT;
            case 2: return Move.RIGHT;
        }

        return Move.FORWRD;
    }

    public void SetMoves(List<Move> moves)
    {
        this.moves = moves;
    }
}
