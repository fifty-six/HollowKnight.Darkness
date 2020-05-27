using HutongGames.PlayMaker;
using Modding;
using UnityEngine.SceneManagement;

public class Darkness : Mod
{
        private static string version = "0.0.1";

        public override string GetVersion()
        {
                return version;
        }

        public override void Initialize()
        {
                Log("Removing Light");
                UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_sceneLoaded;
                Log("Removed Light");
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
                Log("Updating hc.vignetteFSM");

                HeroController instance = HeroController.instance;

                FsmState[] fsmStates = instance.vignetteFSM.FsmStates;

                foreach (FsmState fsmState in fsmStates)
                {
                        FsmTransition[] transitions = fsmState.Transitions;

                        foreach (FsmTransition fsmTransition in transitions)
                        {
                                switch (fsmTransition.ToState)
                                {
                                        case "Dark -1":
                                        case "Normal":
                                        case "Dark 1":
                                        case "Lantern":
                                                fsmTransition.ToState = "Dark 2";
                                                break;
                                        case "Dark -1 2":
                                        case "Normal 2":
                                        case "Dark 1 2":
                                        case "Lantern 2":
                                                fsmTransition.ToState = "Dark 2 2";
                                                break;
                                }

                                Log(fsmTransition.ToState);
                        }
                }
        }
}
