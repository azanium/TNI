using UnityEngine;
using System.Collections;

public class MainController : MonoBehaviour
{
    #region MemVars & Props

    static private MainController mainController;

    private delegate void UpdateDelegate();
    private UpdateDelegate updateSceneDelegate;

    private Component sceneController;

    private string currentScene = "";
    private string nextScene = "";

    private float loadingProgress = 0;
    //private string loadingText = "";
    
    private Texture2D background;
    private Rect screenRect;
    private Rect progressRect;

    #endregion


    #region MonoBehavior's Methods

    protected void Awake()
    {
        mainController = this;
        currentScene = "";
        nextScene = "";

        screenRect = new Rect(0, 0, Screen.width, Screen.height);
        progressRect = new Rect(16, Screen.height - 32, Screen.width - 32, 16);
    }

    protected void OnDestroy()
    {
        mainController = null;
    }

    protected void Start()
    {
        SwitchScene("Menu");
        updateSceneDelegate = UpdateSceneCleanup;
    }

    protected void Update()
    {
        if (updateSceneDelegate != null)
        {
            updateSceneDelegate();
        }
    }

    /*protected void OnGUI()
    {
        if (updateGuiDelegate != null && background != null)
        {
            if (GameSettings.GuiSkin() != null)
            {
                GUI.skin = GameSettings.GuiSkin();
            }
            updateGuiDelegate();
        }
    }*/

    #endregion


    #region Update Delegates

    private void UpdateGui()
    {
        GUI.DrawTexture(screenRect, GameSettings.Background(), ScaleMode.ScaleAndCrop);
        if (loadingProgress > 0)
        {
            progressRect.width = (Screen.width - 32) * loadingProgress;
            GUI.Box(progressRect, "");
        }
    }

    private void UpdateSceneCleanup()
    {
        Resources.UnloadUnusedAssets();

        System.GC.Collect();
        updateSceneDelegate = UpdateSceneLoad;
        if (background == null)
        {
            background = GameSettings.Background();
        }
    }

    private void UpdateSceneLoad()
    {
        loadingProgress = 0;
        sceneController = gameObject.AddComponent(nextScene + "Controller");
        if (sceneController == null)
        {
            Debug.LogWarning("Failed to add component: " + nextScene + "Controller");
        }
        currentScene = nextScene;
        updateSceneDelegate = UpdateSceneLoadingScreen;
    }

    private void UpdateSceneLoadingScreen()
    {
        if (loadingProgress >= 1)
        {
            background = null;
            updateSceneDelegate = UpdateSceneRun;
        }
    }

    private void UpdateSceneRun()
    {
        if (currentScene != nextScene)
        {
            updateSceneDelegate = UpdateSceneUnload;
        }
    }

    private void UpdateSceneUnload()
    {
        if (sceneController != null)
        {
            Destroy(sceneController);
            sceneController = null;
        }
        currentScene = "";
        updateSceneDelegate = UpdateSceneCleanup;
    }

    #endregion


    #region Public Methods

    static public void SetLoadingProgess(float progress)
    {
        if (mainController != null)
        {
            mainController.loadingProgress = progress;
        }
    }

    static public void SwitchScene(string newScene)
    {
        Debug.Log("Switching Scene to: " + newScene);
        if (mainController != null)
        {
            mainController.currentScene = "";
            mainController.nextScene = newScene;
        }
    }

    #endregion


    #region Private Methods

    
    #endregion
}
