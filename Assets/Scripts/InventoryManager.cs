using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Up,
    Right,
    Down,
    Left,
    None
}

public enum ControlScheme
{
    WASD,
    Arrows,
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

    public ControlScheme currentControlScheme;
    public GraphNode<GameObject> currentlySelectedItem;
    private int columns;
    private int numOfSlots;
    private GraphNode<GameObject>[] itemSlots;
    private Dictionary<ControlScheme, Dictionary<KeyCode, Direction>> inputDictionary;

    // Start is called before the first frame update
    void Start()
    {
        CreateInputDictionary();
        currentControlScheme = ControlScheme.WASD;
        currentlySelectedItem = null;
        columns = 6;
        numOfSlots = 15;
        itemSlots = new GraphNode<GameObject>[numOfSlots];

        CreateInventoryGraph();
    }

    // Update is called once per frame
    void Update()
    {
        // Check for any input and change the selected slot accordingly
        Direction direction = GetInput();
        if(direction != Direction.None)
            MoveSelectedSlot(direction);
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
    /// Create each dictionary for each direction within a larger dictionary for each control scheme
    /// </summary>
    private void CreateInputDictionary()
	{
        inputDictionary = new Dictionary<ControlScheme, Dictionary<KeyCode, Direction>>();

        Dictionary<KeyCode, Direction> wasdInputs = new Dictionary<KeyCode, Direction>();
        wasdInputs.Add(KeyCode.W, Direction.Up);
        wasdInputs.Add(KeyCode.A, Direction.Left);
        wasdInputs.Add(KeyCode.S, Direction.Down);
        wasdInputs.Add(KeyCode.D, Direction.Right);

        inputDictionary.Add(ControlScheme.WASD, wasdInputs);

        Dictionary<KeyCode, Direction> arrowInputs = new Dictionary<KeyCode, Direction>();
        arrowInputs.Add(KeyCode.UpArrow, Direction.Up);
        arrowInputs.Add(KeyCode.LeftArrow, Direction.Left);
        arrowInputs.Add(KeyCode.DownArrow, Direction.Down);
        arrowInputs.Add(KeyCode.RightArrow, Direction.Right);

        inputDictionary.Add(ControlScheme.Arrows, arrowInputs);

        Dictionary<KeyCode, Direction> numPadInputs = new Dictionary<KeyCode, Direction>();
        numPadInputs.Add(KeyCode.Keypad8, Direction.Up);
        numPadInputs.Add(KeyCode.Keypad4, Direction.Left);
        numPadInputs.Add(KeyCode.Keypad5, Direction.Down);
        numPadInputs.Add(KeyCode.Keypad6, Direction.Right);

        inputDictionary.Add(ControlScheme.NumPad, numPadInputs);
    }

    /// <summary>
    /// Create and organize inventory slots into a graph structure
    /// </summary>
    private void CreateInventoryGraph()
	{
        // Create and add slot objects to list
        for(int i = 0; i < itemSlots.Length; i++)
            itemSlots[i] = new GraphNode<GameObject>(null);

        // Dynamically add neighbors
        for(int i = 0; i < itemSlots.Length; i++)
        {
            // Has an above neighbor
            if(i / columns > 0)
                itemSlots[i].SetNeighbor(Direction.Up, itemSlots[i - columns]);

            // Has a right neighbor
            if(i % columns < columns - 1 && i + 1 < itemSlots.Length)
                itemSlots[i].SetNeighbor(Direction.Right, itemSlots[i + 1]);

            // Has a below neighbor
            if(i + columns < itemSlots.Length)
                itemSlots[i].SetNeighbor(Direction.Down, itemSlots[i + columns]);

            // Has a left neighbor
            if(i % columns > 0)
                itemSlots[i].SetNeighbor(Direction.Left, itemSlots[i - 1]);
        }

        // Select the first slot
        SelectItem(itemSlots[0]);
    }

    /// <summary>
    /// Gives a direction based on user input
    /// </summary>
    /// <returns>The corresponding direction</returns>
    private Direction GetInput()
    {
        // Check every key in the control scheme,
        // if a key is down, return the corresponding direction
        foreach(KeyCode key in inputDictionary[currentControlScheme].Keys)
            if(Input.GetKeyDown(key))
                return inputDictionary[currentControlScheme][key];
        // Otherwise, return no direction
        return Direction.None;
    }

    /// <summary>
    /// Selects a new slot based on the given direction
    /// </summary>
    /// <param name="direction">The direction to go towards the new selected slot</param>
    private void MoveSelectedSlot(Direction direction)
	{
        if(currentlySelectedItem.GetNeighbor(direction) != null)
            SelectItem(currentlySelectedItem.GetNeighbor(direction));
    }

    /// <summary>
    /// Deselects the current selection and selects the new selection
    /// </summary>
    /// <param name="selectedItem">The newly selected item</param>
    private void SelectItem(GraphNode<GameObject> selectedItem)
	{
        // Deselect the soon-to-be previously selected item
        if(currentlySelectedItem != null)
            currentlySelectedItem.Selected = false;

        // Select and set the newly selected item
        selectedItem.Selected = true;
        currentlySelectedItem = selectedItem;
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
}
