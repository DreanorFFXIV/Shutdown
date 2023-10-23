using System;
using System.Numerics;
using System.Runtime.InteropServices.JavaScript;
using Dalamud.Interface.Windowing;
using ECommons.Logging;
using ImGuiNET;

namespace Shutdown.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;

    public ConfigWindow(Plugin plugin) : base(
        "Config",
        ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.Size = new Vector2(232, 115);
        this.SizeCondition = ImGuiCond.Always;

        this.Configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void Draw()
    {
        var executeTime = this.Configuration.ExecuteTime;
        var executeEnabled = this.Configuration.ExecuteEnabled;
        
        DateTime selectedDateTime = DateTime.MinValue;
        string dateInput = executeTime.ToString("dd.MM.yyyy HH:mm:ss");
        
        if (ImGui.Checkbox("Execute", ref executeEnabled))
        {
            if (DateTime.Now >= Configuration.ExecuteTime)
            {
                executeEnabled = false;
            }
            this.Configuration.ExecuteEnabled = executeEnabled;
            this.Configuration.Save();
        }

        if (ImGui.InputText("Date", ref dateInput, 64))
        {
            if (DateTime.TryParse(dateInput, out DateTime parsedDate))
            {
                selectedDateTime = parsedDate;
            }

            this.Configuration.ExecuteTime = selectedDateTime;
        }
    }
}
