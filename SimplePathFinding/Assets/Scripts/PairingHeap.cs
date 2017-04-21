using System.Collections.Generic;
using System;

public class PairingHeap<T> where T : IComparable<T>
{
    public T root;
    public List<PairingHeap<T>> subHeaps;

    public PairingHeap()
    {
        IsEmpty = true;
        subHeaps = new List<PairingHeap<T>>();
    }

    public PairingHeap(T root, List<PairingHeap<T>> subHeaps)
    {
        this.root = root;
        this.subHeaps = subHeaps;
        IsEmpty = false;
    }

    public bool IsEmpty {
        get;
        set; 
    }

    public T First()
    {
        PairingHeap<T> temp = MergePairs(subHeaps);
        T firstItem = root;
        IsEmpty = temp.IsEmpty;
        root = temp.root;
        subHeaps = temp.subHeaps;
        return firstItem;
    }

    public void Add(T item)
    {
        if (IsEmpty)
        {
            root = item;
            IsEmpty = false;
        } else
        {
            Merge(this, new PairingHeap<T>(item, new List<PairingHeap<T>>()));
        }
    }

    private PairingHeap<T> Merge(PairingHeap<T> heap1, PairingHeap<T> heap2)
    {
        if (heap1.IsEmpty)
        {
            return heap2;
        }
        else if (heap2.IsEmpty)
        {
            return heap1;
        }
        else if (heap1.root.CompareTo(heap2.root) < 0)
        {
            heap1.subHeaps.Add(heap2);
            return new PairingHeap<T>(heap1.root, heap1.subHeaps);
        }
        else
        {
            heap2.subHeaps.Add(heap1);
            return new PairingHeap<T>(heap2.root, heap2.subHeaps);
        }

    }
    private PairingHeap<T> MergePairs(List<PairingHeap<T>> pairs)
    {
        if (pairs.Count == 0)
        {
            return new PairingHeap<T>();
        } else if (pairs.Count == 1)
        {
            return pairs[0];
        } else
        {
            PairingHeap<T> heap1 = pairs[0];
            PairingHeap<T> heap2 = pairs[1];
            pairs.RemoveRange(0, 2);
            return Merge(Merge(heap1, heap2), MergePairs(pairs));
        }
    }
}
