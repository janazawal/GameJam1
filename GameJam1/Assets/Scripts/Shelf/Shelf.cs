using Unity.VisualScripting;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    [SerializeField] private ProductID productID;
    [SerializeField] private Transform interactionPoint;

    public ProductID ProductID => productID;
    public Transform InteractionPoint => interactionPoint;
}