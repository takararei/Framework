using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoopList : MonoBehaviour
{

    private float _itemHeight;
    public float _OffsetY;//纵向item间距 还有top bottom
    public GameObject LoopItemRes;
    private RectTransform _content;

    private List<LoopListItem> _items;
    private List<LoopListItemModel> _models;

    
    // Use this for initialization
    void Start()
    {
        _items = new List<LoopListItem>();
        _models = new List<LoopListItemModel>();
        _content = transform.Find("Viewport/Content").GetComponent<RectTransform>();
        _itemHeight = LoopItemRes.GetComponent<RectTransform>().rect.height;

        GetModel();//模拟数据获取
        int num = GetShowItemNum(_itemHeight, _OffsetY);
        SpawnItem(num, LoopItemRes);

        SetContentSize();
        transform.GetComponent<ScrollRect>().onValueChanged.AddListener(ValueChange);
    }

    private void ValueChange(Vector2 data)
    {
        foreach (var item in _items)//调用所有子项
        {
            item.OnValueChange();
        }
    }

    int GetShowItemNum(float itemHeight,float OffsetY)
    {
        float height = GetComponent<RectTransform>().rect.height;
        return Mathf.CeilToInt(height / (itemHeight + OffsetY)) + 1;
    }

    private void SpawnItem(int num, GameObject itemPrefab)
    {
        GameObject tempGO = null;
        LoopListItem itemTemp = null;
        for (int i = 0; i < num; i++)
        {
            tempGO = Instantiate(itemPrefab,_content);
            itemTemp=tempGO.AddComponent<LoopListItem>();
            itemTemp.AddGetDataListener(GetData);// Func return _models[index]
            itemTemp.Init(i,_OffsetY,num);
            _items.Add(itemTemp);
        }
    }

    private LoopListItemModel GetData(int index)
    {
        if (index < 0 || index >= _models.Count)
            return new LoopListItemModel();
        return _models[index];
    }

    private void GetModel()
    {
        for (int i = 0; i < 40; i++)
        {
            _models.Add(new LoopListItemModel("test" + i));
        }
    }

    private void SetContentSize()
    {
        float y = _models.Count * _itemHeight + (_models.Count - 1) * _OffsetY;
        _content.sizeDelta = new Vector2(_content.sizeDelta.x, y);
    }

}
