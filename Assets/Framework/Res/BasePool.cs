using System.Collections.Generic;
using UnityEngine;

namespace Assets.Framework.Res
{
    public class BasePool:IBasePool 
    {
        //对象池字典 栈 (类型，对象链表) （子弹，子弹对象池）已经生成的
        protected Dictionary<string, Stack<GameObject>> objectPoolDict = new Dictionary<string, Stack<GameObject>>();
       
        public GameObject GetItem(string itemName)
        {
            GameObject itemGo = null;
            //字典中是否有这种类型的对象的对象池
            Stack<GameObject> pool;
            //有对象池
            if (objectPoolDict.TryGetValue(itemName,out pool))
            {
                if(pool.Count==0)
                {//没有就生成
                    GameObject go = ResMgr.Instance.GetRes<GameObject>(itemName);
                    itemGo = GameRoot.Instance.CreateItem(go);
                }//有就弹出
                else
                {
                    itemGo = pool.Pop();
                    itemGo.SetActive(true);
                }
            }
            else//没有对象池
            {
                objectPoolDict.Add(itemName, new Stack<GameObject>());
                GameObject go = ResMgr.Instance.GetRes<GameObject>(itemName);
                itemGo = GameRoot.Instance.CreateItem(go);
            }
            
            return itemGo;
        }

        public void PushItem(string itemName, GameObject item)
        {
            item.SetActive(false);
            item.transform.SetParent(GameRoot.Instance.transform);
            Stack<GameObject> pool;
            if(objectPoolDict.TryGetValue(itemName,out pool))
            {
                pool.Push(item);
            }
            else
            {
                Debug.LogError("字典没有这样的对象池栈" + itemName);
            }
       
        }

        public void ClearAllPool()
        {
            foreach (var item in objectPoolDict.Values)
            {
                item.Clear();
            }

            objectPoolDict.Clear();
        }
    }
}
