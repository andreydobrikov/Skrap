using UnityEngine;
using System.Collections;
using CorruptedSmileStudio.INI;

/// <summary>
/// An example of how the system works.
/// </summary>
public class HUD : MonoBehaviour
{
    INIUnity config;

    string mainText;
    string secondaryText;
    int firstNumber;
    float secondNumber;
    Color HUDColor;

    void Start()
    {
        // Loads the INI file then converts the INIContent to INIUnity.
        try
        {
            config = INIFile.Read(Application.dataPath + "/Packages/INISystem/Example/Script/example.ini").ToINIUnity();
        }
        catch (System.IO.FileNotFoundException ex)
        {
            INIFile.Write(ex.FileName, config);
        }
        // Gets the mainText from the ini file, if no entry is found return the default text.
        mainText = config.GetString("mainText", "General", "This is the default for mainText");
        // Gets the secondaryText from the ini file, if no entry is found return the default text.
        secondaryText = config.GetString("secondaryText", "General", "This is the default for secondaryText");
        // Gets the firstNumber from the ini file, if no entry is found, the default text is returned.
        firstNumber = config.GetInt("firstNumber", "General", 12);
        // Gets the secondNumber from the ini file, if no entry is found, the default text is returned.
        secondNumber = config.GetFloat("secondNumber", "General", 18.55f);

        // Gets Unity specific datatypes handled by the INIUnity class.
        HUDColor = config.GetColor("HUDColor", "Graphics", Color.white);
        Debug.Log(config.GetVector2("mapSize", "FirstLevel", new Vector2(10, 10)));
        Debug.Log(config.GetVector3("startLocation", "FirstLevel", new Vector3(10, 2, 10)));
        Debug.Log(config.GetQuaternion("startRotation", "FirstLevel", new Quaternion(0, 90, 0, 0)));
		foreach(var entry in config.elements)
		{
			Debug.Log("Element found: " + "Key: " + entry.Key + " Value: " + entry.Value);
		}
    }

    void OnGUI()
    {
        GUI.color = HUDColor;
        // Displays the text and numbers. The text can be edited and saved
        mainText = GUILayout.TextField(mainText);
        secondaryText = GUILayout.TextField(secondaryText);
        GUILayout.Label(firstNumber.ToString());
        GUILayout.Label(secondNumber.ToString());
        // Saves the text and numbers to file
        if (GUILayout.Button("save"))
        {
            Save();
        }
    }

    void Save()
    {
        // Makes changes to the elements.
        // These are the standard datatypes supported by INIContent
        config.Change("mainText", mainText, "General");
        config.Change("secondaryText", secondaryText, "General");
        config.Change("firstNumber", firstNumber, "General");
        config.Change("secondNumber", secondNumber, "General");
        // Makes changes to the elements.
        // These are the Unity specific datatypes supported by INIUnity
        config.Change("HUDColor", Color.green, "Graphics");
        config.Change("mapSize", new Vector2(5, 15), "FirstLevel");
        config.Change("startLocation", new Vector3(1, 2, 8), "FirstLevel");
        config.Change("startRotation", new Quaternion(0, 0, 0, 0), "FirstLevel");

        // Saves the file, if save is successful print a message, else print a warning.
        if (INIFile.Write(Application.dataPath + "/INISystem/Example/Script/example.ini", (INIContent)config))
            Debug.Log("File saved at: " + Application.dataPath + "/INISystem/Example/Script/example.ini");
        else
            Debug.LogWarning("Could not save file at: " + Application.dataPath + "/INISystem/Example/Script/example.ini");
    }
}