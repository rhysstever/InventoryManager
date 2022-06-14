using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public GameObject slotsCountInput;
    public GameObject controlsDropdown;

    // Start is called before the first frame update
    void Start()
    {
        CreateInventoryUI();
        SetupUIEvents();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Sets up all event listeners for each user controls
    /// </summary>
    private void SetupUIEvents()
    {
        slotsCountInput.GetComponent<TMP_InputField>().onEndEdit.AddListener(delegate {
            InventoryManager.instance.ChangeSlotCount(int.Parse(slotsCountInput.GetComponent<TMP_InputField>().text));
        });

        // Change current control scheme based on the dropdown's value
        controlsDropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate {
            InventoryManager.instance.ChangeControlScheme(controlsDropdown.GetComponent<TMP_Dropdown>().value);
        });
    }

    /// <summary>
    /// Create the grid of inventory slots
    /// </summary>
    private void CreateInventoryUI()
	{
        // Deactivate all current children
        for(int i = inventoryPanel.transform.childCount - 1; i >= 0; i--)
            inventoryPanel.transform.GetChild(i).parent = null;

        // Get the number of columns and set the inital position and offsets
        int columns = InventoryManager.instance.GetColumnsCount();

        Vector3 initialPos = new Vector3(-1330.0f, -350.0f, 0.0f);
        float offset = 125.0f;

        // Create each inventory slot UI object
        for(int i = 0; i < InventoryManager.instance.GetSlotsCount(); i++)
		{
            // Calculate the element's position, with offsets
            Vector3 slotPos = initialPos;
            slotPos.x += offset * (i % columns);
            slotPos.y += -offset * (i / columns);

            // Create the element object and set its position
            GameObject itemSlotObject = Instantiate(itemSlotPrefab, Vector3.zero, Quaternion.identity, inventoryPanel.transform);
            itemSlotObject.transform.localPosition = slotPos;
        }

        // Pair each created UI element with its graph node
        InventoryManager.instance.PairUIElementsToGraph(inventoryPanel);
	}

    /// <summary>
    /// Updates the canvas when the slot count input field value is changed
    /// </summary>
    /// <param name="slotCountText">The string text of the input field</param>
    public void UpdateSlotCountUI(string slotCountText)
	{
        // Update the input field in case the value was clamped
        SetSlotCountInputFieldText(slotCountText.ToString());

        // Recreate the inventory UI
        CreateInventoryUI();
    }

    /// <summary>
    /// Changes the text of the slot count input field
    /// </summary>
    /// <param name="newText">The new text of the input field</param>
    private void SetSlotCountInputFieldText(string newText) { slotsCountInput.GetComponent<TMP_InputField>().text = newText; }
}
