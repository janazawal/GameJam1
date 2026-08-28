using System.Collections.Generic;
using UnityEngine;

public class CartInventory : MonoBehaviour
{
    private Dictionary<ProductID, int> products = new Dictionary<ProductID, int>();

    public void AddProduct(ProductID productID, int amount = 1)
    {
        if (products.ContainsKey(productID))
        {
            products[productID] += amount;
        }
        else
        {
            products.Add(productID, amount);
        }

        Debug.Log($"{productID} in cart: {products[productID]}");
    }

    public int GetProductAmount(ProductID productID)
    {
        if (products.TryGetValue(productID, out int amount))
        {
            return amount;
        }

        return 0;
    }

    public void RemoveProduct(ProductID productID, int amount = 1)
    {
        if (!products.ContainsKey(productID))
            return;

        products[productID] -= amount;

        if (products[productID] <= 0)
        {
            products.Remove(productID);
        }
    }
}