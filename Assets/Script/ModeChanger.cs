using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    main = 0, partialVideoSeeThrough = 1, mirroring = 2, VRController = 3, lipMotion = 4
}

public class ModeChanger : MonoBehaviour
{

    #region Serialized Priveate Field
    [SerializeField] private List<string> sceneNameList = new List<string>();
    
    #endregion



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Public Function

    public void SceneChagne(int sceneType)
    {
        SceneManager.LoadScene(sceneNameList[sceneType]);
    }
    public void SceneChagne(SceneType sceneType)
    {
        SceneManager.LoadScene(sceneNameList[(int)sceneType]);
    }
    #endregion
}
