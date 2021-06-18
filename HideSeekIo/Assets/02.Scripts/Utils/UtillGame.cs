using System.Collections;

using UnityEngine;

public static class UtillGame
{

    public static Vector2 ConventToVector2(Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.z);
    }

    public static Vector3 ConventToVector3(Vector2 vector2)
    {
        return new Vector3(vector2.x, 0, vector2.y);
    }

    public static float GetAngleY(Vector2 vector2)
    {
        return Mathf.Atan2(vector2.y, vector2.x) * Mathf.Rad2Deg;
    }

    //조이스틱으로 얻은 값을 월드의 각도, 조준점 이나 공격UI시사용
    public static Quaternion WorldRotationByInput(Vector2 vector2)
    {
        var origanlAngle = GetAngleY(vector2);
        var camerAngle = Camera.main.transform.eulerAngles.y;

        return Quaternion.Euler(0, 90 - origanlAngle + camerAngle, 0);
    }
}
