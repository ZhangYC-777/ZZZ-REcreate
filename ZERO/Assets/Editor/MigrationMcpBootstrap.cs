using System;
using System.IO;
using MCPForUnity.Editor.Services;
using MCPForUnity.Editor.Services.Transport;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class MigrationMcpBootstrap
{
    static MigrationMcpBootstrap()
    {
        // Keep the migration session reachable while installing packages and reloading scripts.
        if (File.Exists("MigrationReview/connect-mcp.request"))
            EditorApplication.delayCall += Connect;
    }

    [MenuItem("Tools/Project Migration/Connect Local MCP")]
    public static async void Connect()
    {
        try
        {
            if (!MCPServiceLocator.TransportManager.IsRunning(TransportMode.Http) &&
                !await MCPServiceLocator.TransportManager.StartAsync(TransportMode.Http)) return;
            File.WriteAllText("MigrationReview/mcp-connected.txt", Application.unityVersion);
            Debug.Log("ZERO connected to the existing local Unity MCP server.");
        }
        catch (Exception exception) { Debug.LogException(exception); }
    }
}
