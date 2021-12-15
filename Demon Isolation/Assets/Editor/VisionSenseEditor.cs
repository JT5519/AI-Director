using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VisionSense))]
public class VisionSenseEditor : Editor
{
    private void OnSceneGUI()
    {
        VisionSense vs = (VisionSense)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(vs.transform.position, Vector3.up, Vector3.forward, 360, vs.shortRadiusFOV);

        Vector3 viewAngle01 = DirectionFromAngle(vs.transform.eulerAngles.y, -vs.shortAngleFOV / 2);
        Vector3 viewAngle02 = DirectionFromAngle(vs.transform.eulerAngles.y, vs.shortAngleFOV / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(vs.transform.position, vs.transform.position + viewAngle01 * vs.shortRadiusFOV);
        Handles.DrawLine(vs.transform.position, vs.transform.position + viewAngle02 * vs.shortRadiusFOV);


        Handles.color = Color.red;
        Handles.DrawWireArc(vs.transform.position, Vector3.up, Vector3.forward, 360, vs.longRadiusFOV);

        Vector3 viewAngle03 = DirectionFromAngle(vs.transform.eulerAngles.y, -vs.longAngleFOV / 2);
        Vector3 viewAngle04 = DirectionFromAngle(vs.transform.eulerAngles.y, vs.longAngleFOV / 2);

        Handles.color = Color.blue;
        Handles.DrawLine(vs.transform.position, vs.transform.position + viewAngle03 * vs.longRadiusFOV);
        Handles.DrawLine(vs.transform.position, vs.transform.position + viewAngle04 * vs.longRadiusFOV);

        Handles.color = Color.cyan;
        Handles.DrawWireDisc(vs.transform.position, Vector3.up, vs.extremeShortRadiusFOV);

        if (vs.playerVisible && vs.targetInfo!=null)
        {
            Handles.color = Color.green;
            Handles.DrawLine(vs.transform.position, vs.targetInfo.position);
        }
    }
    private Vector3 DirectionFromAngle(float eulerY,float angleInDegrees)
    {
        angleInDegrees += eulerY;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
