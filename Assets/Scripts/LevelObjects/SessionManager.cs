using UnityEngine;
using UnityEngine.Events;

using System; 
using System.IO; 
using System.Collections.Generic;
using System.Collections;

public class SessionManager : MonoBehaviour
{
    static SessionManager _main;
    public static SessionManager main { get {
            if(_main == null)
                _main = new GameObject("sceneManager").AddComponent<SessionManager>();
            
            return _main;
        } }
    
    public System.DateTime SessionStart = DateTime.Now;
    public int KillStreak = 0;

    public UnityEvent<string> OnResult = new();

    private void Awake() {
        _main = this;
    }

    public void SaveResults() {
        OnResult.Invoke($"{Environment.UserName}\nTime: {DateTime.Now - SessionStart}\nKilled {KillStreak}");

        using(StreamWriter sw = new StreamWriter(File.Create(Application.persistentDataPath + "\\records.log"))){
            sw.WriteLine($"{Environment.UserName} - {DateTime.Now - SessionStart} - {KillStreak}");
        }
        
    }
}
