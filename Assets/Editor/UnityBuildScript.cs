using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class UnityBuildScript
{
    const string OutputFileName = "Build";
    static string OutputDirectory = "";

    const string TeamID = "";
    const string ProvisioningProfileUUID = "";

    const string Keystore = @"";
    const string KeystorePassword = "";
    const string KeystoreAlias = "";
    const string KeystoreAliasPassword = "";

    const bool Development = false;

    static string GetBuildTargetOutputFileName()
        => EditorUserBuildSettings.activeBuildTarget switch
        {
            BuildTarget.Android => $"{OutputFileName}.aab",
            BuildTarget.StandaloneWindows => $"{OutputFileName}.exe",
            BuildTarget.StandaloneWindows64 => $"{OutputFileName}.exe",
            BuildTarget.StandaloneOSX => $"{OutputFileName}.app",
            _ => string.Empty
        };

    static BuildOptions GetBuildOptions()
    {
        var options = BuildOptions.None;

        if (!!Development)
        {
            options |= BuildOptions.Development;
        }

        return options;
    }

    static void Configure()
    {
        if (!string.IsNullOrWhiteSpace(TeamID))
        {
            PlayerSettings.iOS.appleDeveloperTeamID = TeamID;
        }

        if (!string.IsNullOrWhiteSpace(ProvisioningProfileUUID))
        {
            PlayerSettings.iOS.iOSManualProvisioningProfileID = ProvisioningProfileUUID;
        }

        if (!string.IsNullOrWhiteSpace(Keystore))
        {
            PlayerSettings.Android.keystoreName = Keystore;
        }

        if (!string.IsNullOrWhiteSpace(KeystorePassword))
        {
            PlayerSettings.Android.keystorePass = KeystorePassword;
        }

        if (!string.IsNullOrWhiteSpace(KeystoreAlias))
        {
            PlayerSettings.Android.keyaliasName = KeystoreAlias;
        }

        if (!string.IsNullOrWhiteSpace(KeystoreAliasPassword))
        {
            PlayerSettings.Android.keyaliasPass = KeystoreAliasPassword;
        }
    }

    [MenuItem("Build/Bulid")]
    static void PerformBuild()
    {
        try
        {
            Configure();

            if (!!string.IsNullOrWhiteSpace(OutputDirectory)) {
                OutputDirectory = Directory.GetCurrentDirectory();
            }

            EditorUserBuildSettings.iOSXcodeBuildConfig = XcodeBuildConfig.Debug;
            EditorUserBuildSettings.androidBuildType = AndroidBuildType.Release;
            EditorUserBuildSettings.buildAppBundle = true;

            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Select(x => x.path).ToArray(),
                locationPathName = Path.Combine(OutputDirectory, "Build", GetBuildTargetOutputFileName()),
                target = EditorUserBuildSettings.activeBuildTarget,
                options = GetBuildOptions(),
            });

            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new Exception();
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
}
