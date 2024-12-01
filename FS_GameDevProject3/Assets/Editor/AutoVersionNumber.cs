
using System;

using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;
using UnityEngine;



class AutoVersion
{
    const string VERSION_FILE_PATH = "Assets/Editor/VersionNumber.txt";

    public static void UpdateVersion()
    {
        // load version file
        TextAsset versionFile = AssetDatabase.LoadAssetAtPath<TextAsset>(VERSION_FILE_PATH);

        if (versionFile != null)
        {
            // determine the new version number
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;

            string versionFormat = $"{year}.{month}.{day}";

            // write the new file and refresh the database
            File.WriteAllText(AssetDatabase.GetAssetPath(versionFile), versionFormat);
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogError(VERSION_FILE_PATH + " does not exist!");
        }
    }
}


/* ----------------------------------------------------- SAVING */
public class ProjectSaveListener : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFromAssetPaths)
    {
        // generate a new auto version file
        if (imported.Length > 0)
        {
            AutoVersion.UpdateVersion();
        }
    }
}


/* ---------------------------------------------------- BUILDING */
public class BuildProcessor : IPreprocessBuildWithReport
{
    // make sure this runs first
    public int callbackOrder => 0;

    // generate a new auto version file
    public void OnPreprocessBuild(BuildReport report)
    {
        AutoVersion.UpdateVersion();
    }
}

    
