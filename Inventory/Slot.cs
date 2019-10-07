using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField]
    private SlotCategories category;
    public SlotCategories Category { get { return category; } } 
    
    public Item Item;
    
    public Inventory Inventory;
}