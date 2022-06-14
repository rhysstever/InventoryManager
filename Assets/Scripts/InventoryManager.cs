using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Action
{
    Up,
    Right,
    Down,
    Left,
    Select,
    None
}

public enum ControlScheme
{
    Any,
    WASD,
    NumPad
}

public class InventoryManager : MonoBehaviour
{
    #region Singleton Code
    // A public reference to this script
    public static InventoryManager instance = null;

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

    // Set in inspector
    private ControlScheme currentControlScheme;

    // Set at Start()
    public GraphNode<GameObject> currentlyHoveredItem;
    private int columns;
    private int numOfSlots;
    private GraphNode<GameObject>[] itemSlots;
    private Dictionary<ControlScheme, Dictionary<KeyCode, Action>> inputDictionary;

    // Start is called before the first frame update
    void Start()
    {
        CreateInputDictionary();
        currentlyHoveredItem = null;
        columns = 7;

        ChangeSlotCount(28);
    }

    // Update is called once per frame
    void Update()
    {
        // Check for any input
        CheckForInput(currentControlScheme);

        // Closes the app when 'ESC' is pressed
        if(Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    /// <summary>
    /// Gets the number of columns
    /// </summary>
    /// <returns>The number of columns</returns>
    public int GetColumnsCount() { return columns; }

    /// <summary>
    /// Gets the number of inventory slots
    /// </summary>
    /// <returns>The number of inventory slots</returns>
    public int GetSlotsCount() { return itemSlots.Length; }

    /// <summary>
    /// Change the control scheme
    /// </summary>
    /// <param name="newControlSchemeIndex">The index of the control scheme (based on the enum)</param>
    public void ChangeControlScheme(int newControlSchemeIndex) { currentControlScheme = (ControlScheme)newControlSchemeIndex; }

    /// <summary>
    /// Change the number of inventory slots
    /// </summary>
    /// <param name="newSlotCount">The new amount of inventory slots</param>
    public void ChangeSlotCount(int newSlotCount)
	{
        // Clamp the value and recreate the graph
        numOfSlots = Mathf.Clamp(newSlotCount, 1, 28);
        itemSlots = CreateInventoryGraphArray(numOfSlots, columns);

        // Update UI
        UIManager.instance.UpdateSlotCountUI(numOfSlots.ToString());
    }

    /// <summary>
    /// Pairs each UI slot object to the corresponding graph node
    /// </summary>
    /// <param name="parentUIElements">The parent object of all item slot objects</param>
    public void PairUIElementsToGraph(GameObject parentUIElements)
    {
        for(int i = 0; i < parentUIElements.transform.childCount; i++)
            parentUIElements.transform.GetChild(i).GetComponent<ItemSlot>().itemSlotNode = itemSlots[i];
    }

    /// <summary>
    /// Create each dictionary for each direction within a larger dictionary for each control scheme
    /// </summary>
    private void CreateInputDictionary()
	{
        inputDictionary = new Dictionary<ControlScheme, Dictionary<KeyCode, Action>>();

        Dictionary<KeyCode, Action> wasdInputs = new Dictionary<KeyCode, Action>();
        wasdInputs.Add(KeyCode.W, Action.Up);
        wasdInputs.Add(KeyCode.A, Action.Left);
        wasdInputs.Add(KeyCode.S, Action.Down);
        wasdInputs.Add(KeyCode.D, Action.Right);
        wasdInputs.Add(KeyCode.E, Action.Select);

        inputDictionary.Add(ControlScheme.WASD, wasdInputs);

        Dictionary<KeyCode, Action> numPadInputs = new Dictionary<KeyCode, Action>();
        numPadInputs.Add(KeyCode.Keypad8, Action.Up);
        numPadInputs.Add(KeyCode.Keypad4, Action.Left);
        numPadInputs.Add(KeyCode.Keypad5, Action.Down);
        numPadInputs.Add(KeyCode.Keypad6, Action.Right);
        numPadInputs.Add(KeyCode.Keypad9, Action.Select);

        inputDictionary.Add(ControlScheme.NumPad, numPadInputs);
    }

    /// <summary>
    /// Create inventory slots into a graphic structure
    /// </summary>
    /// <param name="numOfSlots">The total number of slots in the inventory</param>
    /// <param name="numOfColumns">The number of columns in the inventory</param>
    /// <returns>An array of graph nodes that represent the inventory</returns>
    private GraphNode<GameObject>[] CreateInventoryGraphArray(int numOfSlots, int numOfColumns)
	{
        // Create the array based on the number of slots
        GraphNode<GameObject>[] itemSlots = new GraphNode<GameObject>[numOfSlots];

        // Create and add slot objects to list
        for(int i = 0; i < numOfSlots; i++)
            itemSlots[i] = new GraphNode<GameObject>(null);

        // Dynamically add neighbors
        for(int i = 0; i < numOfSlots; i++)
        {
            // Has an above neighbor if the slot is not in the first row
            if(i / numOfColumns > 0)
                itemSlots[i].SetNeighbor(Action.Up, itemSlots[i - columns]);

            // Has a right neighbor if the slot is not in the last column
            // AND is not the last slot (edge case if numOfSlots / numOfColumns != 0)
            if(i % numOfColumns < numOfColumns - 1 && i + 1 < numOfSlots)
                itemSlots[i].SetNeighbor(Action.Right, itemSlots[i + 1]);

            // Has a below neighbor if the slot is not in the last row
            if(i + numOfColumns < numOfSlots)
                itemSlots[i].SetNeighbor(Action.Down, itemSlots[i + columns]);

            // Has a left neighbor if the slot is not in the first column
            if(i % numOfColumns > 0)
                itemSlots[i].SetNeighbor(Action.Left, itemSlots[i - 1]);
        }

        // Hover over the first slot
        SetNewHoveredItem(itemSlots[0]);
        
        // Return the array of GraphNodes
        return itemSlots;
    }

    /// <summary>
    /// A simpler overall method to be called to handle all input
    /// </summary>
    /// <param name="controlScheme">The control scheme being used for user input</param>
    private void CheckForInput(ControlScheme controlScheme)
	{
        InterpretAction(GetActionFromInput(controlScheme));
	}

    /// <summary>
    /// Gives an action based on user input
    /// </summary>
    /// <param name="controlScheme">The control scheme being used</param>
    /// <returns>The corresponding action</returns>
    private Action GetActionFromInput(ControlScheme controlScheme)
    {
        // If the control scheme is "Any", 
        // ALL control schemes need to be checked
        if(controlScheme == ControlScheme.Any)
        {
            // loop thr each control scheme
            foreach(ControlScheme ctrlscheme in inputDictionary.Keys)
                // loop thr each key in the control scheme
                foreach(KeyCode key in inputDictionary[ctrlscheme].Keys)
                    if(Input.GetKeyDown(key))
                        return inputDictionary[ctrlscheme][key];
        }
        else
        {
            // Check every key in the control scheme,
            // if a key is down, return the corresponding direction
            foreach(KeyCode key in inputDictionary[controlScheme].Keys)
                if(Input.GetKeyDown(key))
                    return inputDictionary[controlScheme][key];
        }

        // Otherwise, return no direction
        return Action.None;
    }

    /// <summary>
    /// Interprets input based on the particular action
    /// </summary>
    /// <param name="action">The action being performed</param>
    private void InterpretAction(Action action)
	{
        // Ensure there is an actual action
        if(action == Action.None)
            return;
        // If the item is being selected, select it
        else if(action == Action.Select)
            SelectItem(currentlyHoveredItem);
        // Otherwise move the hovered slot
        else
            MoveHoveredSlot(action);
    }

    /// <summary>
    /// Hovers a new slot based on the given direction
    /// </summary>
    /// <param name="direction">The direction to go towards the new hovered slot</param>
    private void MoveHoveredSlot(Action direction)
	{
        if(currentlyHoveredItem.GetNeighbor(direction) != null)
            SetNewHoveredItem(currentlyHoveredItem.GetNeighbor(direction));
    }

    /// <summary>
    /// Changes which item is being hovered over
    /// </summary>
    /// <param name="hoveredItem">The newly hovered item</param>
    private void SetNewHoveredItem(GraphNode<GameObject> hoveredItem)
	{
        // Unhover the soon-to-be previously hovered item
        if(currentlyHoveredItem != null)
            currentlyHoveredItem.Hovered = false;

        // Set the new hovered item
        hoveredItem.Hovered = true;
        currentlyHoveredItem = hoveredItem;
    }

    /// <summary>
    /// Selects the given item
    /// </summary>
    /// <param name="selectedItem">The item to select</param>
    private void SelectItem(GraphNode<GameObject> selectedItem)
	{
        if(selectedItem.value != null)
            Debug.Log(selectedItem.value.name + " has been selected");
        else
            Debug.Log("There is no item to select");
    }
}
