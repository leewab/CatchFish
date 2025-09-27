using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class SortingOrderVisulizer : MonoBehaviour {

    public string sortingLayerName = "";
    public int sortingOrder = 0;

    public bool sortingOrderOverride = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        sortingLayerName = GetComponent<Renderer>().sortingLayerName;
        if (!sortingOrderOverride)
        {
            sortingOrder = GetComponent<Renderer>().sortingOrder;
        }
        else
        {
            GetComponent<Renderer>().sortingOrder = sortingOrder;
        }

	}
}
