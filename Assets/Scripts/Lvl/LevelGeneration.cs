using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    public GameObject[] walls;

    public bool randomizeRotation;

    [SerializeField] private int chanceAmount;
    [SerializeField] private bool chance;

    private void Awake()
    {
        if (chance && GameManager.instance.Chance(0, chanceAmount) != 1)
            return;

        transform.position = RoundVector(transform.position);
        int _rand = Random.Range(0, walls.Length);
        if (randomizeRotation)
        {
            float rotation = RandomizeRotation();
            transform.eulerAngles = new Vector3(0, 0, rotation);
            Instantiate(walls[_rand], transform.position, Quaternion.Euler(0,0,rotation));
            return;
        }

        Instantiate(walls[_rand], transform.position, Quaternion.identity, transform);
    }

    private float RandomizeRotation()
    {
        int _chance = Random.Range(0, 2);

        float _rotation = 0;

        if (_chance == 1)
            _rotation = 0;
        else
            _rotation = 90;

        return Mathf.Round(_rotation);
    }

    private Vector3 RoundVector(Vector3 vector)
    {
        float x = Mathf.Round(vector.x);
        float y = Mathf.Round(vector.y);
        float z = Mathf.Round(vector.z);

        vector = new Vector3(x, y, z);
        return vector;
    }
}
