using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Heap<T> where T :IHeapItem<T> {

    T[] items;

    int currentItemCount;

    public Heap(int max)
    {
        items = new T[max];
    }

    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    public int Count
    {
        get
        {
            return currentItemCount;
        }
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    public T RemoveFirst() //removes the first item and returns it
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    void SortDown(T item) //moves an item down the binary tree and sorts it
    {
        bool whileLoop = true;
        while (whileLoop)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;
                if (childIndexRight < currentItemCount)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    whileLoop = false;
                }
            }
            else
            {
                whileLoop = false;
            }
        }
    }


    void SortUp(T item) //moves an item up the binary tree
    {
        bool whileLoop = true;
        int parentIndex = (item.HeapIndex - 1) / 2;
        while (whileLoop)
        {
            T parentItem = items[parentIndex];
            if(item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                whileLoop = false;
            }
        }
    }

    void Swap(T item1, T item2) //swaps two items
    {
        items[item1.HeapIndex] = item2;
        items[item2.HeapIndex] = item1;

        int itemAIndex = item1.HeapIndex;
        item1.HeapIndex = item2.HeapIndex;
        item2.HeapIndex = itemAIndex;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}
