using UnityEngine;

public class InteractableControlPoint : MonoBehaviour
{

    public Beziercurve connectedCurve;
    public Beziercurve parentCurve;
    public int sharedPointIndex = -1;
    public bool isSharedPoint = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger == false && other.gameObject.transform.GetComponent<Beziercurve>() == null)
        {
            sharedPointIndex = int.Parse(other.gameObject.name);
            isSharedPoint = true;
            parentCurve = gameObject.transform.parent.GetComponent<Beziercurve>();

            //set new point and coloring the sharedpoint
            connectedCurve = other.gameObject.transform.parent.GetComponent<Beziercurve>();
            other.gameObject.SetActive(false);
            connectedCurve.GetControlPoints()[sharedPointIndex] = gameObject.transform;
            gameObject.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.red);
            gameObject.GetComponentInChildren<TextMesh>().text = "P" + gameObject.name + "=" + "P" + other.name;
            connectedCurve.isConnected = true;
            connectedCurve.updateable = true;

        }

    }


    public int GetPointIndex()
    {
        return int.Parse(gameObject.name);
    }
    public int GetSharedPointIndex() 
    {
        return sharedPointIndex;
    }

}
