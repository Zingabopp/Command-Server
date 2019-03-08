using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using CommandPluginLib;
using WebSocketSharp;


namespace Command_Server
{
    class Program
    {
        private static WebSocketSharp.WebSocket ws;
        private static int conAttempts = 10;
        private static List<ICommandPlugin> Plugins;
        public static string bsUrl = "ws://127.0.0.1:6558/socket";

        static void Main(string[] args)
        {
            
            Logger.LogLevel = LogLevel.Trace;
            Logger.ShortenSourceName = true;
            Logger.ShowTime = false;
            /*
            for (int i = 0; i <= 4; i++)
            {
                Logger.LogLevel = (LogLevel)i;
                Console.WriteLine($"LogLevel is {Logger.LogLevel.ToString()}");
                Logger.Trace("Trace test");
                Logger.Debug("Debug test");
                Logger.Info("Info test");
                Logger.Warning("Warning test");
                Logger.Error("Error test");
                Logger.Exception("Exception test", new Exception());
            }
            */
            Logger.Info("Starting Command-Server");
            Plugins = new List<ICommandPlugin>();
            Plugins = LoadPlugins();
            ws = new WebSocketSharp.WebSocket(bsUrl);
            ws.Log.Output = (_, __) => { }; // Disable error output
            ws.OnMessage += OnMessage;
            /*
            ws.OnClose += (s, e) => {
                Logger.Info($"Disconnected");
            };
            */
            TryConnect();
            Console.ReadKey(true);
            ws.Close();
            Console.ReadKey(true);

        }
        public event EventHandler Disconnected;
        static void SendMessage(object sender, MessageData msg)
        {
            Logger.Debug($"Sending message: {msg.ToString(3)}");
            ws.Send(msg.ToJSON());
        }

        static void TryConnect(int maxAttempts = -1)
        {
            Logger.Trace("Entering Command_Server TryConnect()");
            bool infAttempts = (maxAttempts == -1) ? true : false;
            conAttempts = 1;
            Timer timer = new Timer(1);
            timer.AutoReset = false;
            timer.Elapsed += (source, e) => {

                Logger.Info($"Attempting to connect to Beat Saber ({conAttempts})...");

                ws.Connect();
                if (!ws.IsAlive && ((conAttempts <= maxAttempts) || infAttempts))
                {
                    conAttempts++;
                    ((Timer) source).Interval = 3000;
                    ((Timer) source).Start();
                }
                if (ws.IsAlive)
                {
                    ws.OnClose += onDisconnect;
                    Logger.Info("Connected to Beat Saber!");
                }
            };
            timer.Start();
        }

        /// <summary>
        /// We received a message, pass it along to the appropriate plugin.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnMessage(object sender, MessageEventArgs e)
        {
            Logger.Trace($"Received message: {e.Data.ToMessageData().ToString(3)}");
            var msg = e.Data.ToMessageData();
            foreach (var p in Plugins)
            {
                if (msg.Destination == p.PluginName)
                {
                    Logger.Debug($"Passing message to {p.PluginName}:\n{msg.ToString(3)}");
                    p.OnMessage(sender, msg);
                }
            }
        }

        static void onDisconnect(object sender, CloseEventArgs e)
        {
            Logger.Info("Disconnected from Beat Saber");
            ws.OnClose -= onDisconnect;
            TryConnect();
        }

        static List<ICommandPlugin> LoadPlugins()
        {
            Logger.Info("Loading Plugins...");
            var plugins = new List<ICommandPlugin>();
            string pluginDirectory = Path.Combine(Environment.CurrentDirectory, "Plugins");
            if (!Directory.Exists(pluginDirectory)) return plugins;
            String[] files = Directory.GetFiles(pluginDirectory, "*.dll");
            foreach (var f in files)
            {
                plugins.AddRange(LoadPluginsFromFile(Path.Combine(pluginDirectory, f)));
            }
            foreach (var p in plugins)
            {
                p.Initialize();
                p.MessageReady += SendMessage;
            }
            return plugins;
        }

        private static IEnumerable<ICommandPlugin> LoadPluginsFromFile(string file)
        {
            List<ICommandPlugin> plugins = new List<ICommandPlugin>();
            if (!File.Exists(file) || !file.EndsWith(".dll", true, null))
                return plugins;

            try
            {
                Assembly assembly = Assembly.LoadFrom(file);
                foreach (Type t in assembly.GetTypes())
                {
                    if (t.GetInterface("ICommandPlugin") != null)
                    {
                        try
                        {
                            Logger.Trace($"Found correct type in {file}");
                            ICommandPlugin pluginInstance = Activator.CreateInstance(t) as ICommandPlugin;
                            //Logger.Info($"   Loaded {pluginInstance.PluginName} from {file.Substring(file.LastIndexOf(@"\") + 1)}");
                            if (pluginInstance != null)
                                plugins.Add(pluginInstance);
                            else
                                Logger.Error($"Unable to load plugin: {t.FullName} in {file} - Instance could not be created.");
                        }
                        catch (Exception ex)
                        {
                            Logger.Exception($"Unable to load plugin: {t.FullName} in {file}\n{ex.Message}\n{ex.StackTrace}", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception($"Unable to load from: {file}\n{ex.Message}\n{ex.StackTrace}", ex);
            }
            return plugins;
        }

    }
}
