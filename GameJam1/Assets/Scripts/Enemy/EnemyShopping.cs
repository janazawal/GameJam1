using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShopping : MonoBehaviour
{
    [SerializeField] private List<ShoppingItem> shoppingList;
    [SerializeField] private ShelfRegistry shelfRegistry;
    [SerializeField] private CartInventory cartInventory;
    [SerializeField] private float collectDelay = 1f;

    private EnemyMovement movement;

    private int currentItemIndex;
    private int collectedFromCurrentItem;

    private bool isPaused;
    private bool isCollecting;
    private bool shoppingComplete;

    private Coroutine collectRoutine;

    public bool IsShoppingComplete => shoppingComplete;

    public event Action OnShoppingCompleted;

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

    public void Initialize(
       List<ShoppingItem> newShoppingList,
       ShelfRegistry registry)
    {
        shoppingList =
            new List<ShoppingItem>(newShoppingList);

        shelfRegistry = registry;

        currentItemIndex = 0;
        collectedFromCurrentItem = 0;
        shoppingComplete = false;
        isPaused = false;

        GoToCurrentProduct();
    }
    public void PauseShopping()
    {
        if (shoppingComplete)
            return;

        isPaused = true;

        if (collectRoutine != null)
        {
            StopCoroutine(collectRoutine);
            collectRoutine = null;
        }

        isCollecting = false;

        movement.Stop();
    }

    public void ResumeShopping()
    {
        if (shoppingComplete)
            return;

        isPaused = false;

        GoToCurrentProduct();
    }

    private void GoToCurrentProduct()
    {
        if (isPaused)
            return;

        if (currentItemIndex >= shoppingList.Count)
        {
            CompleteShopping();
            return;
        }

        ShoppingItem currentItem =
            shoppingList[currentItemIndex];

        Shelf targetShelf =
            shelfRegistry.GetShelf(currentItem.productID);

        if (targetShelf == null)
        {
            Debug.LogWarning(
                $"No shelf found for {currentItem.productID}"
            );

            return;
        }

        movement.MoveTo(
            targetShelf.InteractionPoint.position
        );
    }

    private void HandleDestinationReached()
    {
        if (isPaused)
            return;

        if (shoppingComplete)
            return;

        if (isCollecting)
            return;

        collectRoutine =
            StartCoroutine(CollectCurrentProduct());
    }

    private IEnumerator CollectCurrentProduct()
    {
        isCollecting = true;

        ShoppingItem currentItem =
            shoppingList[currentItemIndex];

        Shelf targetShelf =
            shelfRegistry.GetShelf(currentItem.productID);

        if (targetShelf == null)
        {
            isCollecting = false;
            collectRoutine = null;

            yield break;
        }

        while (
            collectedFromCurrentItem <
            currentItem.requiredAmount
        )
        {
            if (isPaused)
            {
                isCollecting = false;
                collectRoutine = null;

                yield break;
            }

            GameObject product =
                targetShelf.Stock.TryTakeProduct();

            if (product == null)
            {
                Debug.LogWarning(
                    $"No more {currentItem.productID}"
                );

                break;
            }

            cartInventory.AddProduct(
                currentItem.productID,
                product
            );

            collectedFromCurrentItem++;

            Debug.Log(
                $"Collected {currentItem.productID} " +
                $"{collectedFromCurrentItem}/" +
                $"{currentItem.requiredAmount}"
            );

            yield return new WaitForSeconds(
                collectDelay
            );
        }

        collectedFromCurrentItem = 0;

        currentItemIndex++;

        isCollecting = false;
        collectRoutine = null;

        GoToCurrentProduct();
    }

    private void CompleteShopping()
    {
        if (shoppingComplete)
            return;

        shoppingComplete = true;

        movement.Stop();

        Debug.Log("Shopping Complete");

        OnShoppingCompleted?.Invoke();
    }
}