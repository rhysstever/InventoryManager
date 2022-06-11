using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    public GraphNode<GameObject> itemSlotNode;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(itemSlotNode != null)
            Select(itemSlotNode.Selected);
    }

    /// <summary>
    /// Select (or deselect) the slot's UI
    /// </summary>
    /// <param name="isSelected">Whether the slot is being selected or deselected</param>
    public void Select(bool isSelected) { transform.GetChild(0).GetChild(1).gameObject.SetActive(isSelected); }
}
