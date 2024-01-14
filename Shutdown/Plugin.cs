using System;
using System.Diagnostics;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using Dalamud.Plugin.Services;
using Shutdown.Windows;

namespace Shutdown
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Shutdown";
        private const string CommandName = "/psd";

        private DalamudPluginInterface PluginInterface { get; init; }
        private ICommandManager CommandManager { get; init; }
        private IFramework framework { get; init; }
        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("Shutdown");

        private ConfigWindow ConfigWindow { get; init; }

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] ICommandManager commandManager,
            [RequiredVersion("1.0")] IFramework framework)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);

            ConfigWindow = new ConfigWindow(this);
            
            WindowSystem.AddWindow(ConfigWindow);

            this.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "A useful message to display in /xlhelp"
            });

            framework.Update += ShutdownGame;
            
            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();
            
            framework.Update -= ShutdownGame;
            
            this.CommandManager.RemoveHandler(CommandName);
            ConfigWindow.Dispose();
        }

        private void OnCommand(string command, string args)
        {
            // in response to the slash command, just display our main ui
            DrawConfigUI();
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        private void ShutdownGame(object _)
        {
            if (Configuration.ExecuteEnabled 
                && !ConfigWindow.IsOpen 
                && Configuration.ExecuteTime > DateTime.MinValue
                && DateTime.Now >= Configuration.ExecuteTime)
            {
                PluginLog.Log("shutdown");
                Configuration.ExecuteEnabled = false;
                Configuration.Save();
                
                CommandManager.ProcessCommand("/pyes toggle");

                //gracefully close game
                Process.Start("shutdown", "/s /t 60");
                Process.GetCurrentProcess().CloseMainWindow();
                
                CommandManager.ProcessCommand("/pyes toggle");
            }
        }
        
        public void DrawConfigUI()
        {
            ConfigWindow.IsOpen = true;
        }
    }
}
