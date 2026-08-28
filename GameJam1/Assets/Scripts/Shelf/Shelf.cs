using UnityEngine;

public class Shelf : MonoBehaviour
{
    [SerializeField] private ProductID productID;
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private ShelfStock stock;

    public ProductID ProductID => productID;
    public Transform InteractionPoint => interactionPoint;
    public ShelfStock Stock => stock;
}