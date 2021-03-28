using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Helper))]
public class Beziercurve : MonoBehaviour
{
    public int curveSteps = 20;

    public float controlPointScale = 0.3f;
    public float level_Length = 3.0f;

    public List<Transform> controlPoints;
    Vector3[] controlPointsPositions;

    public bool isConnected = false;
    public bool updateable = false;

    Mesh mesh_level_T0;
    Mesh mesh_level_T1;

    public GameObject lineObj;
    public GameObject hullObj;
    private LineRenderer lCurve;
    private LineRenderer lHull;
    public GameObject level_GC2_T0;
    public GameObject level_GC2_T1;
    public Material level_T0_material;
    public Material level_T1_material;


    Helper helper;


    public void InitializeBezierCurve()
    {
        controlPoints = new List<Transform>();

        lineObj = Instantiate(Resources.Load("LinerendererCurve") as GameObject);
        hullObj = Instantiate(Resources.Load("LinerendererHull") as GameObject);

        lCurve = lineObj.GetComponent<LineRenderer>();
        lHull = hullObj.GetComponent<LineRenderer>();

        //controlPointScale = 0.3f;
        helper = GetComponent<Helper>();
    }

    public void CreateControlPoint()
    {
        
        GameObject controlPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        controlPoint.name = GetControlPoints().Count.ToString();
        controlPoint.transform.localScale = new Vector3(controlPointScale, controlPointScale, controlPointScale);


        Vector3 screenPoint = Input.mousePosition;
        //distance of the plane from the camera
        //Points looks smaller by increasing distance
        screenPoint.z = 10.0f;

        controlPoint.transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
        //create description for points
        helper.Create_Info_Text("P" + controlPoint.name, controlPoint.transform, Color.white);

        controlPoint.transform.SetParent(gameObject.transform);
        controlPoint.AddComponent<InteractableControlPoint>();
        controlPoint.GetComponent<Renderer>().material = Resources.Load("Materials/outline", typeof(Material)) as Material;
        controlPoints.Add(controlPoint.transform);

    }

    public void ShowControlPointText(bool enable) 
    {
        if (GetControlPoints().Count > 0)
        {
            foreach (Transform controlPoint in GetControlPoints())
                controlPoint.GetChild(0).GetComponent<MeshRenderer>().enabled = enable;
        }
    }

    public void CreateControlPoint_VR(Vector3 position)
    {

        GameObject controlPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        controlPoint.name = GetControlPoints().Count.ToString();
        controlPoint.transform.localScale = new Vector3(controlPointScale, controlPointScale, controlPointScale);

        controlPoint.transform.position = position;
        //create description for points
        helper.Create_Info_Text(controlPoint.name, controlPoint.transform, Color.white);

        controlPoint.transform.SetParent(gameObject.transform);
        controlPoint.AddComponent<InteractableControlPoint>();
        //controlPoint.GetComponent<Renderer>().material = Resources.Load("Materials/outline", typeof(Material)) as Material;
        controlPoints.Add(controlPoint.transform);

    }

    public int GetCurveGrade()
    {
        return controlPoints.Count - 1;
    }

    public Vector3 GetCurvePoint(float t)
    {
        Vector3 point = new Vector3(0, 0, 0);
        //calculate point by add each bersteinpolynom
        if (controlPointsPositions != null)
        {
            foreach (Vector3 polynom in MathBezier.GetBernsteinPolynoms(controlPointsPositions, t))
                point += polynom;
        }
        return point;
    }
    public List<Transform> GetControlPoints()
    {
        return controlPoints;
    }

    public Vector3[] GetControlPointsPosition ()
    {
        controlPointsPositions = new Vector3[GetControlPoints().Count];

        if (GetControlPoints() != null)
        {
            for (int i = 0; i < GetControlPoints().Count; i++)
                controlPointsPositions[i] = GetControlPoints()[i].position;
        }
        return controlPointsPositions;
    }

    public List<Vector3> GetCurvePoints() 
    {
        List<Vector3> curvePoints = new List<Vector3>();

        for (float ratio = 0; ratio < 1.0f; ratio += 1.0f / curveSteps)
            curvePoints.Add(GetCurvePoint(ratio));

        //last point of curve t = 1
        curvePoints.Add(GetCurvePoint(1f));

        return curvePoints;
    }

    public void DrawCurve() 
    {
        if (GetCurveRenderer() != null)
        {
            if (GetCurvePoints().Count != 0)
            {
                lCurve.positionCount = GetCurvePoints().Count;
                lCurve.SetPositions(GetCurvePoints().ToArray());
            }
        }
      
    }

    public void DrawControlPoints() 
    {
        if (GetControlPoints() != null)
        {
            lHull.positionCount = GetControlPoints().Count;
            lHull.SetPositions(GetControlPointsPosition());
        }
    }

    public void DeleteControlPoints(int index)
    {
        if (GetControlPoints().Count > 0) 
        {
            Destroy(GetControlPoints()[index - 1].gameObject);
            controlPoints.RemoveAt(index - 1);
        }
    }


    public LineRenderer GetCurveRenderer() 
    {
        return lCurve;
    }

    public LineRenderer GetHullRenderer() 
    {
        return lHull;
    }

    public GameObject GetCurveRendererObj()
    {
        return lineObj;
    }

    public GameObject GetHullRendererObj()
    {
        return hullObj;
    }

    public void SetControlPointScale(float scale) 
    {
        controlPointScale = scale;
    }

    public void ControlPointScale(float scale) 
    {
        controlPointScale = scale;
    }

    public Vector3 GetFirstDerivative_0F() 
    {
        return MathBezier.GetFirsDerivative_0F(
            GetControlPoints()[0].transform.position,
            GetControlPoints()[1].transform.position,
            GetCurveGrade());
    }

    public Vector3 GetFirstDerivative_1F()
    {
        return MathBezier.GetFirsDerivative_1F(
            GetControlPoints()[GetCurveGrade() - 1].transform.position,
            GetControlPoints()[GetCurveGrade()].transform.position,
            GetCurveGrade());

    }

    public Vector3 GetSecondDerivative_1F()
    {
        return MathBezier.GetSecondDerivative_1F(
            GetControlPoints()[GetCurveGrade()].transform.position,
            GetControlPoints()[GetCurveGrade() - 1].transform.position,
            GetControlPoints()[GetCurveGrade() - 2].transform.position,
            GetCurveGrade());
    }

    public Vector3 GetSecondDerivative_0F()
    {
        return MathBezier.GetSecondDerivative_0F(
            GetControlPoints()[0].transform.position,
            GetControlPoints()[1].transform.position,
            GetControlPoints()[2].transform.position,
            GetCurveGrade());
    }


    public void Update_GC2_Plane_T1() 
    {
        if (controlPoints.Count > 2)
        {
            Vector3[] ebenePoints = new Vector3[3];
            ebenePoints[0] = GetCurvePoint(1f);
            ebenePoints[1] = GetCurvePoint(1f) + GetFirstDerivative_1F().normalized * level_Length;
            ebenePoints[2] = GetCurvePoint(1f) + GetSecondDerivative_1F().normalized * level_Length;

            if (ebenePoints != null)
            {
                mesh_level_T1.vertices = ebenePoints;
                mesh_level_T1.RecalculateNormals();
                mesh_level_T1.RecalculateBounds();
            }
            else
            {
                Debug.Log("GC2_Plane_T1 not intialized");
            }
        }
    }

    public void Update_GC2_Plane_T0()
    {
        if (controlPoints.Count > 2)
        {
            Vector3[] ebenePoints = new Vector3[3];
            ebenePoints[0] = GetCurvePoint(0f);
            ebenePoints[1] = GetCurvePoint(0f) + GetFirstDerivative_0F().normalized * level_Length;
            ebenePoints[2] = GetCurvePoint(0f) + GetSecondDerivative_0F().normalized * level_Length;

            if (ebenePoints != null)
            {
                mesh_level_T0.vertices = ebenePoints;
                mesh_level_T0.RecalculateNormals();
                mesh_level_T0.RecalculateBounds();
            }
            else
            {
                Debug.Log("GC2_Plane_T0 not intialized");
            }
        }
    }


    public void Initialize_Plane_GC2_T0(Vector3 p0, Vector3 p1, Vector3 p2, Color color)
    {
        mesh_level_T0 = new Mesh();
        Vector3[] ebenePoints = new Vector3[3];
        ebenePoints = new Vector3[3];
        ebenePoints[0] = p0;
        ebenePoints[1] = p1;
        ebenePoints[2] = p2;

        int[] triangles = new int[6];
        string name = gameObject.name;
        level_GC2_T0 = new GameObject(name + "_Level_T0");
        level_GC2_T0.AddComponent<MeshFilter>();
        level_GC2_T0.AddComponent<MeshRenderer>();
        level_GC2_T0.GetComponent<Renderer>().material = level_T0_material;
        //Renderer renderer = level_GC2_T0.GetComponent<Renderer>();
        //renderer.material.SetColor("_Color", color);

        MeshFilter meshFilter = level_GC2_T0.GetComponent<MeshFilter>();
        

        triangles[0] = ebenePoints.Length - 3;
        triangles[1] = ebenePoints.Length - 2;
        triangles[2] = ebenePoints.Length - 1;

        mesh_level_T0.vertices = ebenePoints;
        mesh_level_T0.triangles = triangles;
        meshFilter.mesh = mesh_level_T0;
    }

    public void Initialize_Plane_GC2_T1(Vector3 p0, Vector3 p1, Vector3 p2, Color color)
    {
        mesh_level_T1 = new Mesh();
        Vector3[] ebenePoints = new Vector3[3];
        ebenePoints = new Vector3[3];
        ebenePoints[0] = p0;
        ebenePoints[1] = p1;
        ebenePoints[2] = p2;

        int[] triangles = new int[6];
        string name = gameObject.name;
        level_GC2_T1 = new GameObject(name + "_Level_T1");
        level_GC2_T1.AddComponent<MeshFilter>();
        level_GC2_T1.AddComponent<MeshRenderer>();
        //Renderer renderer = level_GC2_T1.GetComponent<Renderer>();
        //renderer.material.SetColor("_Color", color);
        level_GC2_T1.GetComponent<Renderer>().material = level_T1_material;


        MeshFilter meshFilter = level_GC2_T1.GetComponent<MeshFilter>();

        triangles[0] = ebenePoints.Length - 3;
        triangles[1] = ebenePoints.Length - 2;
        triangles[2] = ebenePoints.Length - 1;

        mesh_level_T1.vertices = ebenePoints;
        mesh_level_T1.triangles = triangles;
        meshFilter.mesh = mesh_level_T1;
    }

    public float GetRadius_at_T1() 
    {
        return 1 / MathBezier.GetCurvature(GetFirstDerivative_1F(), GetSecondDerivative_1F());
    }

    public float GetRadius_at_T0()
    {
        return 1 / MathBezier.GetCurvature(GetFirstDerivative_0F(), GetSecondDerivative_0F());
    }

    //public void Tangenthint(Helper helper) 
    //{
    //    helper.Instaniate_Tanget_Prefab(GetControlPoints()[GetCurveGrade()].position);
    //}

    private void OnDrawGizmos()
    {
        if (GetControlPoints().Count > 2 && controlPoints != null)
        {
            if (gameObject.name == "Beziercurve_1")
            {
                // 1 Ableitung T = 1
                Gizmos.color = Color.blue;
                Vector3 derivative_1F_length = GetCurvePoint(1f) + level_Length * GetFirstDerivative_1F().normalized;
                Gizmos.DrawLine(GetCurvePoint(1f), derivative_1F_length);
                Handles.Label(derivative_1F_length, "1 Derivative " + gameObject.name);


                //Nomrmal of plane
                //Gizmos.DrawLine(GetCurvePoint(1f), GetCurvePoint(1f) + GetNormal_Of_Plane_1T() * tangentLength);


                // 2. Ableitung T = 1 
                Gizmos.color = Color.blue;
                Vector3 secondDerivative_1F_length = GetCurvePoint(1f) + GetSecondDerivative_1F().normalized * level_Length;
                Gizmos.DrawLine(GetCurvePoint(1f), secondDerivative_1F_length);
                Handles.Label(secondDerivative_1F_length, "2 Derivative " + gameObject.name);


            }
            else
            {
                // 1 Ableitung T = 0
                Gizmos.color = Color.yellow;
                Vector3 derivative_0F_length = GetCurvePoint(0f) + level_Length * GetFirstDerivative_0F().normalized;
                Gizmos.DrawLine(GetCurvePoint(0f), derivative_0F_length);
                Handles.Label(derivative_0F_length, "1 Derivative");

                // 2. Ableitung T = 0
                Vector3 secondDerivative_0F_length = GetCurvePoint(0f) + GetSecondDerivative_0F().normalized * level_Length;
                Gizmos.DrawLine(GetCurvePoint(0f), secondDerivative_0F_length);
                Handles.Label(secondDerivative_0F_length, "2 Derivative");
            }



            //Gizmos.color = Color.red;
            //Vector3 p2_dir = GetControlPoints()[1].transform.position - GetControlPoints()[2].transform.position;
            //Vector3 parellelPoint = Vector3.Project(p2_dir, derivative1_length);
            //p2_dir -= parellelPoint;
            ////Vector3 parellelPoint = (GetControlPoints()[2].transform.position - GetControlPoints()[1].transform.position) +  GetControlPoints()[2].transform.position;
            //Gizmos.DrawLine(GetControlPoints()[2].transform.position, p2_dir);
            //Handles.Label(parellelPoint, "parallel point");


        }
    }


}
