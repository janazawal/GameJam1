using System.Collections.Generic;
using UnityEngine;

public class CartInventory : MonoBehaviour
{
    [SerializeField] private Transform productContainer;

    private Dictionary<ProductID, int> products =
        new Dictionary<ProductID, int>();

    public void AddProduct(ProductID productID, GameObject productObject)
    {
        if (productObject == null)
        {
            Debug.LogError("Product object is NULL");
            return;
        }

        if (productContainer == null)
        {
            Debug.LogError("ProductContainer is NOT assigned");
            return;
        }

        if (products.ContainsKey(productID))
            products[productID]++;
        else
            products.Add(productID, 1);

        MoveProductToContainer(productObject);

        Debug.Log(
            $"Added {productObject.name} to {productContainer.name}"
        );
    }

    public int GetAmount(ProductID productID)
    {
        if (products.TryGetValue(productID, out int amount))
            return amount;

        return 0;
    }

    private void MoveProductToContainer(GameObject productObject)
    {
        Rigidbody rb =
            productObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        Collider col =
            productObject.GetComponent<Collider>();

        if (col != null)
            col.enabled = false;

        productObject.transform.SetParent(
            productContainer
        );

        productObject.transform.localPosition =
            Vector3.zero;

        productObject.transform.localRotation =
            Quaternion.identity;

        Debug.Log(
            "Product parent is now: "
            + productObject.transform.parent.name
        );
    }
}