using UnityEngine;
using System.Collections;

public class BinaryHeapTest : MonoBehaviour {
    [SerializeField]
    private int itemCount = 1000;

    private int valueRange = 100;

    void Awake () {
        BinaryHeap<TestNode> binaryHeap = new BinaryHeap<TestNode>(itemCount);
        for (int i = 0; i < itemCount; ++i)
        {       
            binaryHeap.Add(new TestNode(Random.Range(-valueRange, valueRange)));
        }
        Debug.Log("Is in correct order: " + isInOrder(binaryHeap));
    }

    private bool isInOrder(BinaryHeap<TestNode> binaryHeap)
    {
        int lastValue = binaryHeap.First().Value;
        while (binaryHeap.Count > 0)
        {
            int newValue = binaryHeap.First().Value;
            if (newValue < lastValue)
                return false;
            lastValue = newValue;
        }
        
        return true;
    }

    private class TestNode : IHeapItem<TestNode>
    {
        private readonly int value;
        private int heapIndex;
        
        public int HeapIndex
        {
            get
            {
                return heapIndex;
            }
            set
            {
                heapIndex = value;
            }
        }

       public int Value {
            get
            {
                return value;
            }
        }

        public TestNode (int value)
        {
            this.value = value;
        }

        public int CompareTo(TestNode other)
        {
            return value.CompareTo(other.Value);
        }
    }
}
