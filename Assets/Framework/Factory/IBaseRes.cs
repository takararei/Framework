using UnityEngine;

namespace Assets.Framework.Factory
{
    public interface IBaseRes
    {
        T GetRes<T>(string resourcePath) where T : Object;
    }
}
 