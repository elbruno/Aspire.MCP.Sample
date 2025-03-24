﻿using ModelContextProtocol.Server;
using System.ComponentModel;

namespace McpSample.AspNetCoreSseServer;

[McpToolType]
public static class Jokes
{
    [McpTool, Description("Returns a joke about a specific topic")]
    public static string GetJoke(string topic)
    {
        Console.WriteLine($"Function get joke with topic: {topic}");
        var message = $"I don't do jokes about {topic}.";
        Console.WriteLine("Function report: " + message);
        Console.WriteLine($"Function End get joke with topic: {topic}");
        return message;
    }
}
