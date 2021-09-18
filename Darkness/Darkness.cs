using System.Collections.Generic;
using System.Reflection;
using HutongGames.PlayMaker;
using JetBrains.Annotations;
using Modding;
using UnityEngine;

namespace Darkness
{
    [UsedImplicitly]
    public class Darkness : Mod, ITogglableMod
    {
        private readonly Dictionary<(string Name, string EventName), string> OriginalTransitions = new ();

        private bool _enabled = true;

        public override string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public override void Initialize()
        {
            if (!_enabled)
                Darken();

            _enabled = true;

            On.GameManager.EnterHero += OnEnterHero;
            ModHooks.HeroUpdateHook += Update;
        }

        private void OnEnterHero(On.GameManager.orig_EnterHero orig, GameManager self, bool additivegatesearch)
        {
            orig(self, additivegatesearch);

            if (_enabled)
                Darken();
        }

        public void Unload()
        {
            if (_enabled)
                Lighten();

            _enabled = false;

            On.GameManager.EnterHero -= OnEnterHero;
            ModHooks.HeroUpdateHook -= Update;
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
            if (HeroController.UnsafeInstance == null) return;

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

        private void Darken()
        {
            if (HeroController.instance == null)
                return;

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