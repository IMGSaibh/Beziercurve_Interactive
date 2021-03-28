using UnityEngine;
using UnityEngine.UI;

public class Beziercurve_Controller : MonoBehaviour
{

    //Bezier variables
    [Range(0.1f, 4.0f)]
    public float parallel_Length = 1;

    public Beziercurve[] curves;
    public Transform[] splinePoints;
    public Helper helper;
    public Splinewalker splineWalker;


    //control variables
    private bool isMouseDrag;
    public GameObject target;
    public Vector3 screenPosition;
    public Vector3 offset;
    public float curveSteps_Curve_1 = 20.0f;
    public float curveSteps_Curve_2 = 20.0f;
    public float level_1_size = 3.0f;
    public float level_2_size = 3.0f;


    //variables to show some geometry
    float dist_h_curve1;
    float dist_h_curve2;
    Vector3 enforcePosition_GC_2;
    Vector3 point1_dist_h, point2_dist_h;

    public GameObject text_Firstderivative_T1;
    public GameObject text_Firstderivative_T0;
    public GameObject text_Radius_pn;
    public GameObject text_Radius_p0;

    // control variables

    private bool toggleGC0 = false; 
    private bool toggleGC1 = false; 
    private bool toggleGC2 = false;

    private bool toggle_ContinuityPlane = false;
    private bool toggleControlPointsTxt = true;
    private bool toggleDerivativesTxt = false;
    private bool activateWalker = false;
    private bool toggleRadiusText = false;
    private bool toggleGC2_lines = false;


    // image hints continuity
    public Image imageGC0;
    public Image imageGC1;
    public Image imageGC2;
    Color32 color32_Blue = new Color32(32, 140, 163, 255);
    Color32 color32_White = new Color32(255, 255, 255, 255);


    void Start()
    {

        //Init vector
        Vector3[] levelCorners = new Vector3[3] 
        {
            new Vector3(0.0f,0.0f,0.0f),
            new Vector3(0.0f,0.0f,0.0f),
            new Vector3(0.0f,0.0f,0.0f)
        };
        for (int i = 0; i < curves.Length; i++)
        {
            curves[i].InitializeBezierCurve();
     
        }

        curves[0].Initialize_Plane_GC2_T1(
         levelCorners[0],
         levelCorners[1],
         levelCorners[2],
         Color.blue
         );

        curves[1].Initialize_Plane_GC2_T0(
          levelCorners[0],
          levelCorners[1],
          levelCorners[2],
          Color.yellow
          );

        // image hints continuity
        
    }

    void Update()
    {

        //if (imageGC0 != null && toggleGC0)
        //{
        //    imageGC0.color = color32_White;
        //}
        if (imageGC1 != null && toggleGC1)
        {
            imageGC1.color = color32_White;
        }
        if (imageGC2 != null && toggleGC2)
        {
            imageGC2.color = color32_White;
        }


        MoveGameObjects();

        //create points
        if (Input.GetMouseButtonDown(1) && target != null && (target.name == "Beziercurve_1" || target.name == "Beziercurve_2"))
            target.GetComponent<Beziercurve>().CreateControlPoint();

        //delete Points
        if (Input.GetKeyDown(KeyCode.Space) && target != null && target.transform.parent == null && curves[0].isConnected == false && curves[1].isConnected == false) 
        {
            splinePoints = new Transform[0];
            target.GetComponent<Beziercurve>().DeleteControlPoints(target.GetComponent<Beziercurve>().GetControlPoints().Count);
        }

        //connect curves
        if (Input.GetKeyDown(KeyCode.LeftControl) && target != null) 
        {
            ConnectCurves();
        }
        if (curves[0].isConnected || curves[1].isConnected)
            imageGC0.color = color32_Blue;

        //Disable connection of curvepoints right after connection was performed
        //shared point will now only be connectable when CTRL + LMB is pressed again
        if (Input.GetKeyUp(KeyCode.LeftControl) && target != null) 
            DisableCurveConnect();

        //disconnect curves at sharedpoint
        if (Input.GetKeyDown(KeyCode.L) && target != null) 
        {
            imageGC0.color = color32_White;
            imageGC1.color = color32_White;
            imageGC2.color = color32_White;

            toggleGC0 = false;
            toggleGC1 = false;
            toggleGC2 = false;

            DisconnectCurves();
        }

        //tangents are kolinear
        //if (Input.GetKeyDown(KeyCode.Alpha1)) 
        //{
        //    toggleGC0 = toggleGC0 ? false : true;
        //}
        //if (imageGC0 != null && toggleGC0)
        //{
        //    AlignTangent();
        //    imageGC0.color = color32_Blue;
        //}

        //tangents are kolinear and have the same length
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            toggleGC1 = toggleGC1 ? false : true;
        }
        if (imageGC1 != null && toggleGC1)
        {
            MirrowedTangent();
            imageGC1.color = color32_Blue;
        }

        //show some geometry for better investigation of CG-X conditions
        if (Input.GetKeyDown(KeyCode.Alpha2)) 
        {
            toggleGC2 = toggleGC2 ? false : true;
        }

        if (imageGC2 != null && toggleGC2)
        {
            imageGC2.color = color32_Blue;
            Construct_GC2_Continuity(toggleGC2);
        }

        if (splinePoints.Length > 4)
            splineWalker.gameObject.SetActive(activateWalker);
        
        //update curves
        foreach (Beziercurve curve in curves)
        {

            if (curve.updateable)
                ConstructSplinePoints();

            curve.ShowControlPointText(toggleControlPointsTxt);
            curve.DrawControlPoints();
            curve.DrawCurve();
        }

        curves[0].level_Length = level_1_size;
        curves[1].level_Length = level_2_size;

        curves[0].curveSteps = (int)curveSteps_Curve_1;
        curves[1].curveSteps = (int)curveSteps_Curve_2;

        if (toggle_ContinuityPlane)
        {
            curves[0].Update_GC2_Plane_T1();
            curves[1].Update_GC2_Plane_T0();
        }

        curves[0].level_GC2_T1.SetActive(toggle_ContinuityPlane);
        curves[1].level_GC2_T0.SetActive(toggle_ContinuityPlane);



    }

    public void AlignTangent()
    {
        if ((curves[0].isConnected || curves[1].isConnected))
        {
            for (int i = 0; i < splinePoints.Length; i++)
            {
                if (splinePoints[i].GetComponent<InteractableControlPoint>().isSharedPoint)
                {
                    if (target.transform.parent.name == "Beziercurve_1")
                    {
                        //leftHandle
                        Vector3 enforceTangent = splinePoints[i].transform.position - splinePoints[i - 1].transform.position;
                        enforceTangent = enforceTangent.normalized * Vector3.Distance(splinePoints[i].transform.position, splinePoints[i + 1].transform.position);
                        splinePoints[i + 1].transform.position = splinePoints[i].transform.position + enforceTangent;
                    }
                    if (target.transform.parent.name == "Beziercurve_2")
                    {
                        //righthandle
                        Vector3 enforceTangent = splinePoints[i].transform.position - splinePoints[i + 1].transform.position;
                        enforceTangent = enforceTangent.normalized * Vector3.Distance(splinePoints[i].transform.position, splinePoints[i - 1].transform.position);
                        splinePoints[i - 1].transform.position = splinePoints[i].transform.position + enforceTangent;
                    }


                }
            }

        }

    }
    private void MirrowedTangent() 
    {
        if ((curves[0].isConnected || curves[1].isConnected) && target != null)
        {
            for (int i = 0; i < splinePoints.Length; i++)
            {
                if (splinePoints[i].GetComponent<InteractableControlPoint>().isSharedPoint)
                {
                    if (target.transform.parent != null && target.transform.parent.name == "Beziercurve_1")
                    {
                        //leftHandle
                        Vector3 enforceTangentLeft = splinePoints[i].transform.position - splinePoints[i - 1].transform.position;
                        splinePoints[i + 1].transform.position = splinePoints[i].transform.position + enforceTangentLeft;
                    }
                    if (target.transform.parent != null && target.transform.parent.name == "Beziercurve_2")
                    {
                        //righthandle
                        Vector3 enforceTangentRight = splinePoints[i].transform.position - splinePoints[i + 1].transform.position;
                        splinePoints[i - 1].transform.position = splinePoints[i].transform.position + enforceTangentRight;
                    }
                }
            }
        }
    }
    private void ConstructSplinePoints() 
    {
        splinePoints = new Transform[(curves[0].transform.childCount - 1) + curves[1].transform.childCount];
        int i = 0;
        foreach (Beziercurve curve in curves)
            foreach (Transform point in curve.transform)
            {
                if (point.gameObject.activeInHierarchy)
                {
                    splinePoints[i] = point;
                    i++;
                }
            }
    }
    private void MoveGameObjects()
    {

        if (Input.GetMouseButtonUp(0))
            isMouseDrag = false;

        if (isMouseDrag)
        {
            //track mouse position.
            Vector3 currentScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z);
            //convert screen position to world position with offset changes.
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenSpace) + offset;
            //It will update target gameobject's current postion.
            target.transform.position = currentPosition;
        }

        //move points
        if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift))
        {
            RaycastHit hitInfo;
            target = ReturnClickedObject(out hitInfo);
            if (target != null)
            {
                isMouseDrag = true;
                //Convert world position to screen position.
                screenPosition = Camera.main.WorldToScreenPoint(target.transform.position);
                offset = target.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z));
            }
        }
    }

    private void ConnectCurves()
    {
        if (target.GetComponent<SphereCollider>() != null)
        {
            target.gameObject.GetComponent<SphereCollider>().isTrigger = true; 
            if (target.gameObject.GetComponent<Rigidbody>() == null)
            {
                target.gameObject.AddComponent<Rigidbody>();
                target.gameObject.GetComponent<Rigidbody>().useGravity = false;
            }
        }

    }

    private void DisableCurveConnect() 
    {
        if (target.gameObject.GetComponent<SphereCollider>() != null)
            target.gameObject.GetComponent<SphereCollider>().isTrigger = false;

        if (target.gameObject.GetComponent<Rigidbody>() != null)
            Destroy(target.gameObject.GetComponent<Rigidbody>());
        
    }

    private void DisconnectCurves()
    {
        target.GetComponent<Renderer>().material.SetColor("_BaseColor", UnityEngine.Color.green);

        for (int i = 0; i < curves.Length; i++)
            for (int k = 0; k < curves[i].transform.childCount; k++)
            {
                if (!curves[i].transform.GetChild(k).gameObject.activeInHierarchy)
                {
                    curves[i].transform.GetChild(k).gameObject.SetActive(true);
                    curves[i].GetComponent<Beziercurve>().GetControlPoints()[k] = curves[i].transform.GetChild(k).gameObject.transform;
                }
                curves[i].transform.GetChild(k).GetComponentInChildren<TextMesh>().text = "P" + k;
                curves[i].transform.GetChild(k).GetComponent<InteractableControlPoint>().isSharedPoint = false;
                curves[i].transform.GetChild(k).GetComponent<InteractableControlPoint>().sharedPointIndex = -1;
                curves[i].GetComponent<Beziercurve>().isConnected = false;
                curves[i].updateable = false;
            }
    }

    GameObject ReturnClickedObject(out RaycastHit hit)
    {
        if (target != null) 
        { 
            target.GetComponent<Renderer>().material.SetFloat("_OutlineThickness", 0f);        
        }

        //GameObject target = null;
        target = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


        if (Physics.Raycast(ray.origin, ray.direction * 10, out hit))
            target = hit.collider.gameObject;

        if (target != null) 
        {
            if (target.name == "Beziercurve_1" || target.name == "Beziercurve_2")
                target.GetComponent<Renderer>().material.SetFloat("_OutlineThickness", 0.1f);
            else
                target.GetComponent<Renderer>().material.SetFloat("_OutlineThickness", 0.3f);
        }


        return target;
    }

    public void Construct_GC2_Continuity(bool enable)
    {
        if (splinePoints != null && enable)
        {
            for (int i = 0; i < splinePoints.Length; i++)
            {
                if (splinePoints[i].GetComponent<InteractableControlPoint>().isSharedPoint)
                {
                    dist_h_curve1 = MathBezier.Get_Distance_h(
                       curves[0].GetCurveGrade() - 1,
                       curves[0].GetCurveGrade(),
                       MathBezier.Get_Pow_Distance_SharedPoint_to_Handle(splinePoints[i].position, splinePoints[i - 1].position),
                       MathBezier.GetCurvature(curves[0].GetFirstDerivative_1F(), curves[0].GetSecondDerivative_1F())
                   );

                    dist_h_curve2 = MathBezier.Get_Distance_h(
                        curves[1].GetCurveGrade() - 1,
                        curves[1].GetCurveGrade(),
                        MathBezier.Get_Pow_Distance_SharedPoint_to_Handle(splinePoints[i].position, splinePoints[i + 1].position),
                        MathBezier.GetCurvature(curves[1].GetFirstDerivative_0F(), curves[1].GetSecondDerivative_0F())
                     );

                    // distance h curve 1
                    point1_dist_h = MathBezier.Get_Endpoint_Distance_H(
                        splinePoints[i - 1].position,
                        splinePoints[i].position,
                        splinePoints[i - 2].position,
                        dist_h_curve1
                        );


                    // distance h curve 2
                    point2_dist_h = MathBezier.Get_Endpoint_Distance_H(
                        splinePoints[i + 1].position,
                        splinePoints[i].position,
                        splinePoints[i + 2].position,
                        dist_h_curve2
                        );

                    //Direction of GC2
                    enforcePosition_GC_2 = splinePoints[i + 2].position - splinePoints[i - 2].position;
                    splinePoints[i - 2].position = point1_dist_h + enforcePosition_GC_2.normalized * parallel_Length;

                    //Debug.Log("Kruemmung Curve 1 Punkt T=1 : " + MathBezier.GetCurvature(curves[0].GetFirstDerivative_1F(), curves[0].GetSecondDerivative_1F()));
                    //Debug.Log("Kruemmung Curve 2 Punkt T=0 : " + MathBezier.GetCurvature(curves[1].GetFirstDerivative_0F(), curves[1].GetSecondDerivative_0F()));

                    //Debug.Log("Radius Curve 1 Punkt T=1 : " + curves[0].GetRadius_at_T1());
                    //Debug.Log("Radius Curve 2 Punkt T=0 : " + curves[1].GetRadius_at_T0());

                    //Debug.Log("Curve 1 a/h: " + dist_h_curve1 / MathBezier.Get_Pow_Distance_SharedPoint_to_Handle(splinePoints[i].position, splinePoints[i - 1].position));
                    //Debug.Log("Curve 2 a/h: " + dist_h_curve2 / MathBezier.Get_Pow_Distance_SharedPoint_to_Handle(splinePoints[i].position, splinePoints[i + 1].position));

                }

            }

        }

    }

    private void OnRenderObject()
    {

       if (splinePoints != null && splinePoints.Length > 4)
        {
            for (int i = 0; i < splinePoints.Length; i++)
            {
                if (splinePoints[i].GetComponent<InteractableControlPoint>().isSharedPoint)
                {
                    if (toggleGC2_lines)
                    {
                        //Hilfslinien GC2 hoehe h
                        helper.GL_DrawLine(splinePoints[i - 1].position,
                            MathBezier.Get_Endpoint_Distance_H(
                                splinePoints[i - 1].position,
                                splinePoints[i].position,
                                splinePoints[i - 2].position,
                                dist_h_curve1
                                ), Color.blue
                            );

                        helper.GL_DrawLine(splinePoints[i + 1].position,
                            MathBezier.Get_Endpoint_Distance_H(
                                splinePoints[i + 1].position,
                                splinePoints[i].position,
                                splinePoints[i + 2].position,
                                dist_h_curve2
                                ), Color.blue
                            );

                        //parallel vector
                        //helper.GL_DrawLine(point1_dist_h, point1_dist_h + enforcePosition_GC_2 * parallel_Length, Color.blue);
                        helper.GL_DrawLine(point1_dist_h, splinePoints[i + 2].position, Color.blue);
                    }



                    //Ableitungen Text
                    text_Firstderivative_T1.SetActive(toggleDerivativesTxt);
                    text_Firstderivative_T0.SetActive(toggleDerivativesTxt);

                    if (toggleDerivativesTxt)
                    {
                        //1 Ableitung curve 1
                        helper.GL_DrawLine(
                            splinePoints[i - 1].position,
                            curves[0].GetCurvePoint(1f) + curves[0].GetFirstDerivative_1F(),
                            Color.cyan
                            );

                        text_Firstderivative_T1.GetComponent<TextMesh>().transform.position = curves[0].GetCurvePoint(1f) + curves[0].GetFirstDerivative_1F();
                        text_Firstderivative_T1.GetComponent<TextMesh>().color = Color.cyan;

                        //1 Ableitung curve 2
                        helper.GL_DrawLine(
                            splinePoints[i].position,
                            curves[1].GetCurvePoint(0f) + curves[1].GetFirstDerivative_0F(),
                            Color.yellow
                            );

                        text_Firstderivative_T0.GetComponent<TextMesh>().transform.position = curves[1].GetCurvePoint(0f) + curves[1].GetFirstDerivative_0F();
                        text_Firstderivative_T0.GetComponent<TextMesh>().color = Color.yellow;
                    }

                    //Radius Schmiegkreis
                    text_Radius_p0.SetActive(toggleRadiusText);
                    text_Radius_pn.SetActive(toggleRadiusText);

                    text_Radius_p0.transform.position = splinePoints[i].position + Vector3.down * curves[1].GetRadius_at_T0();
                    text_Radius_pn.transform.position = splinePoints[i].position + Vector3.down * curves[0].GetRadius_at_T1();

                    //Debug.Log("Raduis p0 = " + curves[1].GetRadius_at_T0());
                    //Debug.Log("Raduis pn = " + curves[0].GetRadius_at_T1());

                    if (toggleRadiusText)
                    {
                        // Radius curve 1
                        helper.GL_DrawLine(
                            splinePoints[i].position,
                            splinePoints[i].position + Vector3.down * curves[0].GetRadius_at_T1(), 
                            Color.white);

                        // Radius curve 2
                        helper.GL_DrawLine(
                            splinePoints[i].position,
                            splinePoints[i].position + Vector3.down * curves[1].GetRadius_at_T0(),
                            Color.black);
                    }
                }
            }
        }
    }

    void OnGUI()
    {
        toggleControlPointsTxt = GUI.Toggle(new Rect(10, 10, 250, 20), toggleControlPointsTxt, "Zeige Kontrollpunkte");
        
        toggleDerivativesTxt = GUI.Toggle(new Rect(10, 40, 250, 20), toggleDerivativesTxt, "Zeige Ableitungen");
        GUI.Label(new Rect(20, 55, 250, 20), "Nur mit Bezier-Spline möglich");
        
        toggle_ContinuityPlane = GUI.Toggle(new Rect(10, 80, 250, 20), toggle_ContinuityPlane, "Zeige Schmiegebenen");


        activateWalker = GUI.Toggle(new Rect(10, 110, 250, 20), activateWalker,"Aktiviere Splinerunner");
        GUI.Label(new Rect(20, 125, 250, 20), "Nur mit Bezier-Spline möglich");

        toggleRadiusText = GUI.Toggle(new Rect(10, 150, 250, 20), toggleRadiusText, "Zeige Radius");
        GUI.Label(new Rect(20, 165, 250, 20), "Nur mit Bezier-Spline möglich");

        GUI.Label(new Rect(20, 195, 250, 20), "Kurven-Interpolation");
        curveSteps_Curve_1 = (int)GUI.HorizontalSlider(new Rect(10, 215, 250, 20), curveSteps_Curve_1, 1.0f, 60.0f);
        curveSteps_Curve_2 = (int)GUI.HorizontalSlider(new Rect(10, 235, 250, 20), curveSteps_Curve_2, 1.0f, 60.0f);


        GUI.Label(new Rect(20, 255, 250, 20), "Schmiegebenengröße");
        level_1_size = GUI.HorizontalSlider(new Rect(10, 275, 250, 20), level_1_size, 1.0f, 10.0f);
        level_2_size = GUI.HorizontalSlider(new Rect(10, 295, 250, 20), level_2_size, 1.0f, 10.0f);

        toggleGC2_lines = GUI.Toggle(new Rect(10, 320, 250, 20), toggleGC2_lines, "GC2 Hilfslinien");
        GUI.Label(new Rect(20, 335, 250, 20), "Nur mit Bezier-Spline möglich");

        //GUI.Label(new Rect(20, 375, 250, 20), "GC Punktbewegung");
        //parallel_Length = GUI.HorizontalSlider(new Rect(10, 395, 250, 20), parallel_Length, 1.0f, 6.0f);

    }


    private void OnDrawGizmos()
    {
        if ((curves[0].isConnected || curves[1].isConnected) && curves.Length > 0)
        {

            for (int i = 0; i < splinePoints.Length; i++)
            {
           
                //if (splinePoints[i].GetComponent<InteractableControlPoint>().isSharedPoint)
                //{

                    //Gizmos.color = Color.yellow;
                    //Gizmos.DrawSphere(splinePoints[i - 2].position, 0.2f);

                    ////radius
                    //float radius = 1 / MathBezier.GetCurvature(curves[0].GetFirstDerivative_1F(), curves[0].GetSeccondDerivative_1F());
                    //Debug.Log("Raduis = " + radius);

                    ////Abstand a von middle - p1 der zweiten Kurve
                    //float distance_A_Curve2 = Vector3.Distance(splinePoints[i].transform.position, splinePoints[i + 1].transform.position);
                    ////Debug.Log("Distanz A von Mitte - B1 an curve2 = " + distance_A_Curve2);

                    //distance_A_Curve2 = Mathf.Pow(distance_A_Curve2, 2f);

                    ////Abstand a von middle - p1 der ersten Kurve
                    //float distance_A_Curve1 = Vector3.Distance(splinePoints[i].transform.position, splinePoints[i - 1].transform.position);
                    //distance_A_Curve1 = Mathf.Pow(distance_A_Curve1, 2f);

                    //Debug Ausgabe
                    //Debug.Log("Distanz A von Mitte - B1 an curve2 ins Quadrat = " + distance_A_Curve2);
                    //Debug.Log("h von B2 zu B1 Kurve 2  = " + (distance_A_Curve2 / radius));


                    //Gizmos.color = Color.red;
                    //Vector3 firstNormal = MathBezier.GetNormalOfPlane(
                    //    curves[0].GetControlPoints()[curves[0].GetCurveGrade() - 1].position,
                    //    curves[0].GetControlPoints()[curves[0].GetCurveGrade()].position,
                    //    curves[0].GetControlPoints()[curves[0].GetCurveGrade() - 2].position
                    //    );

                    //Gizmos.DrawLine(
                    //    curves[0].GetControlPoints()[1].position, 
                    //    curves[0].GetControlPoints()[1].position + firstNormal * tangentLength);

                    //Vector3 secondnormal = MathBezier.GetNormalOfPlane(
                    //    curves[0].GetControlPoints()[curves[0].GetCurveGrade() - 1].position,
                    //    curves[0].GetControlPoints()[curves[0].GetCurveGrade() - 1].position + firstNormal,
                    //    curves[0].GetControlPoints()[curves[0].GetCurveGrade()].position
                    //    );

                    //Gizmos.DrawLine(curves[0].GetControlPoints()[curves[0].GetCurveGrade() - 1].position,
                    //    curves[0].GetControlPoints()[curves[0].GetCurveGrade() - 1].position + secondnormal);


                //}
            }
        }
    }
}
