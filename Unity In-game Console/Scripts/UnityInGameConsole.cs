using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BarryY.InGameConsole{
    public class UnityInGameConsole : MonoBehaviour
    {   
        [Header("Setting")]
        [SerializeField]private DisplaySetting displaySetting;
        [SerializeField]private ColorSetting colorSetting;

        [Header("Prefabs")]
        [SerializeField]private Transform holder;
        [SerializeField]private Transform scrollViewContent;
        [SerializeField]private GameObject textPrefab;

        // Logs Lists
        private Dictionary<LogType, List<GameObject>> logTypeObjects = new Dictionary<LogType, List<GameObject>>();

        // UIs
        private GameObject functionsBar;
        private GameObject inputField;
        private Vector2 defaultSize;
        private TMP_Text logCountText;
        private TMP_Text warningCountText;
        private TMP_Text errorCountText;
        private float fontSize;

        // Logs active state
        private bool showLogs;
        private bool showWarnings;
        private bool showErrors;

        // private on:hover background color
        private string onHoverStyle;

        // Function from jslib
        [DllImport("__Internal")]
        private static extern void CopyLogMessage(string str);

        // Instance
        private static UnityInGameConsole _Instance;
        public static UnityInGameConsole Instance{
            get{
                if(_Instance == null){
                    _Instance = new UnityInGameConsole();
                }

                return _Instance;
            }
        }

        private void Awake()
        {
            if(_Instance != null){
                Destroy(gameObject);
            }

            _Instance = this;

            defaultSize = GetComponent<RectTransform>().sizeDelta * 0.5f;

            fontSize = displaySetting.defaultFontSize;
            
            logTypeObjects[LogType.Log] = new List<GameObject>();
            logTypeObjects[LogType.Warning] = new List<GameObject>();
            logTypeObjects[LogType.Error] = new List<GameObject>();
        }

        private void OnEnable(){
            ApplySetting();

            // Start listen log messages
            Application.logMessageReceived += LogToConsoleUI;
        }


        private void Update() {
            if(Input.GetKeyDown(displaySetting.shortCut)){
                holder.gameObject.SetActive(!holder.gameObject.activeSelf); 
            }
        }

        private void OnDestroy() {
            Application.logMessageReceived -= LogToConsoleUI;
        }

        public void SendMessage(TMP_InputField inputField){
            if(inputField.text.Length > 0){
                //Check command symbol
                if(inputField.text[0].Equals('/')){
                    string input = inputField.text.Replace("/", "");
                    ProcessCommand(input);
                }
                else{
                    // Print the message if it isnt a command
                    LogToConsoleUI(inputField.text, new System.Diagnostics.StackTrace().ToString(), LogType.Log);
                }
                inputField.text = string.Empty;
            }
        }

    #region Log Result to Console
        private void LogToConsoleUI(string logString, string stackTrace, LogType type){
            GameObject log = Instantiate(textPrefab, scrollViewContent.position, Quaternion.identity, scrollViewContent);
            
            var logTextComponent = log.GetComponent<TMP_Text>();
            logTextComponent.text = GetLogContent(logString, type);
            logTextComponent.color = GetLogColor(type);
            logTextComponent.fontSize = fontSize;

            UpdateLogTypeCount(type, log);
            AddEvents(log, logTextComponent);
        }

        private string GetLogContent(string logString, LogType type) => type switch{
            var t when t == LogType.Log                             => logString,
            var t when t == LogType.Warning                         => $"[WARNING] {logString}",
            var t when t == LogType.Error || t == LogType.Exception => $"[ERROR] {logString}",
            _                                                       => logString,
        };

        private Color GetLogColor(LogType type) => type switch{
            var t when t == LogType.Log                             => colorSetting.normalLogColor,
            var t when t == LogType.Warning                         => colorSetting.warningLogColor,
            var t when t == LogType.Error || t == LogType.Exception => colorSetting.errorLogColor,
            _                                                       => colorSetting.normalLogColor,
        };

        private void UpdateLogTypeCount(LogType type, GameObject obj){
            switch(type){
                case LogType.Log        : logTypeObjects[LogType.Log].Add(obj);      logCountText.text = $"{logTypeObjects[LogType.Log].Count}";          if(!showLogs) obj.SetActive(false); break;
                case LogType.Warning    : logTypeObjects[LogType.Warning].Add(obj);  warningCountText.text = $"{logTypeObjects[LogType.Warning].Count}";  if(!showWarnings) obj.SetActive(false); break;
                case LogType.Error      :
                case LogType.Exception  : logTypeObjects[LogType.Error].Add(obj);    errorCountText.text = $"{logTypeObjects[LogType.Error].Count}";      if(!showErrors) obj.SetActive(false); break;
            }
        }

        private void AddEvents(GameObject obj, TMP_Text textComponent){

            // Pointer Enter Event: Add bakcground color to text
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };

            entry.callback.AddListener((eventData) => { 
                textComponent.text = onHoverStyle + textComponent.text;
            });
            
            // Pointer Exit Event: Remove bakcground color to text
            EventTrigger.Entry exit = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };

            exit.callback.AddListener((eventData) => { 
                textComponent.text = textComponent.text.Remove(0,37);
            });

            // Pointer Click Event: Copy text to clipboard
            EventTrigger.Entry click = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };

            click.callback.AddListener((eventData) => {
                string str = textComponent.text.Remove(0,37);

                #if UNITY_WEBGL
                    CopyLogMessage(str);
                #else
                    GUIUtility.systemCopyBuffer = str;
                #endif
            });

            obj.GetComponent<EventTrigger>().triggers.Add(entry);
            obj.GetComponent<EventTrigger>().triggers.Add(exit);
            obj.GetComponent<EventTrigger>().triggers.Add(click);
        }
    #endregion

    #region Settings & Functions
        private void ApplySetting(){
            LoadComponents();
            UpdatePanelVisible();
            UpdatePanelColor();
            SetSize();
            ResetLogCounts();
        }

        private void UpdatePanelColor(){
            Image displayPanel = holder.Find("Display").GetComponent<Image>();
            if (displayPanel != null) displayPanel.color = colorSetting.consoleBackgroundColor;

            functionsBar.GetComponent<Image>().color = colorSetting.consoleBackgroundColor;
        
            Image clearButton = functionsBar.transform.Find("Clear Button").GetComponent<Image>();
            if (clearButton != null) clearButton.color = colorSetting.consoleButtonIconColor;

            Image fullscreenButton = functionsBar.transform.Find("Fullscreen Button").GetComponent<Image>();
            if (fullscreenButton != null) fullscreenButton.color = colorSetting.consoleButtonIconColor;

            Image closeButton = functionsBar.transform.Find("Close Button").GetComponent<Image>();
            if (closeButton != null) closeButton.color = colorSetting.consoleButtonIconColor;

            TMP_Text submitButton = inputField.transform.Find("Submit Button").GetChild(0).GetComponent<TMP_Text>();
            if (submitButton != null) submitButton.color = colorSetting.consoleButtonIconColor;

            Transform _inputField = inputField.transform.Find("InputField");
            _inputField.GetComponent<Image>().color = colorSetting.consoleBackgroundColor;

            TMP_Text placeHolder = _inputField.transform.GetChild(0).Find("Placeholder").GetComponent<TMP_Text>();
            TMP_Text inputFieldText = _inputField.transform.GetChild(0).Find("Text").GetComponent<TMP_Text>();

            placeHolder.color = new Color(colorSetting.consoleInputFieldTextColor.r, colorSetting.consoleInputFieldTextColor.g, colorSetting.consoleInputFieldTextColor.b, 100);
            inputFieldText.color = colorSetting.consoleInputFieldTextColor;
        }

        private void LoadComponents(){
            functionsBar = holder.Find("Functions Bar").gameObject;
            inputField = holder.Find("InputField Group").gameObject;

            logCountText = functionsBar.transform.Find("Log Button").transform.GetChild(1).GetComponent<TMP_Text>();
            warningCountText = functionsBar.transform.Find("Warning Button").transform.GetChild(1).GetComponent<TMP_Text>();
            errorCountText = functionsBar.transform.Find("Error Button").transform.GetChild(1).GetComponent<TMP_Text>();
            
            TMP_InputField fontSizeTest = functionsBar.transform.Find("Font size Button").Find("InputField (TMP)").GetComponent<TMP_InputField>();
            fontSizeTest.text = fontSize.ToString();

            onHoverStyle = $"<mark=#{ColorUtility.ToHtmlStringRGB(colorSetting.logHighlightColor)} padding=\"10, 10, 0, 0\">";
        }

        private void UpdatePanelVisible(){
            if(!displaySetting.showFunctionsBar){
                functionsBar.SetActive(false);
            }

            if(!displaySetting.showInputField){
                inputField.SetActive(false);
            }
        }

        private void SetSize(){
            if(displaySetting.isFullScreen){
                GetComponent<RectTransform>().offsetMin = Vector2.zero;
                GetComponent<RectTransform>().offsetMax = Vector2.zero;
            }
            else{
                GetComponent<RectTransform>().offsetMin = -defaultSize;
                GetComponent<RectTransform>().offsetMax = defaultSize;
            }
        }

        public void ClearConsole(){
            foreach(Transform child in scrollViewContent){
                Destroy(child.gameObject);
            }

            ResetLogCounts();
        }

        public void ResetLogCounts(){
            showLogs = true;
            showWarnings = true;
            showErrors = true;

            logTypeObjects[LogType.Log] = new List<GameObject>();
            logTypeObjects[LogType.Warning] = new List<GameObject>();
            logTypeObjects[LogType.Error] = new List<GameObject>();

            logCountText.text = $"{logTypeObjects[LogType.Log].Count}";
            warningCountText.text = $"{logTypeObjects[LogType.Warning].Count}";
            errorCountText.text = $"{logTypeObjects[LogType.Error].Count}";
        }

        public void SwitchFullScreenMode(){
            displaySetting.isFullScreen = !displaySetting.isFullScreen;
            SetSize();
        }

        public void SwitchLogsActiceByType(string type){
            switch(type){
                case "Log"          :   showLogs = !showLogs;
                                        foreach(GameObject obj in logTypeObjects[LogType.Log]){
                                            obj.SetActive(showLogs);
                                        };
                                        logCountText.transform.parent.gameObject.GetComponent<Image>().color = showLogs ? colorSetting.activeLogButtonColor : colorSetting.inactiveLogButtonColor;
                                        break;
                case "Warning"      :   showWarnings = !showWarnings;
                                        foreach(GameObject obj in logTypeObjects[LogType.Warning]){
                                            obj.SetActive(showWarnings);
                                        };
                                        warningCountText.transform.parent.gameObject.GetComponent<Image>().color = showWarnings ? colorSetting.activeLogButtonColor : colorSetting.inactiveLogButtonColor;
                                        break;
                case "Error"        :   
                case "Exception"    :   showErrors = !showErrors;
                                        foreach(GameObject obj in logTypeObjects[LogType.Error]){
                                            obj.SetActive(showErrors);
                                        };
                                        errorCountText.transform.parent.gameObject.GetComponent<Image>().color = showErrors ? colorSetting.activeLogButtonColor : colorSetting.inactiveLogButtonColor;
                                        break;
            }
        }

        public void UpdateFontSize(TMP_InputField inputField){
            try
            {
                fontSize = float.Parse(inputField.text);

                foreach(LogType key in logTypeObjects.Keys){
                    foreach(GameObject obj in logTypeObjects[key]){
                        var logTextComponent = obj.GetComponent<TMP_Text>();
                        logTextComponent.fontSize = fontSize;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    #endregion

    #region Custom Command
        public delegate void ConsoleCommand(string[] args);
        private static Dictionary<string, ConsoleCommand> commands = new Dictionary<string, ConsoleCommand>();
        private static Dictionary<string, string> commandsDescription = new Dictionary<string, string>();

        public static void AddCommand(string command, ConsoleCommand funciton) {
            commands.Add(command, funciton);
            commandsDescription.Add(command, "");
        }

        public static void AddCommand(string command, ConsoleCommand funciton, string description) {
            commands.Add(command, funciton);
            commandsDescription.Add(command, description);
        }

        private static void RemoveCommand(string command) {
            commands.Remove(command);
            commandsDescription.Remove(command);
        }

        public static Dictionary<string, string> GetCommandsDescription(){
            return commandsDescription;
        }

        private void ProcessCommand(string input){
            string[] inputSplit = input.Split(' ');
            string command = inputSplit[0];
            string[] args = inputSplit.Skip(1).ToArray();
            
            if (commands.ContainsKey(command)){
                commands[command].Invoke(args);
            }
        }
    #endregion
    }
}