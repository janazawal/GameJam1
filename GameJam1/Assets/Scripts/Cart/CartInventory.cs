using System.Collections.Generic;
using UnityEngine;

public class CartInventory : MonoBehaviour
{
    [SerializeField] private Transform productContainer;

    private Dictionary<ProductID, int> products =
        new Dictionary<ProductID, int>();
    private void MoveProductToCart(GameObject productObject)
    {
        if (productObject == null)
            return;

        Rigidbody rb = productObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        Collider col = productObject.GetComponent<Collider>();

        if (col != null)
        {
            col.enabled = false;
        }

        productObject.transform.SetParent(productContainer, false);

        productObject.transform.localPosition = Vector3.zero;
        productObject.transform.localRotation = Quaternion.identity;
    }
    public void AddProduct(ProductID productID, GameObject productObject)
    {
        if (products.ContainsKey(productID))
        {
            products[productID]++;
        }
        else
        {
            products.Add(productID, 1);
        }

        MoveProductToCart(productObject);

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
}