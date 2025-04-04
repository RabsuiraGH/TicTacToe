using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeBase
{
    public class Logger : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private TextMeshProUGUI _text;
        private string logContent = "";


        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }


        void OnEnable()
        {
            SceneManager.sceneLoaded += (i, m) => _canvas.worldCamera = Camera.main;
            Application.logMessageReceived += HandleLog;
        }


        void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }


        void HandleLog(string logString, string stackTrace, LogType type)
        {
            logContent += $"{type}: {logString}\n";

            _text.text = logContent;
        }
    }
}