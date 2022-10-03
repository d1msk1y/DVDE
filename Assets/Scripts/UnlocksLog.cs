using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UnlocksLog : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject _messageThumbnail;
    [SerializeField] private Transform _content;

    private List<GameObject> _messages = new List<GameObject>();

    private void Start()
    {
        GameManager.instance.OnGameOver += ResetLog;
    }

    public void CreateMessage (string text)
    {
        var message = Instantiate(_messageThumbnail, Vector3.zero, quaternion.identity, _content);
        _messages.Add(message);
        message.GetComponentInChildren<Text>().text = text;
        message.transform.localPosition = Vector2.zero;
    }

    private void ResetLog()
    {
        foreach (var text in _messages) Destroy(text);
        _messages.Clear();
    }
}