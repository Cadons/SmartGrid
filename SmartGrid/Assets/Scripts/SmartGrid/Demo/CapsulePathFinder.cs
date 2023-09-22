using SmartGrid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsulePathFinder : MonoBehaviour
{
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private SmartGridController _controller;
    private SmartCell _tmp;
    // Start is called before the first frame update
    void Start()
    {
        SmartCell cell = _controller.GetNearest(transform.position);
        SmartCell target = _controller.GetNearest(_target.position);
        _controller.FindPath(cell,target);
    }

    // Update is called once per frame
    void Update()
    {
        SmartCell target = _controller.GetNearest(_target.position);

        if(target != null )
        {
            if(_tmp != target)
            {
                _tmp = target;
                _controller.ResetDebugPath();
                _controller.FindPath(_controller.GetNearest(transform.position), target);
            }
        }
    }
}
