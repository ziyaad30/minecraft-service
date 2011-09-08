using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;

namespace MineCraft_Server
{
    public partial class MinecraftService : ServiceBase
    {
        public MinecraftService()
        {
            InitializeComponent();
        }

        BackgroundWorker bw = new BackgroundWorker();
        String[] ops;
        Process p = new Process();

        StreamWriter chat;
        StreamReader log;

        Dictionary<String, MinecraftUser> users = new Dictionary<String, MinecraftUser>();

        protected override void OnStart(string[] args)
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerAsync();
            if (File.Exists("ops.txt"))
            {
                ops = File.ReadAllLines("ops.txt");
            }
            else
            {
                ops = new String[0];
            }
        }

        protected override void OnStop()
        {
            chat.WriteLine("stop");
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {

            p.StartInfo.FileName = @"java.exe";
            p.StartInfo.Arguments = "-Xmx1024M -Xms1024M -jar server.jar nogui";
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WorkingDirectory = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
            p.Start();
            log = p.StandardError;
            chat = p.StandardInput;
            while (!p.HasExited)
            {
                String line;
                try
                {
                    line = log.ReadLine();
                }
                catch //the server is shutdown
                {
                    break;
                }

                if (line.Contains("logged in with entity id"))
                {
                    String username = line.Split(" ".ToCharArray())[3];
                    MinecraftUser u = new MinecraftUser();
                    u.username = username;
                    if (ops.Contains(username.ToLower()))
                    {
                        u.isAdmin = true;
                    }
                    users.Add(username, u);
                }

                if (line.Contains("lost connection: "))
                {
                    String username = line.Split(" ".ToCharArray())[3];
                    users.Remove(username);
                }

                if (line.Contains("<"))
                {
                    String part = line.Substring(line.IndexOf("<") + 1);
                    String username = part.Substring(0, part.IndexOf(">"));
                    String chatmessage = part.Substring(part.IndexOf(">") + 1);
                    users[username].chat.Add(chatmessage);
                }

                if (line.ToLower().Contains(": give"))
                {
                    String username = line.Split(" ".ToCharArray())[3].Replace(":", "");
                    String cheat = line.Split(":".ToCharArray())[3];
                    users[username].cheats.Add(cheat);
                }
                if (line.ToLower().Contains(": set time to"))
                {
                    String username = line.Split(" ".ToCharArray())[3].Replace(":", "");
                    String cheat = line.Split(":".ToCharArray())[3];
                    users[username].cheats.Add(cheat);
                }

                if (line.ToLower().Contains("issued server command: "))
                {
                    String username = line.Split(" ".ToCharArray())[3];
                    String command = line.Split(":".ToCharArray())[3].Trim();
                    String[] commandpart = command.Split(" ".ToCharArray());
                    switch (commandpart[0])
                    {
                        case "day":
                            chat.WriteLine("time set 0");
                            break;
                        case "night":
                            chat.WriteLine("time set 14000");
                            break;
                        case "showcheats":
                            if (commandpart.Length < 2)
                            {
                                chat.WriteLine("say Missing parameter: username");
                            }
                            else
                            {
                                foreach (String cheat in users[commandpart[1]].cheats)
                                {
                                    chat.WriteLine("say " + commandpart[1] + ": " + cheat);
                                }
                            }
                            break;
                    }
                }

            }

        }
    }
    public class MinecraftUser
    {
        public String username;
        public List<String> chat;
        public List<String> cheats;
        public bool isAdmin = false;
    }
}
