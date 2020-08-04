using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heap<T> where T : IHeapItem<T>{

    T[] items;
    int currentItemCount = 0;

    public Heap(int _maxHeapSize){
        items = new T[_maxHeapSize];
    }

    public void Add(T item) {
        item.HeapIndex = currentItemCount; // Do not understand
        items[currentItemCount] = item;
        SortUp(items[currentItemCount]);
        currentItemCount++;
    }

    public T RemoveFirst() {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    public bool Contains(T item) {
        return Equals(item, items[item.HeapIndex]);
    }

    public int Count
    {
        get {return currentItemCount;}
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    private void SortDown(T item)
    {
        // it would be more intuitive to say "while (hasLeftChild(index))"
        // because item is being updated during the process
        // it has to repeat until two requirements are meet:
        // 1. no children: leftChildIndex > currentItemCount
        // 2. no children with higher priority: items[swapIndex].CompareTo(item) < 0
        while (true)
        {
            int leftChildIndex = item.HeapIndex * 2 + 1;
            int rightChildIndex = item.HeapIndex * 2 + 2;
            if (leftChildIndex < currentItemCount)
            {
                int swapIndex = leftChildIndex;
                if (rightChildIndex < currentItemCount)
                    if (items[rightChildIndex].CompareTo(items[leftChildIndex]) > 0)
                        swapIndex = rightChildIndex;

                if (items[swapIndex].CompareTo(item) > 0)
                    Swap(item, items[swapIndex]);
                else
                    break;
            }
            else
            {
                break;
            }
        }
    }

    private void SortUp(T item){
        // the reason for using while true loop here is similar to SortDown() as explained above
        while (true) {
            int parentIndex = (item.HeapIndex - 1) / 2;
            if (item.CompareTo(items[parentIndex]) > 0) // higher priority
            {
                Swap(item, items[parentIndex]);
            } else {
                break;
            }
        }
    }

    private void Swap(T t1, T t2)
    {
        items[t1.HeapIndex] = t2;
        items[t2.HeapIndex] = t1;
        int t1Index = t1.HeapIndex;
        t1.HeapIndex = t2.HeapIndex;
        t2.HeapIndex = t1Index;
    }
}

public interface IHeapItem<T> : IComparable<T> {
    int HeapIndex{
        get;
        set;
    }
}
