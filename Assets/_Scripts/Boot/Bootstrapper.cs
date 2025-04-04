using InspectorPathField;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeBase
{
    public class Bootstrapper : MonoBehaviour
    {
        [SerializeField] private PathField _firstScene;


        private void Start()
        {
            string sceneName = ((string)_firstScene).Split('/')[^1].Split('.')[0];

            SceneManager.LoadScene(sceneName);
        }
    }
}