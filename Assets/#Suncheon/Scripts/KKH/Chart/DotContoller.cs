using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotContoller : MonoBehaviour
{
    [SerializeField] private GameObject go_dot;

    private void Start()
    {
        for (float i = -45; i < 45; i += 0.05f)
        {
            DrawDot(go_dot, linearFunction(1, 0, i));
        }
    }

    private void DrawDot(GameObject obj ,Vector2 pos)
    {
        GameObject dot = Instantiate(obj, transform);
        dot.transform.position = pos;
    }
    
    private Vector2 linearFunction(float rotation, float plusY, float X)
    {
        float Y = rotation * X + plusY;
        return new Vector2(X, Y);
    }
}
