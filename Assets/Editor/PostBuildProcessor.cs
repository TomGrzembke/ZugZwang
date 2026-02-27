using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Debug = UnityEngine.Debug;

public class PostBuildProcessor : IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.platform != BuildTarget.WebGL) return;

        var zipAndUploadBatchScriptPath = Directory.GetCurrentDirectory() + "\\Build\\automated-webgl-upload.bat";
        var zipBatchScriptPath = Directory.GetCurrentDirectory() + "\\Build\\zip-folder.bat";

        var config = AssetDatabase.LoadAssetAtPath<BuildConfig>("Assets/BuildSettings/BuildConfig.asset");

        if (config == null)
        {
            Debug.LogError("BuildConfig nicht gefunden!");
            return;
        }

        if (config.shouldUploadToItch && File.Exists(zipAndUploadBatchScriptPath))
        {
            var zipAndUploadProcess = new ProcessStartInfo(zipAndUploadBatchScriptPath)
            {
                UseShellExecute = true,
                CreateNoWindow = false
            };
            Process.Start(zipAndUploadProcess);
        }
        else if (!config.shouldUploadToItch && File.Exists(zipBatchScriptPath))
        {
            var zipProcess = new ProcessStartInfo(zipBatchScriptPath)
            {
                UseShellExecute = true,
                CreateNoWindow = false
            };
            Process.Start(zipProcess);
        }
    }
}