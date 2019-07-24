using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Assets.Framework.Singleton
{
    public abstract class SingletonReflection<T> where T : SingletonReflection<T>
    {
        protected static T mInstance = null;
        protected SingletonReflection() { }
        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    //获取所有非public的构造方法
                    var ctors = typeof(T)
                        .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);//或，合并条件
                    //找到无参的构造
                    var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);

                    if (ctor == null)
                    {
                        throw new Exception("Non-public ctor() not found");
                    }
                    //调用
                    mInstance = ctor.Invoke(null) as T;

                }
                return mInstance;
            }
        }

    }
}
