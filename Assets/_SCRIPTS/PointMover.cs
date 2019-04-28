using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointMover : MonoBehaviour {

    public Transform startPoint, endPoint;

	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Mouse0)) //LMB
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform != null)
                {
                    startPoint.position = hit.point;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse1)) //RMB
        {
            
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform != null)
                {
                    endPoint.position = hit.point;
                }
            }
        }
    }
}
