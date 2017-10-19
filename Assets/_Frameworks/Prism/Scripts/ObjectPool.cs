using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Prism
{
    /// <summary>
    /// ��UGUIԴ����Ų���������
    /// </summary>
    /// <typeparam name="T">����</typeparam>
    internal class ObjectPool<T> where T : new()
    {
        private readonly Stack<T> m_Stack = new Stack<T>();
        private readonly UnityAction<T> m_ActionOnGet;
        private readonly UnityAction<T> m_ActionOnRelease;

        public int countAll { get; private set; }
        public int countActive { get { return countAll - countInactive; } }
        public int countInactive { get { return m_Stack.Count; } }

        /// <summary>
        /// ʵ�������
        /// </summary>
        /// <param name="actionOnGet"></param>
        /// <param name="actionOnRelease"></param>
        public ObjectPool(UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease)
        {
            m_ActionOnGet = actionOnGet;
            m_ActionOnRelease = actionOnRelease;
        }
        /// <summary>
        /// �ӳ����ȡ
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            T element;
            if (m_Stack.Count == 0)
            {
                element = new T();
                countAll++;
            }
            else
            {
                element = m_Stack.Pop();
            }
            if (m_ActionOnGet != null)
            {
                m_ActionOnGet(element);
            }
            return element;
        }

        /// <summary>
        /// �ͷŻس�
        /// </summary>
        /// <param name="element"></param>
        public void Release(T element)
        {
            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
            {
                Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
            }
            if (m_ActionOnRelease != null)
            {
                m_ActionOnRelease(element);
            }
            m_Stack.Push(element);
        }
    }
}
