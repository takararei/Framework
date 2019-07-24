using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Framework.Singleton
{
    public class Singleton<T> where T:class,new()//必须是Class类型，必须有一个无参的构造函数
    {
        private static T _instance;

        // 定义一个标识确保线程同步
        private static readonly object syslock = new object();
       
        public static T Instance
        {
            get
            {

                if (_instance == null)
                {
                    lock (syslock)
                    {
                        if (_instance == null)
                        {
                            _instance = new T();
                        }
                    }
                }

                return _instance;
            }
        }

        public virtual void Init()
        {

        }
    }
}
