using System.Collections.Generic;
using UnityEngine;

public class ShelfStock : MonoBehaviour
{
    [SerializeField] private List<GameObject> productVisuals;

    public int CurrentAmount => productVisuals.Count;

    public GameObject TryTakeProduct()
    {
        if (productVisuals.Count == 0)
            return null;

        int lastIndex = productVisuals.Count - 1;

        GameObject product = productVisuals[lastIndex];
        productVisuals.RemoveAt(lastIndex);

        return product;
    }
}