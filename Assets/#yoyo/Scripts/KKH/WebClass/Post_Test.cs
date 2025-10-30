using UnityEngine;
using Suncheon;

public class Post_Test : MonoBehaviour
{
    [SerializeField] private string url1;
    [SerializeField] private string url2;

    [SerializeField] private DefaultData defaultData;

    private void Start()
    {
        defaultData = GameManager.Instance.defaultData;
    }

    public void OnPostClick()
    {

    }

    public void OnGetClick()
    {
    }
}
