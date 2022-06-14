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
            Hover(itemSlotNode.Hovered);

        // Destroys the gameObject if there is no parent
        if(transform.parent == null)
            DestroyImmediate(this.gameObject);
    }

    /// <summary>
    /// Hover (or unhover) the slot's UI
    /// </summary>
    /// <param name="isHovered">Whether the slot is being hovered or unhovered</param>
    public void Hover(bool isHovered) { transform.GetChild(0).GetChild(1).gameObject.SetActive(isHovered); }
}
