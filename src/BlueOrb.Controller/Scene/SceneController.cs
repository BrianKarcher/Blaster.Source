using BlueOrb.Base.Global;
using BlueOrb.Common.Container;
using BlueOrb.Messaging;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BlueOrb.Controller
{
    [AddComponentMenu("RQ/Manager/Scene Controller")]
    public class SceneController : MonoBehaviour
    {
        public event Action BeforeSceneUnload;
        public event Action AfterSceneLoad;
        public float fadeDuration;
        public string FadeOutAnimTriggerName;
        public string FadeInAnimTriggerName;
        public string PersistScene;
        public Animator OverlayAnimator;
        public Image OverlayImage;
        public string initialStartingPositionName;

        private bool isLoadingScene;
        public bool IsLoadingScene => this.isLoadingScene;
        private Coroutine _currentSceneSwitch = null;

        private void Awake()
        {
            Debug.Log("(GameController) Next Scene is set to " + GlobalStatic.NextScene);
        }

        public void FadeAndLoadScene(string sceneName, bool fade)
        {
            this.isLoadingScene = true;
            OverlayAnimator.gameObject.SetActive(true);
            Debug.Log($"Loading scene {sceneName}");
            // In case scenes get switched quickly and the previous one didn't fully load, just abort it
            // and load this instead.
            if (_currentSceneSwitch != null)
                StopCoroutine(_currentSceneSwitch);
            _currentSceneSwitch = StartCoroutine(FadeAndSwitchScenes(sceneName, fade));
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

        private IEnumerator FadeAndSwitchScenes(string sceneName, bool fade)
        {
            if (fade)
            {
                yield return StartCoroutine(Fade(FadeOutAnimTriggerName, 1f));
            }
            if (BeforeSceneUnload != null)
                BeforeSceneUnload();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (!scene.name.Contains(PersistScene))
                {
                    Debug.Log($"(SceneController) Unloading Scene {scene.name}");
                    yield return SceneManager.UnloadSceneAsync(scene.name);
                }
            }
            MessageDispatcher.Instance.DispatchMsg("SceneUnloaded", 0f, null, "Game Controller", null);
            yield return StartCoroutine(LoadSceneAndSetActive(sceneName));
            if (AfterSceneLoad != null)
                AfterSceneLoad();

            if (fade)
            {
                yield return StartCoroutine(Fade(FadeInAnimTriggerName, 0f));
            }
            this.isLoadingScene = false;
        }

        private IEnumerator LoadSceneAndSetActive(string sceneName)
        {
            Debug.Log($"SceneController LoadScene called for {sceneName}");
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
            Debug.Log($"Setting Active scene to {sceneName}");
            var newlyLoadedScene = SceneManager.GetSceneByPath(sceneName);
            // Move this to SceneSetup?
            SceneManager.SetActiveScene(newlyLoadedScene);
            yield return null;
        }

        private IEnumerator Fade(string triggerName, float finalAlpha)
        {
            Debug.Log($"Setting fade to {finalAlpha}");
            if (OverlayAnimator != null)
            {
                OverlayAnimator.SetTrigger(triggerName);
            }
            yield return new WaitForSeconds(fadeDuration);
            Debug.Log($"Fade to {finalAlpha} complete");
        }
    }
}
