using UnityEngine;
using System.Collections;

public class HeapTesting : MonoBehaviour {
    [SerializeField]
    private int itemCount = 1000;

    private int valueRange = 100;

    void Awake () {
        BinaryTests();
        PairingTests();
    }

    private void BinaryTests()
    {
        BinaryHeap<TestNode> binaryHeap = new BinaryHeap<TestNode>(itemCount);
        float startTime = Time.realtimeSinceStartup;
        for (int i = 0; i < itemCount; ++i)
        {
            binaryHeap.Add(new TestNode(Random.Range(-valueRange, valueRange)));
        }
        float time = Time.realtimeSinceStartup - startTime;
        print("Binary heap. Adding items(" + itemCount + "): " + time * 100 + "ms");

        startTime = Time.realtimeSinceStartup;
        isInOrder(binaryHeap);
        time = Time.realtimeSinceStartup - startTime;
        print("Binary heap. Removing items(" + itemCount + "): " + time * 100 + "ms");
    }

    private void PairingTests()
    {
        PairingHeap<TestNode> pairingHeap = new PairingHeap<TestNode>();
        float startTime = Time.realtimeSinceStartup;
        for (int i = 0; i < itemCount; ++i)
        {
            pairingHeap.Add(new TestNode(Random.Range(-valueRange, valueRange)));
        }
        float time = Time.realtimeSinceStartup - startTime;
        print("Pairing heap. Adding items(" + itemCount + "): " + time * 100 + "ms");

        startTime = Time.realtimeSinceStartup;
        isInOrder(pairingHeap);
        time = Time.realtimeSinceStartup - startTime;
        print("Pairing heap. Removing items(" + itemCount + "): " + time * 100 + "ms");
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

    private bool isInOrder(PairingHeap<TestNode> heap)
    {
        int lastValue = heap.First().Value;
        while (!heap.IsEmpty)
        {
            int newValue = heap.First().Value;
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
