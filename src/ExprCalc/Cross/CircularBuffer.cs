using System;
using System.Collections.Generic;

namespace ExprCalc.Cross
{
    public class CircularBuffer<T>
    {
        private readonly T[] _items;
        private int _start;

        public CircularBuffer(int capacity)
        {
            _items = new T[capacity];
            _start = 0;
        }

        public T Peek(int i) => _items[(_start + i) % _items.Length];

        public void Poke(T item)
        {
            _items[_start] = item;
            _start = (_start + 1) % _items.Length;
        }
        
        public T this[int i] => Peek(i);
    }
}
