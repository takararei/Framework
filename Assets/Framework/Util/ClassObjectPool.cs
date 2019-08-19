using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Framework
{
    /// <summary>
    /// 类对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClassObjectPool<T>where T:class,new()
    {
        protected Stack<T> m_Pool = new Stack<T>();

        protected int m_MaxCount = 0;//<=0表示不限制个数

        protected int m_NoRecycleCount = 0;//被拿出去没回收的对象

        public ClassObjectPool(int maxCount)
        {
            m_MaxCount = maxCount;
            for (int i = 0; i < maxCount; i++)
            {
                m_Pool.Push(new T());
            }
        }

        public T Spawn(bool createIfPoolEmpty = true)
        {
            if (m_Pool.Count > 0)
            {
                T rtn = m_Pool.Pop();
                if (rtn == null)
                {
                    if (createIfPoolEmpty)
                    {
                        rtn = new T();
                    }
                }

                m_NoRecycleCount++;
                return rtn;
            }
            else
            {
                if (createIfPoolEmpty)
                {
                    T rtn = new T();
                    m_NoRecycleCount++;
                    return rtn;
                }
            }

            return null;
        }

        public bool Recycle(T obj)
        {
            if (obj == null)
                return false;
            if (m_Pool.Count >= m_MaxCount && m_MaxCount > 0)
            {
                obj = null;
                return false;
            }
            m_Pool.Push(obj);
            return true;
        }

    }
}
