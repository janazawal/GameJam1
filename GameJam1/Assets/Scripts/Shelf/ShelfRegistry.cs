using System.Collections.Generic;
using UnityEngine;

public class ShelfRegistry : MonoBehaviour
{
    private List<Shelf> shelves = new List<Shelf>();

    private void Awake()
    {
        shelves.AddRange(FindObjectsByType<Shelf>(FindObjectsSortMode.None));
    }

    public Shelf GetShelf(ProductID productID)
    {
        foreach (Shelf shelf in shelves)
        {
            if (shelf.ProductID == productID)
            {
                return shelf;
            }
        }

        return null;
    }
}