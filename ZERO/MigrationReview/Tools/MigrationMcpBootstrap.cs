using System;
using System.IO;
using MCPForUnity.Editor.Services;
using MCPForUnity.Editor.Services.Transport;
using UnityEditor;
using UnityEngine;

// One-time connection to the same local MCP server used by the source project.
[InitializeOnLoad]
public static class MigrationMcpBootstrap
{
    private const string Request = "MigrationReview/connect-mcp.request";

    static MigrationMcpBootstrap()
    {
        if (File.Exists(Request)) EditorApplication.delayCall += Connect;
    }

    private static async void Connect()
    {
        try
        {
            if (!await MCPServiceLocator.TransportManager.StartAsync(TransportMode.Http)) return;
            File.Move(Request, "MigrationReview/mcp-connected.txt");
            Debug.Log("ZERO connected to the existing local Unity MCP server.");
        }
        catch (Exception exception) { Debug.LogException(exception); }
    }
}
