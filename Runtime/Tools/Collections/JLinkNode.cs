namespace JECS
{
    /// <summary>
    /// 链表节点
    /// </summary>
    public class JLinkNode<T>
    {
        internal bool head;
        internal JLinkNode<T> next;
        internal JLinkNode<T> prev;
        internal T item;

        public JLinkNode(T value)
        {
            item = value;
        }

        public JLinkNode<T> Next => next != null && !next.head ? next : null;

        public JLinkNode<T> Previous => prev != null && !head ? prev : null;

        public T Value
        {
            get => item;
            set => item = value;
        }

        internal void Invalidate()
        {
            head = false;
            next = null;
            prev = null;
            item = default(T);
        }
    }
}