using System.Collections;
using System.Linq;
using UnityEngine;

/// <summary>
/// Management game scenes transitions.
/// </summary>
public class SceneManager : MonoBehaviour
{
    private string _prevScene = null;

    /// <summary>
    /// Gets a value that indicates whether the object can be changed to other scene.
    /// </summary>
    public bool CanSceneChange { get { return _canSceneChange;  } }
    private bool _canSceneChange = true;

    /// <summary>
    /// Gets a default(entry) scene name.
    /// </summary>
    public string DefaultScene { get { return _defaultScene; } }
    [SerializeField]
    private string _defaultScene = null;

    /// <summary>
    /// Changes the scene to specified scene name as new scene.
    /// </summary>
    /// <param name="scene">Scene name.</param>
    /// <returns>YieldInstruction.</returns>
    public YieldInstruction ChangeScene(string scene)
    {
        if (_prevScene == null)
        {
            _prevScene = CurrentScene;
        }
        return ChangeScene<object>(scene, null);
    }

    /// <summary>
    /// Changes the scene to specified scene name as new scene.
    /// </summary>
    /// <typeparam name="T">Type of parameter.</typeparam>
    /// <param name="scene">Scene name.</param>
    /// <param name="parameter">The scene parameter to pass to the target scene.</param>
    /// <returns>YieldInstruction</returns>
    public YieldInstruction ChangeScene<T>(string scene, T parameter)
    {
        if (!_canSceneChange) return null;

        return scene != null ?
            StartCoroutine(IterateChangeScene(scene, parameter)) :
            StartCoroutine(Enumerable.Empty<object>().GetEnumerator()); // Empty
    }

    private IEnumerator IterateChangeScene<T>(string scene, T parameter)
    {
        _canSceneChange = false;

        // TODO: Add transition effect for current scene.

        // Removed this GameObject children.
        transform.DetachChildren();

        yield return StartCoroutine(IterateLoadScene(scene, parameter));

        // TODO: Add transition effect for new scene.

        _canSceneChange = true;
    }

    private IEnumerator IterateLoadScene<T>(string scene, T parameter)
    {
        // Destroy previous scene GameObjects.
        var prevSceneRoot = GameObject.Find(_prevScene.ToString());
        if (prevSceneRoot != null)
        {
            Destroy(prevSceneRoot);
        }

        var sceneName = scene.ToString();
        Application.LoadLevelAdditive(sceneName);

        // Finds a scene root GameObject.
        // Scene root GameObject must be same name as the current scene.
        var sceneObject = GameObject.Find(sceneName);
        while (sceneObject == null)
        {
            // TODO: Timeout.
            yield return null;
            sceneObject = GameObject.Find(sceneName);
        }

        InitializeScene(sceneObject, parameter);
        _prevScene = scene;
    }

    private void InitializeScene<T>(GameObject sceneObject, T parameter)
    {
        var st = sceneObject.transform;

        // Gets a root GameObject from new scene.
        var loadedRoot = st.GetRoot().gameObject;

        // Sets Scene root GameObject as child of this SceneManager.
        st.parent = transform;

        // 読み込んだシーンの不要なゲームオブジェクトを削除する
        if (loadedRoot != sceneObject)
        {
            GameObject.Destroy(loadedRoot);
        }

        var components = sceneObject.GetComponents<Scene<T>>();
        foreach (var component in components) component.Initialized(parameter);
    }

    private void Start()
    {
        ChangeScene(DefaultScene);
    }

    public static string CurrentScene
    {
        get
        {
            var name = Application.loadedLevelName;
            return name;
        }
    }
}
