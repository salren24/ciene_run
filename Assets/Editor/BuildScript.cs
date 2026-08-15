using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildScript
{
    public static void BuildAndroidAPK()
    {
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.DefaultCompany.cienerun");
        EditorUserBuildSettings.buildAppBundle = false;

        string[] scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();

        string outputDir = "Builds/Android";
        Directory.CreateDirectory(outputDir);
        string outputPath = Path.Combine(outputDir, "ciene_run.apk");

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outputPath,
            target = BuildTarget.Android,
            targetGroup = BuildTargetGroup.Android,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        Debug.Log("[BUILD] result=" + summary.result +
                   " totalErrors=" + summary.totalErrors +
                   " totalWarnings=" + summary.totalWarnings +
                   " outputPath=" + summary.outputPath +
                   " sizeBytes=" + summary.totalSize);

        if (summary.result != BuildResult.Succeeded)
            EditorApplication.Exit(1);
        else
            EditorApplication.Exit(0);
    }
}
