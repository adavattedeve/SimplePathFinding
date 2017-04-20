using System;

public class BinaryHeap<T> where T : IHeapItem<T>  {

    public T[] items;
    private int currentItemCount;

    public int Count
    {
        get
        {
            return currentItemCount;
        }
    }

    /// <summary>
    /// Constructor for the heap
    /// </summary>
    /// <param name="maxHeapSize"> Maximum count of items that the heap is able to contain </param>
    public BinaryHeap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    /// <summary>
    /// Adds item to the heap
    /// </summary>
    /// <param name="item"> Item to be added </param>
    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        currentItemCount++;

        SortUp(item);
    }

    /// <summary>
    /// Clears the heap
    /// </summary>
    public void Clear()
    {
        currentItemCount = 0;
    }

    /// <summary>
    /// Removes first item from the heap and returns it
    /// </summary>
    /// <returns> Item that has smallest value in the heap </returns>
    public T First()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);

        return firstItem;
    }

    /// <summary>
    /// Check if heap contains item
    /// </summary>
    /// <param name="item"> Item to be checked </param>
    /// <returns> Returns true if item is found </returns>
    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    /// <summary>
    /// Sorts item down until there aren't any child items left or child items are greater in value 
    /// </summary>
    /// <param name="item"> Item to be sorted </param>
    public void SortDown(T item)
    {
        //Loop until there aren't any child items left or child items are greater in value
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if (childIndexLeft >= currentItemCount)
                return;

            //Decide which child item we want to swap with
            swapIndex = childIndexLeft;
            if (childIndexRight < currentItemCount &&
                items[childIndexLeft].CompareTo(items[childIndexRight]) > 0)
            {
                swapIndex = childIndexRight;
            }

            if (item.CompareTo(items[swapIndex]) > 0)
            {
                Swap(item, items[swapIndex]);
            } else
            {
                return;
            }

        }
    }

    /// <summary>
    /// Sorts item up until its index is 0 or its parent item is lesser in value
    /// </summary>
    /// <param name="item"> Item to be sorted </param>
    public void SortUp(T item)
    {
        while (true)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;    
            T parentItem = items[parentIndex];

            if (item.CompareTo(parentItem) < 0)
            {
                Swap(item, parentItem);
            } else
            {
                return;
            }

            if (parentIndex <= 0)
            {
                return;
            }
        }
    }

    /// <summary>
    /// Swaps places of itemA and itemB
    /// </summary>
    /// <param name="itemA"> Item to be swapped </param>
    /// <param name="itemB"> Other item to be swapped </param>
    void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}

/// <summary>
/// Interface for heap items
/// </summary>
/// <typeparam name="T"> Class that implements this interface </typeparam>
public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}