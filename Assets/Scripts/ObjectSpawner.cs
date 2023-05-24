using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectSpawner : MonoBehaviour
{
    public Vector3 GETRandomPosition()
    {
        var coll = GetComponent<Collider>();

        if (coll == null)
        {
            Debug.LogError("ObjectSpawner requires a Collider component");
            throw new Exception();
        }

        Random.InitState(DateTime.Now.Millisecond);
        var bounds = coll.bounds;
        var sizeX = bounds.size.x / 2;
        var sizeZ = bounds.size.z / 2;
        Vector3 pos = transform.position + new Vector3(Random.Range(-sizeX, sizeX), .4f, Random.Range(-sizeZ, sizeZ));
        return pos;
    }
}