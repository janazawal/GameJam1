using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemySpawnData
{
    public Transform spawnPoint;

    public List<ShoppingItem> shoppingList =
        new List<ShoppingItem>();
}