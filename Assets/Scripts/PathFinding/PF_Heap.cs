using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PF_Heap<T> where T : IHeapItem<T>
{
    public int Count => curItemCount;

    public PF_Heap(int _maxHeapSize)
    {
        items = new T[_maxHeapSize];
    }

    public T[] GetArray()
    {
        return items;
    }


    public void Add(T _item)
    {
        _item.HeapIdx = curItemCount;
        items[curItemCount] = _item;
        SortUp(_item);
        ++curItemCount;
    }

    public T RemoveFirstItem()
    {
        T firstItem = items[0];
        --curItemCount;
        items[0] = items[curItemCount];
        items[0].HeapIdx = 0;
        SortDown(items[0]);
        return firstItem;
    }

    /// <summary>
    /// Heap ���� _item�� ��ġ�� �缳��
    /// </summary>
    /// <param name="_item"></param>
    public void UpdateItem(T _item)
    {
        SortUp(_item);
    }


    public bool Contains(T _item)
    {
        return Equals(items[_item.HeapIdx], _item);
    }

    /// <summary>
    /// �� ������ �Ʒ��� �������鼭 �ڸ��� ã�ư�
    /// </summary>
    /// <param name="_item"></param>
    void SortDown(T _item)
    {
        while (true)
        {
            int childIdxLeft = (_item.HeapIdx << 1) + 1;
            int childIdxRight = (_item.HeapIdx << 1) + 2;
            int swapIdx = 0;

            // �������� �׻� �������� ä������ ������ ���� �ڽĺ��� üũ
            if(childIdxLeft < curItemCount)
            {
                swapIdx = childIdxLeft;

                // ������ �ڽĵ� �����Ѵٸ�
                if (childIdxRight < curItemCount)
                {
                    // �� �� �� ���� �켱������ ��
                    if (items[childIdxLeft].CompareTo(items[childIdxRight]) < 0)
                        swapIdx = childIdxRight;
                }

                // �ڽĺ��� �켱������ �� ���ٸ� Swap
                if (_item.CompareTo(items[swapIdx]) < 0)
                    Swap(_item, items[swapIdx]);
                else
                    return;
            }
            // �ڽ��� ���ٸ� ����
            else
                return;
        }
    }

    /// <summary>
    /// �ش� ������ ����
    /// </summary>
    /// <param name="_item"></param>
    private void SortUp(T _item)
    {
        int parentIdx = (_item.HeapIdx - 1) / 2;

        while (true)
        {
            T parentItem = items[parentIdx];

            // CompareTo
            // _item�� �켱������ �� ������ 1, ������ 0, ������ -1�� ��ȯ�Ѵ�.
            // ���⼭�� �켱������ fCost�� �Ұ���.
            if (_item.CompareTo(parentItem) > 0)
                Swap(_item, parentItem);
            else
                break;

            parentIdx = (_item.HeapIdx - 1) / 2;
        }
    }

    void Swap(T _lhsItem, T _rhsItem)
    {
        items[_lhsItem.HeapIdx] = _rhsItem;
        items[_rhsItem.HeapIdx] = _lhsItem;

        int lhsItemIdx = _lhsItem.HeapIdx;
        _lhsItem.HeapIdx = _rhsItem.HeapIdx;
        _rhsItem.HeapIdx = lhsItemIdx;
    }

    private T[] items;
    private int curItemCount = 0;
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIdx { get; set; }
}