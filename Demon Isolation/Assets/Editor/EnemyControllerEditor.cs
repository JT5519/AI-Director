using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyController))]
public class EnemyControllerEditor : Editor
{
    Vector3[] Face = new Vector3[5];
    private Vector3 bottomLeftVert;
    private Vector3 bottomRightVert;
    private Vector3 topRightVert;
    private Vector3 topLeftVert;

    private void OnSceneGUI()
    {
        EnemyController ec = (EnemyController)target;
        Vector3 boxCenter = ec.transform.position;
        boxCenter.y += ec.GetComponent<CapsuleCollider>().height/2;
        float _POIBoxHuntX = ec.POIBoxHuntX / 2;
        float _POIBoxHuntY = ec.POIBoxHuntY / 2;
        float _POIBoxHuntZ = ec.POIBoxHuntZ / 2;

        //Postive Z face
        bottomLeftVert = new Vector3(boxCenter.x - _POIBoxHuntX, boxCenter.y - _POIBoxHuntY, boxCenter.z + _POIBoxHuntZ);
        bottomRightVert = new Vector3(boxCenter.x + _POIBoxHuntX, boxCenter.y - _POIBoxHuntY, boxCenter.z + _POIBoxHuntZ);
        topRightVert =  new Vector3(boxCenter.x + _POIBoxHuntX, boxCenter.y + _POIBoxHuntY, boxCenter.z + _POIBoxHuntZ);
        topLeftVert = new Vector3(boxCenter.x - _POIBoxHuntX, boxCenter.y + _POIBoxHuntY, boxCenter.z + _POIBoxHuntZ);
        DrawFace();

        //Negative Z face
        bottomLeftVert = new Vector3(boxCenter.x - _POIBoxHuntX, boxCenter.y - _POIBoxHuntY, boxCenter.z - _POIBoxHuntZ);
        bottomRightVert = new Vector3(boxCenter.x + _POIBoxHuntX, boxCenter.y - _POIBoxHuntY, boxCenter.z - _POIBoxHuntZ);
        topRightVert = new Vector3(boxCenter.x + _POIBoxHuntX, boxCenter.y + _POIBoxHuntY, boxCenter.z - _POIBoxHuntZ);
        topLeftVert = new Vector3(boxCenter.x - _POIBoxHuntX, boxCenter.y + _POIBoxHuntY, boxCenter.z - _POIBoxHuntZ);
        DrawFace();

        //Positve X face
        bottomLeftVert = new Vector3(boxCenter.x + _POIBoxHuntX, boxCenter.y - _POIBoxHuntY, boxCenter.z + _POIBoxHuntZ);
        bottomRightVert = new Vector3(boxCenter.x + _POIBoxHuntX, boxCenter.y - _POIBoxHuntY, boxCenter.z - _POIBoxHuntZ);
        topRightVert = new Vector3(boxCenter.x + _POIBoxHuntX, boxCenter.y + _POIBoxHuntY, boxCenter.z - _POIBoxHuntZ);
        topLeftVert = new Vector3(boxCenter.x + _POIBoxHuntX, boxCenter.y + _POIBoxHuntY, boxCenter.z + _POIBoxHuntZ);
        DrawFace();

        //Negative X face
        bottomLeftVert = new Vector3(boxCenter.x - _POIBoxHuntX, boxCenter.y - _POIBoxHuntY, boxCenter.z + _POIBoxHuntZ);
        bottomRightVert = new Vector3(boxCenter.x - _POIBoxHuntX, boxCenter.y - _POIBoxHuntY, boxCenter.z - _POIBoxHuntZ);
        topRightVert = new Vector3(boxCenter.x - _POIBoxHuntX, boxCenter.y + _POIBoxHuntY, boxCenter.z - _POIBoxHuntZ);
        topLeftVert = new Vector3(boxCenter.x - _POIBoxHuntX, boxCenter.y + _POIBoxHuntY, boxCenter.z + _POIBoxHuntZ);
        DrawFace();
    }
    private void DrawFace()
    {
        Face[0] = bottomLeftVert;
        Face[1] = bottomRightVert;
        Face[2] = topRightVert;
        Face[3] = topLeftVert;
        Face[4] = bottomLeftVert;

        Handles.color = Color.green;
        Handles.DrawAAPolyLine(Face);
    }
}
