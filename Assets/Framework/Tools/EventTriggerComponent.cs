using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Framework.Tools
{
    [RequireComponent(typeof(EventTrigger))]
    public class EventTriggerComponent : MonoBehaviour
    {
        
        private EventTrigger.Entry _onPointerEnter;
        private EventTrigger.Entry _onPointerExit;
        private EventTrigger.Entry _onPointerDown;
        private EventTrigger.Entry _onPointerUp;
        private EventTrigger.Entry _onPointerClick;
        private EventTrigger.Entry _onBeginDrag;
        private EventTrigger.Entry _onEndDrag;
        private EventTrigger.Entry _onDrag;
        private EventTrigger.Entry _onDrop;
        private EventTrigger.Entry _onScroll;
        private EventTrigger.Entry _onUpdateSelected;
        private EventTrigger.Entry _onSelect;
        private EventTrigger.Entry _onDeselect;
        private EventTrigger.Entry _onMove;
        private EventTrigger.Entry _onInitializePotentialDrag;
        private EventTrigger.Entry _onSubmit;
        private EventTrigger.Entry _onCancel;



        public EventTrigger EventTrigger => GetComponent<EventTrigger>();
        public EventTrigger.Entry OnPointerEnter
        {
            get
            {
                if (_onPointerEnter != null) return _onPointerEnter;
                _onPointerEnter = GetEventEntry(EventTriggerType.PointerEnter);
                return _onPointerEnter;
            }
        }
        public EventTrigger.Entry OnPointerExit
        {
            get
            {
                if (_onPointerExit != null) return _onPointerExit;
                _onPointerExit = GetEventEntry(EventTriggerType.PointerExit);
                return _onPointerExit;
            }
        }
        public EventTrigger.Entry OnPointerUp
        {
            get
            {
                if (_onPointerUp != null) return _onPointerUp;
                _onPointerUp = GetEventEntry(EventTriggerType.PointerUp);
                return _onPointerUp;
            }
        }
        public EventTrigger.Entry OnPointerDown
        {
            get
            {
                if (_onPointerDown != null) return _onPointerDown;
                _onPointerDown = GetEventEntry(EventTriggerType.PointerDown);
                return _onPointerDown;
            }
        }
        public EventTrigger.Entry OnPointerClick
        {
            get
            {
                if (_onPointerClick != null) return _onPointerClick;
                _onPointerClick = GetEventEntry(EventTriggerType.PointerClick);
                return _onPointerClick;
            }
        }
        public EventTrigger.Entry OnBeginDrag
        {
            get
            {
                if (_onBeginDrag != null) return _onBeginDrag;
                _onBeginDrag = GetEventEntry(EventTriggerType.BeginDrag);
                return _onBeginDrag;
            }
        }
        public EventTrigger.Entry OnEndDrag
        {
            get
            {
                if (_onEndDrag != null) return _onEndDrag;
                _onEndDrag = GetEventEntry(EventTriggerType.EndDrag);
                return _onEndDrag;
            }
        }
        public EventTrigger.Entry OnDrag
        {
            get
            {
                if (_onDrag != null) return _onDrag;
                _onDrag = GetEventEntry(EventTriggerType.Drag);
                return _onDrag;
            }
        }
        public EventTrigger.Entry OnDrop
        {
            get
            {
                if (_onDrop != null) return _onDrop;
                _onDrop = GetEventEntry(EventTriggerType.Drop);
                return _onDrop;
            }
        }
        public EventTrigger.Entry OnScroll
        {
            get
            {
                if (_onScroll != null) return _onScroll;
                _onScroll = GetEventEntry(EventTriggerType.Scroll);
                return _onScroll;
            }
        }
        public EventTrigger.Entry OnUpdateSelected
        {
            get
            {
                if (_onUpdateSelected != null) return _onUpdateSelected;
                _onUpdateSelected = GetEventEntry(EventTriggerType.UpdateSelected);
                return _onUpdateSelected;
            }
        }
        public EventTrigger.Entry OnSelect
        {
            get
            {
                if (_onSelect != null) return _onSelect;
                _onSelect = GetEventEntry(EventTriggerType.Select);
                return _onSelect;
            }
        }
        public EventTrigger.Entry OnDeselect
        {
            get
            {
                if (_onDeselect != null) return _onDeselect;
                _onDeselect = GetEventEntry(EventTriggerType.Deselect);
                return _onDeselect;
            }
        }
        public EventTrigger.Entry OnMove
        {
            get
            {
                if (_onMove != null) return _onMove;
                _onMove = GetEventEntry(EventTriggerType.Move);
                return _onMove;
            }
        }
        public EventTrigger.Entry OnInitializePotentialDrag
        {
            get
            {
                if (_onInitializePotentialDrag != null) return _onInitializePotentialDrag;
                _onInitializePotentialDrag = GetEventEntry(EventTriggerType.InitializePotentialDrag);
                return _onInitializePotentialDrag;
            }
        }
        public EventTrigger.Entry OnSubmit
        {
            get
            {
                if (_onSubmit != null) return _onSubmit;
                _onSubmit = GetEventEntry(EventTriggerType.Submit);
                return _onSubmit;
            }
        }
        public EventTrigger.Entry OnCancel
        {
            get
            {
                if (_onCancel != null) return _onCancel;
                _onCancel = GetEventEntry(EventTriggerType.Cancel);
                return _onCancel;
            }
        }

        private EventTrigger.Entry GetEventEntry(EventTriggerType type)
        {
            var entry = new EventTrigger.Entry {eventID = type};
            EventTrigger.triggers.Add(entry);
            return entry;
        }


        
    }
}
