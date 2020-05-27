using System.Collections.Generic;
using HutongGames.PlayMaker;
using Modding;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Darkness
{
    public class Darkness : Mod, ITogglableMod
    {
        private const string VERSION = "0.0.2";

        private readonly Dictionary<(string Name, string EventName), string> OriginalTransitions = new Dictionary<(string Name, string EventName), string>();

        private bool _enabled = true;

        public override string GetVersion() => VERSION;

        public override void Initialize()
        {
            if (!_enabled)
                Darken();

            _enabled = true;

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoad;
            ModHooks.Instance.HeroUpdateHook += Update;
        }

        public void Unload()
        {
            if (_enabled)
                Lighten();

            _enabled = false;

            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoad;
            ModHooks.Instance.HeroUpdateHook -= Update;
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Alpha0)) return;

            if (_enabled)
                Lighten();
            else
                Darken();

            _enabled = !_enabled;
        }

        private void Lighten()
        {
            if (HeroController.instance == null) return;

            foreach (FsmState state in HeroController.instance.vignetteFSM.FsmStates)
            {
                foreach (FsmTransition trans in state.Transitions)
                {
                    if (!OriginalTransitions.TryGetValue((state.Name, trans.EventName), out string orig)) continue;

                    trans.ToState = orig;
                }
            }

            HeroController.instance.vignetteFSM.SetState("Normal");

            HeroController.instance.vignette.enabled = false;
        }

        private void OnSceneLoad(Scene arg0, LoadSceneMode arg1)
        {
            if (HeroController.instance == null) return;

            if (!_enabled) return;

            Log("Updating hc.vignetteFSM");

            Darken();
        }

        private void Darken()
        {
            if (HeroController.instance == null) return;

            foreach (FsmState state in HeroController.instance.vignetteFSM.FsmStates)
            {
                foreach (FsmTransition trans in state.Transitions)
                {
                    switch (trans.ToState)
                    {
                        case "Dark -1":
                        case "Normal":
                        case "Dark 1":
                        case "Lantern":

                            OriginalTransitions[(state.Name, trans.EventName)] = trans.ToState;

                            trans.ToState = "Dark 2";
                            break;
                        case "Dark -1 2":
                        case "Normal 2":
                        case "Dark 1 2":
                        case "Lantern 2":

                            OriginalTransitions[(state.Name, trans.EventName)] = trans.ToState;

                            trans.ToState = "Dark 2 2";
                            break;
                    }
                }
            }

            HeroController.instance.vignetteFSM.SetState("Dark 2");
            HeroController.instance.vignette.enabled = true;
        }
    }
}