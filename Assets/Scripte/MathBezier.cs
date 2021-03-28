using System;
using System.Collections.Generic;
using UnityEngine;

public static class MathBezier
{

    /// <summary>
    /// Taken from http://blog.plover.com/math/choose.html
    /// less fast for large numbers but do not cause an overflow
    /// </summary>
    /// <param name="n">is the total number of items</param>
    /// <param name="k">is the size of the group</param>
    /// <returns></returns>
    public static int GetBinCoeff(int n, int k)
    {
        int result = 1;
        int d;
        if (k > n)
            return 0;

        for (d = 1; d <= k; d++)
        {
            result *= n--;
            result /= d;
        }
        return result;
    }

    public static List<Vector3> GetBernsteinPolynoms(Vector3[] points, float t)
    {

        float oneMinusT = 1f - t;
        List<Vector3> bernsteinpolynome = new List<Vector3>();

        //construct Bersteinpolynom
        for (int i = 0; i < points.Length; i++)
            bernsteinpolynome.Add((GetBinCoeff(points.Length - 1, i) * Mathf.Pow(oneMinusT, (points.Length - 1 - i)) * Mathf.Pow(t, i)) * points[i]);

        return bernsteinpolynome;

    }

    public static float GetCurvature(Vector3 firstDerivative, Vector3 secondDerivative)
    {
        float crossNorm = Vector3.Cross(firstDerivative, secondDerivative).magnitude;
        float powNorm = Mathf.Pow(firstDerivative.magnitude, 3f);
        return crossNorm / powNorm;
    }

    public static float GetRadius(Vector3 firstDerivative, Vector3 secondDerivative) 
    {
        return 1 / GetCurvature(firstDerivative, secondDerivative);
    }

    public static Vector3 GetNormalOfPlane(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 side1 = p2 - p1;
        Vector3 side2 = p3 - p1;
        return Vector3.Cross(side1, side2).normalized;

    }

    public static Vector3 GetFirsDerivative_1F(Vector3 pn_minus_1, Vector3 pn, int curveGrade)
    {
        return curveGrade * (pn - pn_minus_1);
    }

    public static Vector3 GetFirsDerivative_0F(Vector3 p0, Vector3 p1, int curveGrade)
    {
        return curveGrade * (p1 - p0);
    }
    public static Vector3 GetSecondDerivative_1F(Vector3 pn_minus_2, Vector3 pn_minus_1, Vector3 pn, float curveGrade)
    {

        return curveGrade * (curveGrade - 1) * (pn - (2 * pn_minus_1) + pn_minus_2);
    }

    public static Vector3 GetSecondDerivative_0F(Vector3 p2, Vector3 p1, Vector3 p0, float curveGrade)
    {
        return curveGrade * (curveGrade - 1) * (p2 - (2 * p1) + p0);
    }


    public static float Get_Pow_Distance_SharedPoint_to_Handle(Vector3 sharedpoint, Vector3 handle) 
    {
        return Mathf.Pow(Vector3.Distance(sharedpoint, handle), 2f);
    }

    public static float Get_Distance_h(float curvegrade_minus_1, float curvegrade, float distance_a_pow, float curvature)
    {
        return (curvature * distance_a_pow * curvegrade) / curvegrade_minus_1;
    }
    /// <summary>
    /// function to construct a paralell line
    /// having line bn-1 to bn of first curve, we construct a parallel to it using crossproduct 2 times
    /// result will be a point which can used as endvector. We construct the parallel from bn-2 to this point
    /// </summary>
    public static Vector3 Get_Endpoint_Distance_H(Vector3 p1, Vector3 p2, Vector3 p3, float h) 
    {
        Vector3 firstNormal = GetNormalOfPlane(p1, p2, p3);
        Vector3 secondNormal = GetNormalOfPlane(p1, p1 + firstNormal, p2);

        //return constructed distanced from bn-1 to point with distance h
        return p1 + secondNormal.normalized * h;
    }
}
