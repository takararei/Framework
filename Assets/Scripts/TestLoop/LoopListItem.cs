using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoopListItem : MonoBehaviour
{
    public int _id;
    public RectTransform Rect;
    public Text txt;
    private float _offsetY;
    private Func<int, LoopListItemModel> _getData;
    private int _startId, _endId;
    private RectTransform _content;

    private int _showItemNum;
    private LoopListItemModel _model;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    //offset应自动获取
    public void Init(int id, float offsetY, int showItemNum)
    {
        _content = transform.parent.GetComponent<RectTransform>();
        Rect = GetComponent<RectTransform>();
        txt = GetComponentInChildren<Text>();
        _offsetY = offsetY;
        _showItemNum = showItemNum;
        ChangeId(id);
    }

    public void AddGetDataListener(Func<int, LoopListItemModel> getData)
    {
        this._getData = getData;
    }

    public void OnValueChange()
    {
        UpdateIdRange();
        JudgeSelfId();
    }

    void UpdateIdRange()
    {
        _startId = Mathf.FloorToInt(_content.anchoredPosition.y / (Rect.rect.height + _offsetY));//获取去掉小数点的整数
        _endId = _startId + _showItemNum - 1;
    }

    void JudgeSelfId()//判断ID的范围
    {
        if (_id < _startId)
        {
            ChangeId(_endId);
        }
        else if (_id > _endId)
        {
            ChangeId(_startId);
        }
    }

    void ChangeId(int id)
    {
        if (CheckIdValid(id))
        {
            _id = id;
            _model = _getData(_id);
            txt.text = _model.txt;
            SetPos();
        }
    }

    bool CheckIdValid(int id)
    {
        return !_getData(id).Equals(new LoopListItemModel());
    }

    void SetPos()
    {
        Rect.anchoredPosition = new Vector2(0, -_id * (Rect.rect.height + _offsetY));
    }
}
