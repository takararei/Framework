using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Framework.Util
{
    public interface IRefCounter
    {
        int RefCount { get; }

        void Retain(object refOwner = null);
        void Release(object refOwner = null);
    }
    //简单的计数器
    public class SimpleRC : IRefCounter
    {
        public SimpleRC()
        {
            RefCount = 0;
        }

        public int RefCount { get; private set; }

        public void Retain(object refOwner = null)
        {
            ++RefCount;
        }

        public void Release(object refOwner = null)
        {
            --RefCount;
            if (RefCount == 0)
            {
                OnZeroRef();
            }
        }

        protected virtual void OnZeroRef()
        {
        }
    }
}
