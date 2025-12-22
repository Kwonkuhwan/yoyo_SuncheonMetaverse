using Suncheon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMObject : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        audioSource.volume = GameManager.Instance.AllVolume * GameManager.Instance.BGMVolume;
    }
}
