using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WindowsGSM.Functions;
using WindowsGSM.GameServer.Engine;
using WindowsGSM.GameServer.Query;


namespace WindowsGSM.Plugins
{
    public class Valheim : SteamCMDAgent
    {
        internal const int CTRL_C_EVENT = 0;
        [DllImport("kernel32.dll")]
        internal static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool AttachConsole(uint dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern bool FreeConsole();
        [DllImport("kernel32.dll")]
        static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate HandlerRoutine, bool Add); 
        delegate Boolean ConsoleCtrlDelegate(uint CtrlType);

        // - Plugin Details
        public Plugin Plugin = new Plugin
        {
            name = "WindowsGSM.Valheim", // WindowsGSM.XXXX
            author = "PsymoN",
            description = "A Fork of kessef WindowsGSM plugin version for supporting Valheim Dedicated Server",
            version = "1.6.1",
            url = "https://github.com/diegovsantos/WindowsGSM.Valheim", // Github repository link (Best practice)
            color = "#34c9eb" // Color Hex
        };

        // - Settings properties for SteamCMD installer
        public override bool loginAnonymous => true;
        public override string AppId => "896660"; // Game server appId

        // - Standard Constructor and properties
        public Valheim(ServerConfig serverData) : base(serverData) => base.serverData = serverData;


        // - Game server Fixed variables
        public override string StartPath => @"valheim_server.exe"; // Game server start path
        public string FullName = "Valheim Dedicated Server"; // Game server FullName
        public bool AllowsEmbedConsole = true;  // Does this server support output redirect?
        public int PortIncrements = 10; // This tells WindowsGSM how many ports should skip after installation
        public object QueryMethod = new A2S(); // Query method should be use on current server type. Accepted value: null or new A2S() or new FIVEM() or new UT3()


        // - Game server default values
        public string Port = "2456"; // Default port
        public string QueryPort = "2457"; // Default query port
        public string Defaultmap = "Dedicated"; // Default map name
        public string Maxplayers = "10"; // Default maxplayers
        public string Additional = "-password \"CHANGE_ME\" -savedir \"c:\valheim\" -Public 1"; // Additional server start parameter


        // - Create a default cfg for the game server after installation
        public async void CreateServerCFG()
        {
            //No config file seems
        }

        // - Start server function, return its Process to WindowsGSM
        public async Task<Process> Start()
        {

            string shipExePath = Functions.ServerPath.GetServersServerFiles(serverData.ServerID, StartPath);
            if (!File.Exists(shipExePath))
            {
                Error = $"{Path.GetFileName(shipExePath)} not found ({shipExePath})";
                return null;
            }

            // Prepare start parameter
            var param = new StringBuilder();
            param.Append("-batchmode -nographics -crossplay");
            param.Append(string.IsNullOrWhiteSpace(serverData.ServerPort) ? string.Empty : $" -port {serverData.ServerPort}");
            param.Append(string.IsNullOrWhiteSpace(serverData.ServerName) ? string.Empty : $" -name \"{serverData.ServerName}\"");
            param.Append(string.IsNullOrWhiteSpace(serverData.ServerMap) ? string.Empty : $" -world \"{serverData.ServerMap}\"");
            param.Append(string.IsNullOrWhiteSpace(serverData.ServerParam) ? string.Empty : $" {serverData.ServerParam}");

            // Prepare Process
            var p = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = ServerPath.GetServersServerFiles(serverData.ServerID),
                    FileName = shipExePath,
                    Arguments = param.ToString(),
                    WindowStyle = ProcessWindowStyle.Minimized,
                    UseShellExecute = false
                },
                EnableRaisingEvents = true
            };

            // Set up Redirect Input and Output to WindowsGSM Console if EmbedConsole is on
            if (AllowsEmbedConsole)
            {
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                var serverConsole = new ServerConsole(serverData.ServerID);
                p.OutputDataReceived += serverConsole.AddOutput;
                p.ErrorDataReceived += serverConsole.AddOutput;

                // Start Process
                try
                {
                    p.Start();
                }
                catch (Exception e)
                {
                    Error = e.Message;
                    return null; // return null if fail to start
                }

                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                return p;
            }

            // Start Process
            try
            {
                p.Start();
                return p;
            }
            catch (Exception e)
            {
                Error = e.Message;
                return null; // return null if fail to start
            }
        }

        // - Stop server function
        public async Task Stop(Process p)
        {
            await Task.Run(() =>
            {
                if (AttachConsole((uint)p.Id))
                {
                    SetConsoleCtrlHandler(null, true);
                    try
                    {
                        if (!GenerateConsoleCtrlEvent(CTRL_C_EVENT, 0))
                            return;
                        p.WaitForExit(10000);
                    }
                    finally
                    {
                        SetConsoleCtrlHandler(null, false);
                        FreeConsole();
                    }
                    return;
                }
            });
        }

        public async Task<Process> Update(bool validate = false, string custom = null)
        {
            var (p, error) = await Installer.SteamCMD.UpdateEx(serverData.ServerID, AppId, validate, custom: custom, loginAnonymous: loginAnonymous);
            Error = error;
            await Task.Run(() => { p.WaitForExit(); });
            return p;
        }
    }
}