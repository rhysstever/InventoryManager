using System.Collections;
using System.Collections.Generic;

public class GraphNode<T>
{
	#region Fields
	public T value;
    private Dictionary<Direction, GraphNode<T>> neighbors;
    private bool isSelected;
	#endregion

	#region Properties
    public bool Selected 
    { 
        get { return isSelected; } 
        set { isSelected = value; }
    }
	#endregion

	#region Constructors
    /// <summary>
    /// A new graph node
    /// </summary>
    /// <param name="value">The node's value</param>
	public GraphNode(T value)
	{
        this.value = value;
        neighbors = new Dictionary<Direction, GraphNode<T>>();
        neighbors.Add(Direction.Up, null);
        neighbors.Add(Direction.Right, null);
        neighbors.Add(Direction.Down, null);
        neighbors.Add(Direction.Left, null);
        isSelected = false;
    }
    /// <summary>
    /// A new graph node
    /// </summary>
    /// <param name="value">The node's value</param>
    /// <param name="above">The neighbor that is above the node</param>
    /// <param name="right">The neighbor that is right of the node</param>
    /// <param name="below">The neighbor that is below the node</param>
    /// <param name="left">The neighbor that is left of the node</param>
    public GraphNode(T value, GraphNode<T> above, GraphNode<T> right, GraphNode<T> below, GraphNode<T> left)
	{
        this.value = value;
        neighbors = new Dictionary<Direction, GraphNode<T>>();
        neighbors.Add(Direction.Up, above);
        neighbors.Add(Direction.Right, right);
        neighbors.Add(Direction.Down, below);
        neighbors.Add(Direction.Left, left);
        isSelected = false;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Get the neighbor in a certain direction
    /// </summary>
    /// <param name="direction">The direction of the neighbor</param>
    /// <returns>The neighboring node</returns>
    public GraphNode<T> GetNeighbor(Direction direction)
	{
        return neighbors[direction];
	}
    /// <summary>
    /// Sets a new neighbor for the node
    /// </summary>
    /// <param name="direction">The direction of the neighbor to the node</param>
    /// <param name="neighbor">The neighboring node</param>
    public void SetNeighbor(Direction direction, GraphNode<T> neighbor)
	{
        if (neighbors.ContainsKey(direction))
            neighbors[direction] = neighbor;
	}
    #endregion
}
