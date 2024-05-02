using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    private List<GameObject> logObjects = new List<GameObject>();
    private List<GameObject> warningObjects = new List<GameObject>();
    private List<GameObject> errorObjects = new List<GameObject>();

    // UIs
    private GameObject functionsBar;
    private GameObject inputField;
    private Vector2 defaultSize;
    private TMP_Text logCountText;
    private TMP_Text warningCountText;
    private TMP_Text errorCountText;

    // Logs active state
    private bool showLogs;
    private bool showWarnings;
    private bool showErrors;

    private void OnEnable(){
        defaultSize = GetComponent<RectTransform>().sizeDelta;

        ApplySetting();

        // Start listen log messages
        Application.logMessageReceived += LogToConsoleUI;
    }

    private void Update() {
        if(Input.GetKeyUp(KeyCode.BackQuote)){
            OpenConsole();
        }
    }

#region Log Result to Console
    private void LogToConsoleUI(string logString, string stackTrace, LogType type){
        GameObject log = Instantiate(textPrefab, scrollViewContent.position, Quaternion.identity, scrollViewContent);
        
        log.GetComponent<TMP_Text>().text = GetLogContent(logString, type);
        log.GetComponent<TMP_Text>().color = GetLogColor(type);
        log.GetComponent<RectTransform>().sizeDelta = new Vector2(defaultSize.x-10, log.GetComponent<RectTransform>().sizeDelta.y);

        UpdateLogTypeCount(type, log);
    }

    private string GetLogContent(string logString, LogType type) => type switch{
        var t when t == LogType.Log                             => logString,
        var t when t == LogType.Warning                         => "[WARNING] " + logString,
        var t when t == LogType.Error || t == LogType.Exception => "[ERROR] " + logString,
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
            case LogType.Log        : logObjects.Add(obj);      logCountText.text = $"{logObjects.Count}";          if(!showLogs) obj.SetActive(false); break;
            case LogType.Warning    : warningObjects.Add(obj);  warningCountText.text = $"{warningObjects.Count}";  if(!showWarnings) obj.SetActive(false); break;
            case LogType.Error      : errorObjects.Add(obj);    errorCountText.text = $"{errorObjects.Count}";      if(!showErrors) obj.SetActive(false); break;
            case LogType.Exception  : errorObjects.Add(obj);    errorCountText.text = $"{errorObjects.Count}";      if(!showErrors) obj.SetActive(false); break;
        }
    }
#endregion

#region Settings
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
            GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
        }
        else{
            GetComponent<RectTransform>().sizeDelta = defaultSize;
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

        logObjects = new List<GameObject>();
        warningObjects = new List<GameObject>();
        errorObjects = new List<GameObject>();

        logCountText.text = $"{logObjects.Count}";
        warningCountText.text = $"{warningObjects.Count}";
        errorCountText.text = $"{errorObjects.Count}";
    }

    public void SwitchFullScreenMode(){
        displaySetting.isFullScreen = !displaySetting.isFullScreen;
        SetSize();
    }

    public void SwitchLogsActiceByType(string type){
        switch(type){
            case "Log"          :   showLogs = !showLogs;
                                    foreach(GameObject obj in logObjects){
                                        obj.SetActive(showLogs);
                                    };
                                    logCountText.transform.parent.gameObject.GetComponent<Image>().color = showLogs ? colorSetting.activeLogButtonColor : colorSetting.inactiveLogButtonColor;
                                    break;
            case "Warning"      :   showWarnings = !showWarnings;
                                    foreach(GameObject obj in warningObjects){
                                        obj.SetActive(showWarnings);
                                    };
                                    warningCountText.transform.parent.gameObject.GetComponent<Image>().color = showWarnings ? colorSetting.activeLogButtonColor : colorSetting.inactiveLogButtonColor;
                                    break;
            case "Error"        :   showErrors = !showErrors;
                                    foreach(GameObject obj in errorObjects){
                                        obj.SetActive(showErrors);
                                    };  
                                    break;
                                    errorCountText.transform.parent.gameObject.GetComponent<Image>().color = showErrors ? colorSetting.activeLogButtonColor : colorSetting.inactiveLogButtonColor;
            case "Exception"    :   showErrors = !showErrors;
                                    foreach(GameObject obj in errorObjects){
                                        obj.SetActive(showErrors);
                                    };
                                    errorCountText.transform.parent.gameObject.GetComponent<Image>().color = showErrors ? colorSetting.activeLogButtonColor : colorSetting.inactiveLogButtonColor;
                                    break;
        }
    }

    public void OpenConsole(){
        holder.gameObject.SetActive(true);
    }

    public void SendMessage(TMP_InputField inputField){
        if(inputField.text.Length > 0){
            LogToConsoleUI(inputField.text, new System.Diagnostics.StackTrace().ToString(), LogType.Log);
            inputField.text = string.Empty;
        }
    }
#endregion

#region Collapsible Variables
    [Serializable]
    public class DisplaySetting{
        public KeyCode shortCut = KeyCode.BackQuote;
        public bool showFunctionsBar = true;
        public bool showInputField = true;
        public bool isFullScreen = false;
    }

    [Serializable]
    public class ColorSetting{
        public Color activeLogButtonColor = new Color(128,128,128,255);
        public Color inactiveLogButtonColor = new Color(85,85,85,255);
        public Color consoleBackgroundColor = new Color(0,0,0,230);
        public Color consoleButtonIconColor = new Color(255,255,255,255);
        public Color consoleInputFieldTextColor = new Color(255,255,255,255);
        public Color normalLogColor = new Color(255,255,255,255);
        public Color warningLogColor = new Color(255,193,0,255);
        public Color errorLogColor = new Color(255,0,0,255);
    }
#endregion
}
