using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{
    public Material mat;



    //public GameObject tangentPrefab;
    public GameObject textmeshObj;

    //public void Instaniate_Tanget_Prefab(Vector3 position) 
    //{
    //    Instantiate(tangentPrefab, position, Quaternion.identity);
    //}

    public void GL_DrawLine(Vector3 start, Vector3 end, Color color)
    {
        if (!mat)
        {
            Debug.LogError("Please Assign a material on the inspector");
            return;
        }

        GL.PushMatrix();
        GL.Begin(GL.LINES);
        mat.SetColor("_Color", color);
        mat.SetPass(0);
        GL.Color(mat.color);
        GL.Color(Color.blue);
        GL.Vertex3(start.x, start.y, start.z);
        GL.Vertex3(end.x, end.y, end.z);
        GL.End();
        GL.PopMatrix();

    }


    // in use only for controlpoints
    public void Create_Info_Text(string text, Transform parent, Color color) 
    {

        GameObject textmesh = Instantiate(textmeshObj, parent.position, Quaternion.identity) as GameObject;
        textmesh.GetComponent<TextMesh>().text = text;
        textmesh.transform.SetParent(parent);
        textmesh.transform.position = parent.localPosition;
        textmesh.GetComponent<TextMesh>().color = color;
    }

    
}
