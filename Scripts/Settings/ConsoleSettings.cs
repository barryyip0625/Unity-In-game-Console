using System;
using UnityEngine;

namespace BarryY.InGameConsole{
        [Serializable]
        public class DisplaySetting{
            public float defaultFontSize = 24;
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
            public Color logHighlightColor = new Color(94, 94, 94, 255);
        }
}