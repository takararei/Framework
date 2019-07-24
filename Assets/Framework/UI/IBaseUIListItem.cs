using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Framework.UI
{
    public interface IBaseUIListItem
    {
        void Clear();
        int id { get; set; }
        GameObject root { get; set; }
    }
}
