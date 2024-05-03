using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using BarryY.InGameConsole;

public class SpawnCube : MonoBehaviour
{
    public GameObject cubePrefab;

    private void Start(){
        UnityInGameConsole.AddCommand("spawnCube", SpawnNewCube, "Spawn a cube in scene world space");
    }

    [Preserve]
    private void SpawnNewCube(string[] arg){
        //arg should contain the value of x,y,z
        if(arg.Length < 3){
            Debug.LogWarning("Missing parameter(s) in spawnCube function");
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
