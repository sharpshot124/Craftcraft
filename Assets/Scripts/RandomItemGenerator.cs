using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomItemGenerator : ItemGenerator
{
    public float spawnInterval;
    public float intervalVariance;
    public bool isSpawning = true;
    public List<ItemSO> Items;

    private float nextSpawnTime;

    public void Update()
    {
        if (isSpawning && Time.time >= nextSpawnTime)
        {
            nextSpawnTime = Time.time + spawnInterval + ((Random.value - .5f) * 2 * intervalVariance);

            Generate(Items[Random.Range(0, Items.Count)]);
        }
    }
}