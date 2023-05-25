using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    //line renderer
    private LineRenderer _lineRenderer;
    public Transform start;
    public Transform end;

    public float _width = 0.1f;
    public Color _color = Color.white;

    // Start is called before the first frame update
    void Start()
    {
        //create a new line renderer
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        //disable it
        _lineRenderer.enabled = false;
        //set the color
        _lineRenderer.material.color = _color;
        //set the width
        _lineRenderer.startWidth = _width;
    }

    void Update()
    {
        if (start != null && end != null)
        {
            if (!_lineRenderer.enabled)
            {
                //enable the line renderer
                _lineRenderer.enabled = true;
            }
            //set the color
            _lineRenderer.material.color = _color;
            //set the width
            _lineRenderer.startWidth = _width;

            //set the start and end positions of the line renderer
            _lineRenderer.SetPosition(0, start.position);
            _lineRenderer.SetPosition(1, end.position);
        }
        else
        {
            //disable the line renderer
            _lineRenderer.enabled = false;
        }
    }
}