using System;

namespace IACheckers.UI.Forms.Game
{
    public class JavaList<TElement> where TElement : class
    {
        public int Count { get; private set; }
        private JavaListNode<TElement> _head;
        private JavaListNode<TElement> _tail;
        
        public void push_front(TElement elem)
        {
            var node = new JavaListNode<TElement>(elem, null, _head);

            if (_head != null)
                _head.Prev = node;
            else
                _tail = node;

            _head = node;
            Count++;
        }

        public void push_back(TElement elem)
        {
            var node = new JavaListNode<TElement>(elem, _tail, null);

            if (_tail != null)
                _tail.Next = node;
            else
                _head = node;

            _tail = node;
            Count++;
        }
        
        public TElement pop_front()
        {
            if (_head == null)
                return null;

            var node = _head;
            _head = _head.Next;

            if (_head != null)
                _head.Prev = null;
            else
                _tail = null;

            Count--;
            return node.Value;
        }

        public TElement pop_back()
        {
            if (_tail == null)
                return null;

            var node = _tail;
            _tail = _tail.Prev;

            if (_tail != null)
                _tail.Next = null;
            else
                _head = null;

            Count--;
            return node.Value;
        }

        public bool IsEmpty()
        {
            return _head == null;
        }

        public void AppendToTail(JavaList<TElement> other)
        {
            var node = other._head;

            while (node != null)
            {
                push_back(node.Value);
                node = node.Next;
            }
        }
        
        public void Clear()
        {
            _head = _tail = null;
        }
        
        public TElement peek_head()
        {
            return _head?.Value;
        }
        
        public TElement peek_tail()
        {
            return _tail?.Value;
        }
        
        public bool Contains(TElement elem)
        {
            var node = _head;

            while (node != null && !node.Value.Equals(elem))
                node = node.Next;

            return node != null;
        }
        
        public JavaList<TElement> Clone()
        {
            var temp = new JavaList<TElement>();
            var node = _head;

            while (node != null)
            {
                temp.push_back(node.Value);
                node = node.Next;
            }

            return temp;
        }
        
        public override string ToString()
        {
            var temp = "[";
            var node = _head;

            while (node != null)
            {
                temp += node.Value.ToString();
                node = node.Next;
                if (node != null)
                    temp += ", ";
            }
            temp += "]";

            return temp;
        }
        
        public JavaListIterator<TElement> GetIterarator()
        {
            return new JavaListIterator<TElement>(_head);
        }
        
    }
    
    public class NoSuchElementException : Exception
    {
    }

    public class JavaListNode<TElement>
    {
        internal JavaListNode<TElement> Prev, Next;
        internal TElement Value;

        public JavaListNode(TElement elem, JavaListNode<TElement> prevNode, JavaListNode<TElement> nextNode)
        {
            Value = elem;
            Prev = prevNode;
            Next = nextNode;
        }
    }

    public class JavaListIterator<TElement>
    {
        private JavaListNode<TElement> _node;

        internal JavaListIterator(JavaListNode<TElement> start)
        {
            _node = start;
        }

        public bool HasMoreElements()
        {
            return _node != null;
        }

        public TElement NextElement()
        {
            if (_node == null)
                throw new NoSuchElementException();

            var temp = _node.Value;
            _node = _node.Next;

            return temp;
        }
    }
}