using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car
{
    public List<Move> moves;

    public float fitness;

    private GameObject model;

    private bool crashed;

    public Car(Transform start)
    {
        moves = new List<Move>();

        fitness = 0;

        model = GameObject.CreatePrimitive(PrimitiveType.Cube);
        model.transform.position = start.position;
        model.transform.rotation = start.rotation;

        model.name = "Car " + model.GetInstanceID();

        crashed = false;
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
        if (sign < 0) {
            angle = 360 - angle;
        }
        return angle;
    }

    public void ResetPosition(Transform start)
    {
        model.transform.position = start.position;
        model.transform.rotation = start.rotation;

        crashed = false;
    }

    private void Forward(float delta)
    {
        model.transform.position += model.transform.forward * delta;
    }

    private void Left(float delta)
    {
        model.transform.RotateAround(model.transform.position, Vector3.up, 15 * delta);
    }

    private void Right(float delta)
    {
        model.transform.RotateAround(model.transform.position, Vector3.up, -15 * delta);
    }

    public bool HasHitTrack(List<Collider> walls)
    {
        if (crashed) return true;

        foreach (Collider wall in walls)
        {
            if (model.GetComponent<Collider>().bounds.Intersects(wall.bounds))
            {
                crashed = true;
                return true;
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
