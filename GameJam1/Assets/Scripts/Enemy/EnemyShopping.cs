using System.Collections.Generic;
using UnityEngine;

public class EnemyShopping : MonoBehaviour
{
    [SerializeField] private List<ShoppingItem> shoppingList;
    [SerializeField] private ShelfRegistry shelfRegistry;
    [SerializeField] private CartInventory cartInventory;
    private EnemyMovement movement;

    private int currentItemIndex;

    private void Awake()
    {
        movement = GetComponent<EnemyMovement>();
    }

    private void OnEnable()
    {
        movement.OnDestinationReached += HandleDestinationReached;
    }

    private void OnDisable()
    {
        movement.OnDestinationReached -= HandleDestinationReached;
    }

    private void Start()
    {
        GoToCurrentProduct();
    }

    private void GoToCurrentProduct()
    {
        if (currentItemIndex >= shoppingList.Count)
        {
            Debug.Log("Shopping Complete");
            return;
        }

        ShoppingItem currentItem = shoppingList[currentItemIndex];

        Shelf targetShelf = shelfRegistry.GetShelf(currentItem.productID);

        if (targetShelf == null)
        {
            Debug.LogWarning($"No shelf found for {currentItem.productID}");
            return;
        }

        movement.MoveTo(targetShelf.InteractionPoint.position);
    }

    private void HandleDestinationReached()
    {
        if (currentItemIndex >= shoppingList.Count)
            return;

        ShoppingItem currentItem = shoppingList[currentItemIndex];

        Shelf targetShelf = shelfRegistry.GetShelf(currentItem.productID);

        if (targetShelf == null)
            return;

        int amountNeeded = currentItem.requiredAmount;

        for (int i = 0; i < amountNeeded; i++)
        {
            GameObject product = targetShelf.Stock.TryTakeProduct();

            if (product != null)
            {
                cartInventory.AddProduct(
                    currentItem.productID,
                    product
                );
            }
            else
            {
                Debug.Log($"Shelf is out of {currentItem.productID}");
                break;
            }
        }

        currentItemIndex++;

        GoToCurrentProduct();
    }
}