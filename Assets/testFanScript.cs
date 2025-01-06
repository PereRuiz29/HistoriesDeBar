using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class testFanScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.DORotate(new Vector3(0, 360, 0), 3f, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);

    }
}
