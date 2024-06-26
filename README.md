# Unity In-game Console

[![openupm](https://img.shields.io/npm/v/com.barryy.unityingameconsole?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.barryy.unityingameconsole/)

<img src="https://i.postimg.cc/VNqzJ2zq/2024-05-06-131116.png" alt="Console"/>

## Links
- Demo: https://barryyip.itch.io/unity-in-game-console-demo
- <img src="https://cdn-icons-png.flaticon.com/512/5969/5969346.png" width="18px"> **Asset Store:** [Coming Soon](https://github.com/barryyip0625/Unity-In-game-Console)
- <a href="https://discord.gg/VRZXNwujt8"><img src="https://static-00.iconduck.com/assets.00/discord-icon-2048x2048-o5mluhz2.png" width="18px"></a> **Discord:** https://discord.gg/VRZXNwujt8
- <a href="https://trello.com/b/98eGoUPJ/unity-in-game-console"><img src="https://cdn-icons-png.flaticon.com/512/2504/2504834.png" width="18px"></a> **Trello:** https://trello.com/b/98eGoUPJ/unity-in-game-console
  

## Table of contents

1. **[About](#about)**
2. **[Installation](#installation)**
3. **[How to use](#how-to-use)**
4. **[License](#license)**
5. **[Support Me](#support-me)**

## About

This asset will log debug messages(including logs, warnings, errors, and exceptions) during runtime in a build.
It also allows users to add custom commands to execute some functions during runtime.

**Functions**:

- Show debug messages

- Add & Execute custom commands

<img src="https://i.ibb.co/52RZYpr/Execute-custom-commands.gif" alt="Execute custom commands" width="640" height="360"/>

- Copy message from in-game console (Not Work in WebGL Build now, will be added in future)

<img src="https://i.ibb.co/L1rqqk9/copy-Message.gif" alt="Copy Message" width="640" height="360"/>

- Show/hide specific types of debug messages

<img src="https://i.postimg.cc/8CSSgwLj/Show-or-hide-messages.gif" alt="Show or hide Messages" width="640" height="360"/>

- Fullscreen mode

<img src="https://i.ibb.co/vL6KZqS/fullscreen.gif" alt="Fullscreen Mode" width="640" height="360"/>

- Responsive UI
  
<img src="https://i.ibb.co/r3mpnBL/rescaling.gif" alt="Responsive UI" width="640" height="360"/>

- Change Font Size

<img src="https://i.ibb.co/kXQwPcC/Change-Font-Size.gif" alt="Change Font Size" width="640" height="360"/>

- Open in-game console by shortcut

**Size**:

Minimum Size: 475*100

Maximum Size: Device Screen Size

## Installation

- Download the latest release
- via [OpenUPM](https://openupm.com/) and install openupm-cli
  
  Run ```openupm add com.barryy.unityingameconsole```
  
## How to use

Place the **Console** prefab in your scene. You can configure the settings according to your needs.

- Add Custom Commands

  1. Add ``using BarryY.InGameConsole;``
  2. Create a new function with parameter(string[] arg)
  3. Add ``UnityInGameConsole.AddCommand("Commands Name", FUNCTION_NAME, "Description")`` into Start();
 
*Remark: Parse the arg[] into the type you need*

*For more information, you can check the **DefaultCommands.cs**.*

Example:
```
using UnityEngine;
using UnityEngine.Scripting;
using BarryY.InGameConsole;

public class SpawnCube : MonoBehaviour
{
    public GameObject cubePrefab;

    private void Start(){
        UnityInGameConsole.AddCommand("spawnCube", SpawnNewCube, "Spawn a cube in scene world space  \n    usage: /spawnCube [x] [y] [z]");
    }

    [Preserve]
    private void SpawnNewCube(string[] arg){
        //arg should contain the value of x,y,z
        if(arg.Length < 3){
            Debug.LogWarning("Missing parameter(s) in /spawnCube function");
            return;
        }

        // Parse parameters to Vector3
        Vector3 position = new Vector3(
            float.Parse(arg[0]),
            float.Parse(arg[1]),
            float.Parse(arg[2])
        );
        
        Instantiate(cubePrefab, position, Quaternion.identity);
    }
}
```
  

- Custom Shortcut
<img src="https://i.ibb.co/B3XWD8X/2024-05-02-145943.png" alt="Custom Shortcut">

- Custom UIs & Color

<img src="https://i.postimg.cc/q7xdJnGQ/2024-05-06-133255.png" alt="Custom UIs and Color">

## License

This asset is licensed under [MIT license](https://github.com/barryyip0625/Unity-In-game-Console/blob/main/LICENSE.md).

## Support Me

<a href="https://www.buymeacoffee.com/barrydev" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/v2/arial-yellow.png" alt="Buy Me A Coffee" style="height: 60px !important;width: 217px !important;" ></a>
