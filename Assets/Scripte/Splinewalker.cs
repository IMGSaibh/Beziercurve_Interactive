using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splinewalker : MonoBehaviour
{

    public Beziercurve[] curves;

	int curveCount = 0;

	public float duration;
    private float progress;

	void OnDisable()
	{
		progress = 0.0f;
		curveCount = 0;
	}

	private void Update()
    {
		progress += Time.deltaTime / duration;


		if (progress > 1f)
		{
			progress = 0f;

			//move only on existing curves so reset index of curve array
			if (curves[1].GetControlPoints().Count != 0)
			{
				if (curveCount == curves.Length - 1)
					curveCount = 0;
				else
					curveCount++;
			}

		}

		transform.localPosition = curves[curveCount].GetCurvePoint(progress);




	}
}
