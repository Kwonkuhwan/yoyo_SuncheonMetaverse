using UnityEngine;
using UnityEngine.UI;
using Suncheon;

public class TestColor : MonoBehaviour
{
    [SerializeField] private Image image;

    private void Start()
    {
        UTILS.Log($"{image.color}");
    }
}
