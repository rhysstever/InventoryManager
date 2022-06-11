using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region Singleton Code
    // A public reference to this script
    public static UIManager instance = null;

    // Awake is called even before start 
    // (I think its at the very beginning of runtime)
    private void Awake()
    {
        // If the reference for this script is null, assign it this script
        if(instance == null)
            instance = this;
        // If the reference is to something else (it already exists)
        // than this is not needed, thus destroy it
        else if(instance != this)
            Destroy(gameObject);
    }
    #endregion

    public GameObject canvas;
    public GameObject inventoryPanel;
    public GameObject itemSlotPrefab;

    // Start is called before the first frame update
    void Start()
    {
        CreateInventoryUI();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Create the grid of inventory slots
    /// </summary>
    private void CreateInventoryUI()
	{
        int columns = InventoryManager.instance.GetColumnsCount();

        Vector3 initialPos = new Vector3(-1300.0f, -400.0f, 0.0f);
        float columnOffset = 125.0f;
        float rowOffset = -150.0f;

        for(int i = 0; i < InventoryManager.instance.GetSlotsCount(); i++)
		{
            // Calculate the element's position, with offsets
            Vector3 slotPos = initialPos;
            slotPos.x += columnOffset * (i % columns);
            slotPos.y += rowOffset * (i / columns);

            // Create the element object and set its position
            GameObject itemSlotObject = Instantiate(itemSlotPrefab, Vector3.zero, Quaternion.identity, inventoryPanel.transform);
            itemSlotObject.transform.localPosition = slotPos;
        }

        // Pair each created UI element with its graph node
        InventoryManager.instance.PairUIElementsToGraph(inventoryPanel);
	}
}
