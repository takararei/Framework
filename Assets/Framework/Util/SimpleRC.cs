using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Framework.Util
{
    public interface IRefCounter
    {
        int RefCount { get; }

        void Retain(int count);
        void Release(int count);
    }
    //简单的计数器
    public class SimpleRC : IRefCounter
    {
        public SimpleRC()
        {
            RefCount = 0;
        }

        public int RefCount { get; private set; }

        public void Retain(int count=1)
        {
            RefCount+=count;
        }

        public void Release(int count=1)
        {
            RefCount-=count;
            if (RefCount == 0)
            {
                OnZeroRef();
            }
            else if (RefCount < 0)
            {
                throw new Exception("引用低于0");
            }
        }

        protected virtual void OnZeroRef()
        {
        }
    }
}
