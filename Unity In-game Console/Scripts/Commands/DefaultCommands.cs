using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BarryY.InGameConsole{
    public class DefaultCommands : MonoBehaviour
    {
        void Awake()
        {
            UnityInGameConsole.AddCommand("help", Help, "List Command List");
            UnityInGameConsole.AddCommand("test", Test, "Command for testing");
            UnityInGameConsole.AddCommand("loadScene", LoadScene, "Load scene by scene name. \n    usage: /loadScene [Scene Name] [LoadSceneMode (Optional)]");
            UnityInGameConsole.AddCommand("loadSceneAsync", LoadSceneAsync, "load the scene asynchronously in the background \n    usage: /loadSceneAsync [Scene Name]");
        }

        // -- Command /help: List Command List
        private void Help(string[] arg){
            Debug.Log("Command List:");
            var commandsDescription = UnityInGameConsole.GetCommandsDescription();
            if(commandsDescription != null) {
                foreach(string command in commandsDescription.Keys){
                    Debug.Log($"/{command} - {commandsDescription[command]}");
                }
            } else {
                Debug.LogError("Unable to get commands description.");
            }
        }
        

        // -- Command /test: testing custom command function
        private void Test(string[] arg){
            Debug.Log("Testing");
        }

        // -- Command /loadScene [Scene Name] [LoadSceneMode (Optional)]: load scene by scene name
        // [LoadSceneMode] - Single or Additive
        private void LoadScene(string[] arg){
            if(arg.Length == 1){
                SceneManager.LoadScene(arg[0]);
                return;
            }
        
            if(arg.Length == 2){
                LoadSceneMode mode;
                if(arg[1].Equals("Single")){
                    mode = LoadSceneMode.Single;
                } else if(arg[1].Equals("Additive")){
                    mode = LoadSceneMode.Additive;
                } else {
                    Debug.LogError("Invalid [LoadSceneMode]. Please use 'Single' or 'Additive'");
                    return;
                }
                SceneManager.LoadScene(arg[0], mode);
                return;
            }

            Debug.LogWarning("Missing/Incorrect parameter(s) in /loadScene function");
        }
        
        // -- Command /loadSceneAsync [Scene Name]: load the scene asynchronously in the background
        private void LoadSceneAsync(string[] arg){
            if(arg.Length == 1){
                StartCoroutine(StartLoadSceneAsync(arg[0]));
            }
        }

        IEnumerator StartLoadSceneAsync(string sceenName){
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceenName);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
    }
}
