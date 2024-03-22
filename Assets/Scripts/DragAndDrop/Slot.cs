using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;



public class Slot : MonoBehaviour, IDropHandler
{

    [SerializeField] Transform SlotPoint;

    private bool haveObject;

    private void Start()
    {
        haveObject = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableObject drag = eventData.pointerDrag.GetComponent<DraggableObject>();
        //If the object can be drop in this slot
        if (haveObject || drag.canBeDropInSlot == false)
            return;

        drag.EnterSlot(this); //Pass the object self to notify is in slot
        haveObject = true;

        drag.isDropedOnSlot = true;
        eventData.pointerDrag.transform.DOMove(SlotPoint.position, 0.2f);
    }

    public void EmptySlot()
    {
        haveObject = false;
    }
}
