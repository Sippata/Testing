using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Testing.Model
{
    public class RandomIterator<T> : IEnumerable<T>, IEnumerator<T>
    {
        public delegate void CurrentChangedHandler();
        public event CurrentChangedHandler CurrentChanged;

        private readonly T[] _source;

        private int[] _randomIndexes;

        private int _position = -1;

        #region Constructors

        public RandomIterator(IEnumerable<T> source)
        {
            _source = source.ToArray();
            SelectUniqRandomElements(_source.Length);
        }
        
        public RandomIterator(IEnumerable<T> source, int count)
        {
            _source = source.ToArray();
            SelectUniqRandomElements(count);
        }

        public RandomIterator(IEnumerator<T> source, int count)
        {
            List<T> tmp = new List<T>();
            source.Reset();
            while (source.MoveNext())
            {
                tmp.Add(source.Current);
            }
            _source = tmp.ToArray();
            
            SelectUniqRandomElements(count);
        }
        

        #endregion

        public void Refresh(int count)
        {
            if (count < 0 || count > _source.Length)
                count = _source.Length;
            SelectUniqRandomElements(count);
            Reset();
        }
        private void SelectUniqRandomElements(int count)
        {
            if (count > _source.Length || count < 0)
                count = _source.Length;
            
            Random random = new Random();
            var sourceCount = _source.Length;
            var sequence = new List<int>(sourceCount);
            for (int i = 0; i < sourceCount; i++)
            {
                sequence.Add(i);
            }

            _randomIndexes = new int[count];


            for (var i = 0; i < count; i++)
            {
                var tmp = random.Next(sequence.Count);
                _randomIndexes[i] = sequence[tmp];
                sequence.RemoveAt(tmp);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (int index in _randomIndexes)
            {
                yield return _source[index];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            
        }

        public bool MoveNext()
        {
            if (_position == _randomIndexes.Length - 1)
                return false;
            
            _position++;
            CurrentChanged?.Invoke();
            return true;
        }

        public void Reset()
        {
            _position = -1;
            CurrentChanged?.Invoke();
        }

        public T Current
        {
            get
            {
                if (_position < 0 || _position >= _randomIndexes.Length)
                    return default;
                return _source[_randomIndexes[_position]];
            }
        }

        public void Skip()
        {
            if (_position < 0 || _position >= _randomIndexes.Length - 1)
                return;

            var tmp = _randomIndexes[_position];
            for (int i = _position + 1; i < _randomIndexes.Length; ++i)
            {
                _randomIndexes[i - 1] = _randomIndexes[i];
            }

            _randomIndexes[_randomIndexes.Length - 1] = tmp;
            CurrentChanged?.Invoke();
        }

        // ReSharper disable once HeapView.BoxingAllocation
        object IEnumerator.Current => Current;
    }
}