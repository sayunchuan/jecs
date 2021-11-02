using System;
using System.Collections.Generic;

namespace JECS
{
    /// <summary>
    /// 链表结构，用于原型管理
    /// </summary>
    public class JLink<T>
    {
        private Queue<JLinkNode<T>> __nodePool = new Queue<JLinkNode<T>>();

        private int __count;
        private JLinkNode<T> __head;

        public int Count => __count;

        public JLinkNode<T> First => __head;

        public JLinkNode<T> Last => __head != null ? __head.prev : null;

        private JLinkNode<T> __NewNode(T value)
        {
            if (__nodePool.Count <= 0)
                return new JLinkNode<T>(value);

            JLinkNode<T> res = __nodePool.Dequeue();
            res.Value = value;
            return res;
        }

        public JLinkNode<T> AddAfter(JLinkNode<T> node, T value)
        {
            JLinkNode<T> newNode = __NewNode(value);
            InternalInsertNodeBefore(node.next, newNode);
            return newNode;
        }

        public JLinkNode<T> AddBefore(JLinkNode<T> node, T value)
        {
            JLinkNode<T> newNode = __NewNode(value);
            InternalInsertNodeBefore(node, newNode);
            if (node == __head)
            {
                node.head = false;
                __head = newNode;
                __head.head = true;
            }

            return newNode;
        }

        public JLinkNode<T> AddFirst(T value)
        {
            JLinkNode<T> newNode = __NewNode(value);
            if (__head == null)
            {
                InternalInsertNodeToEmptyList(newNode);
            }
            else
            {
                __head.head = false;
                InternalInsertNodeBefore(__head, newNode);
                __head = newNode;
                __head.head = true;
            }

            return newNode;
        }

        public JLinkNode<T> AddLast(T value)
        {
            JLinkNode<T> newNode = __NewNode(value);
            if (__head == null)
                InternalInsertNodeToEmptyList(newNode);
            else
                InternalInsertNodeBefore(__head, newNode);
            return newNode;
        }

        public void Clear()
        {
            while (__head != null)
            {
                InternalRemoveNode(__head);
            }

#if UNITY_EDITOR
            if (__head != null)
            {
                throw new Exception("Head not nullptr");
            }

            if (__count != 0)
            {
                throw new Exception("Count not zero");
            }
#endif
        }

        public bool Contains(T value) => Find(value) != null;

        public void CopyTo(T[] array, int index)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), index, "Non-negative number required.");
            if (index > array.Length)
                throw new ArgumentOutOfRangeException(nameof(index), index,
                    "Must be less than or equal to the size of the collection.");
            if (array.Length - index < Count)
                throw new ArgumentException("Insufficient space in the target location to copy the information.");
            JLinkNode<T> JLinkNode = __head;
            if (JLinkNode == null)
                return;
            do
            {
                array[index++] = JLinkNode.item;
                JLinkNode = JLinkNode.next;
            } while (JLinkNode != __head);
        }

        public JLinkNode<T> Find(T value)
        {
            JLinkNode<T> JLinkNode = __head;
            EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
            if (JLinkNode != null)
            {
                if (value != null)
                {
                    while (!equalityComparer.Equals(JLinkNode.item, value))
                    {
                        JLinkNode = JLinkNode.next;
                        if (JLinkNode == __head)
                            goto label_8;
                    }

                    return JLinkNode;
                }

                while (JLinkNode.item != null)
                {
                    JLinkNode = JLinkNode.next;
                    if (JLinkNode == __head)
                        goto label_8;
                }

                return JLinkNode;
            }

            label_8:
            return null;
        }

        public JLinkNode<T> FindLast(T value)
        {
            if (__head == null)
                return null;
            JLinkNode<T> prev = __head.prev;
            JLinkNode<T> JLinkNode = prev;
            EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
            if (JLinkNode != null)
            {
                if (value != null)
                {
                    while (!equalityComparer.Equals(JLinkNode.item, value))
                    {
                        JLinkNode = JLinkNode.prev;
                        if (JLinkNode == prev)
                            goto label_10;
                    }

                    return JLinkNode;
                }

                while (JLinkNode.item != null)
                {
                    JLinkNode = JLinkNode.prev;
                    if (JLinkNode == prev)
                        goto label_10;
                }

                return JLinkNode;
            }

            label_10:
            return null;
        }

        public bool Remove(T value)
        {
            JLinkNode<T> node = Find(value);
            if (node == null)
                return false;
            InternalRemoveNode(node);
            return true;
        }

        public void RemoveFirst()
        {
            if (__head == null)
                throw new InvalidOperationException("The LinkedList is empty.");
            InternalRemoveNode(__head);
        }

        public void RemoveLast()
        {
            if (__head == null)
                throw new InvalidOperationException("The LinkedList is empty.");
            InternalRemoveNode(__head.prev);
        }

        private void InternalInsertNodeBefore(JLinkNode<T> node, JLinkNode<T> newNode)
        {
            newNode.next = node;
            newNode.prev = node.prev;
            node.prev.next = newNode;
            node.prev = newNode;
            ++__count;
        }

        private void InternalInsertNodeToEmptyList(JLinkNode<T> newNode)
        {
            newNode.next = newNode;
            newNode.prev = newNode;
            __head = newNode;
            __head.head = true;
            ++__count;
        }

        internal void InternalRemoveNode(JLinkNode<T> node)
        {
            if (node.next == node)
            {
                __head = null;
            }
            else
            {
                node.next.prev = node.prev;
                node.prev.next = node.next;
                if (__head == node)
                {
                    __head = node.next;
                    __head.head = true;
                }
            }

            node.Invalidate();
            __nodePool.Enqueue(node);
            --__count;
        }
    }
}