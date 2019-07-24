using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Framework.Res
{
    public interface IBaseFactory
    {
        GameObject GetItem(string itemName);

        void PushItem(string itemName, GameObject item);
    }
}
