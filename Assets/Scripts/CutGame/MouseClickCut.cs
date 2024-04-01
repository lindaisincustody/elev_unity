using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Angle
{
	Up,
	Forward
}
public class MouseClickCut : MonoBehaviour
{
	public Cutter cutter;
	public Angle angle;

    void Update(){

		if(Input.GetMouseButtonDown(0)){
			RaycastHit hit;

			if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)){

				GameObject victim = hit.collider.gameObject;
				if(victim.tag != "Safe")
				{
                   
                    if(angle == Angle.Up)
					{
						cutter.Cut(victim, hit.point, Vector3.up);
						
                    } 
					else if (angle == Angle.Forward)
					{
						cutter.Cut(victim, hit.point, Vector3.forward);
						
					}
				}
			}

		}
	}
}
