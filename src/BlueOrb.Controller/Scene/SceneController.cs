using BlueOrb.Base.Global;
using BlueOrb.Base.Manager;
using BlueOrb.Common.Container;
using BlueOrb.Controller.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BlueOrb.Controller
{
    [AddComponentMenu("RQ/Manager/Scene Controller")]
    public class SceneController : MonoBehaviour
    {
        /// <summary>
        /// Pointer to the Scene file (.unity)
        /// </summary>
        //[SerializeField]
        //private UnityEngine.Object firstScene;

        [SerializeField]
        private string _firstScene;
        //private Scene scene;

        public event Action BeforeSceneUnload;
        public event Action AfterSceneLoad;
        public float fadeDuration;
        public string FadeOutAnimTriggerName;
        public string FadeInAnimTriggerName;
        public string PersistScene;
        public Animator OverlayAnimator;
        public Image OverlayImage;
        //public SceneConfig startingScene;
        //public string startingSceneName;
        public string initialStartingPositionName;

        private bool isFading;
        private Coroutine _currentSceneSwitch = null;

        //private IEnumerator Start()
        //{
        //    if (SceneManager.sceneCount == 1)
        //        yield return StartCoroutine(LoadSceneAndSetActive(startingScene.SceneName));
        //    StartCoroutine(Fade(0f));
        //}

        private void Awake()
        {
            SceneManager.sceneLoaded += (Scene, LoadSceneMode) =>
            {
                var sceneSetup = GameObject.FindObjectOfType<SceneSetup>();
                if (sceneSetup != null)
                {
                    //sceneSetup.LevelLoaded();
                }
            };
            if (string.IsNullOrEmpty(GlobalStatic.NextScene))
            {
                GlobalStatic.NextScene = _firstScene;
            }
            Debug.Log("(GameController) Next Scene is set to " + GlobalStatic.NextScene);
            // If the only scene open is the Game Controller, then load in the start scene
            // In the Unity Editor, the start scene is the scene loaded when pressing Play,
            // per AutoPlayFirstSceneAtStartOfGame. When playing after build, it loads the title screen.
            if (SceneManager.sceneCount == 1)
            {
                StartCoroutine(LoadSceneAndSetActive(GlobalStatic.NextScene));
                //SceneManager.LoadScene(GlobalStatic.NextScene, LoadSceneMode.Additive);
            }
        }

        //public void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        //{
        //    var sceneSetup = GameObject.FindObjectOfType<SceneSetup>();
        //    if (sceneSetup != null)
        //    {
        //        sceneSetup.LevelLoaded();
        //    }
        //}

        public void FadeAndLoadScene(string sceneName)
        {
            Debug.Log($"Loading scene {sceneName}");
            // In case scenes get switched quickly and the previous one didn't fully load, just abort it
            // and load this instead.
            if (_currentSceneSwitch != null)
                StopCoroutine(_currentSceneSwitch);
            //StopCoroutine()
            _currentSceneSwitch = StartCoroutine(FadeAndSwitchScenes(sceneName));
        }

        public bool IsSceneLoaded(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.name == sceneName)
                    return true;
            }
            return false;
        }

        //public void LoadScene(string sceneName)
        //{
        //    // Ensure the same scene is not loaded twice

        //    SceneManager.LoadScene(sceneName);
        //}

        private IEnumerator FadeAndSwitchScenes(string sceneName)
        {
            yield return StartCoroutine(Fade(FadeOutAnimTriggerName, 1f));
            if (BeforeSceneUnload != null)
                BeforeSceneUnload();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                //if ()
                if (!scene.name.Contains(PersistScene))
                {
                    Debug.Log($"(SceneController) Unloading Scene {scene.name}");
                    yield return SceneManager.UnloadSceneAsync(scene.name);
                }
            }
            ClearScene(true);
            //SceneManager.GetAllScenes();
            //yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            yield return StartCoroutine(LoadSceneAndSetActive(sceneName));
            if (AfterSceneLoad != null)
                AfterSceneLoad();
            //var sceneSetup = GameController.Instance.GetSceneSetup();
            var sceneSetup = GameObject.FindObjectOfType<SceneSetup>();
            //var _overlayColor = sceneSetup.GetSceneLoadColorInfo();

            //if (sceneSetup == null)
            //{
            //    yield return StartCoroutine(Fade(0f));
            //}
            //else if (sceneSetup.GetSceneLoadPerformFadeIn())
            //{
                yield return StartCoroutine(Fade(FadeInAnimTriggerName, 0f));
                //Debug.Log("Performing fade in (BeginSceneState).");
                //GameController.Instance.GetGraphicsEngine().TweenOverlayToColor(_overlayColor);
                //base.TweenOverlayToColor();
            //}

        }

        private void ClearScene(bool destroyAllEntities)
        {
            //Debug.Log("SceneController.ClearScene called");
            //if (destroyAllEntities)
            //    EntityController.Instance.DestroyAllEntities();
            //else
            //    EntityController.Instance.DestroyReceatedEntities();
            EntityContainer.Instance.ResetEntityList();
            //EntityController.Instance.Cleanup();
            //Cleanup();
            //EntityContainer.Instance.SetMainCharacter(null);
            //EntityContainer.Instance.SetCompanionCharacter(null);
            //if (GameStateController.Instance.Data != null)
            //{
                //GameStateController.Instance.SpawnpointUniqueId = null;
                //    UsableContainerController.Instance.UsableContainer.ClearList();
                //    UsableContainerController.Instance.UsableContainer.SetCurrentUsable(null);
            //}

            //InputManager.Instance.RemoveAllEntities();
            // Each scene has its own Action Controller, make sure we don't use one from a previous scene in the new scene
            //GameController.Instance.ActionController = null;
        }

        private IEnumerator LoadSceneAndSetActive(string sceneName)
        {
            Debug.Log($"SceneController LoadSceneAsync called for {sceneName}");
            var persistScene = SceneManager.GetSceneByName(PersistScene);
            if (sceneName == persistScene.path)
            {
                Debug.Log("Cannot switch to the persistent scene");
                yield break;
            }
            //yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            // Need to wait a frame until we can set the next one as active
            yield return null;
            //yield return frame
            //Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
            Debug.Log($"Setting Active scene to {sceneName}");
            var newlyLoadedScene = SceneManager.GetSceneByPath(sceneName);            
            SceneManager.SetActiveScene(newlyLoadedScene);
            yield return null;
        }

        private IEnumerator Fade(string triggerName, float finalAlpha)
        {
            Debug.LogWarning("Fade called");
            OverlayAnimator.SetTrigger(triggerName);
            //var overlayWindow = GameController.Instance.GetGraphicsEngine().GetOverlayWindow();
            //overlayWindow.TweenOverlayToColor(
            //    new RQ.Model.TweenToColorInfo(new Color(0, 0, 0, finalAlpha), 0, fadeDuration));
            isFading = true;
            //var endTime = Time.unscaledTime + fadeDuration;
            //float fadeSpeed = Math.Abs(overlay;
            //while (!Mathf.Approximately(overlayWindow.GetOverlayColor().a, finalAlpha)
            //yield return new WaitUntil(() => OverlayImage.sprite.a == finalAlpha);
            yield return new WaitForSeconds(fadeDuration);

            //while (Time.unscaledTime < endTime)
            //{
            //    yield return null;
            //}
            isFading = false;

        }
    }
}
