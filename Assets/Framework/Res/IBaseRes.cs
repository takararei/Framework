using UnityEngine;

namespace Assets.Framework.Res
{
    public interface IBaseRes
    {
        T GetRes<T>(string resourcePath) where T : Object;
    }
}
 