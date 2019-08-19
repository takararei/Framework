using System.Collections;
using System.Collections.Generic;
using Assets.Framework.Singleton;
using System;

namespace Assets.Framework
{
    public class ObjectMgr : Singleton<ObjectMgr>
    {
        //类对象池 字典
        protected Dictionary<Type, object> m_ClassPoolDict = new Dictionary<Type, object>();

        //创建类对象池
        public ClassObjectPool<T> GetClassPool<T>(int maxCount) where T : class, new()
        {
            Type type = typeof(T);
            object outObj = null;
            if (!m_ClassPoolDict.TryGetValue(type, out outObj) || outObj == null)
            {
                ClassObjectPool<T> newPool = new ClassObjectPool<T>(maxCount);
                m_ClassPoolDict.Add(type, newPool);
                return newPool;
            }

            return outObj as ClassObjectPool<T>;
        }

    }
}


