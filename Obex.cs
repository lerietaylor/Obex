using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static System.Environment;
using NativeWifi;
using System.IO;
using System.Collections.ObjectModel;
using Microsoft.Expression.Encoder.Devices;
using Microsoft.Expression.Encoder.ScreenCapture;

namespace NetEvent
{
    public class Obex
    {
        private List<string> winCmds = new List<string>() { };
        public List<string[]> winDirs = new List<string[]>() { };
        public WlanClient client = new WlanClient();
        public List<IfaceInfo> wifiIfaces = new List<IfaceInfo>() { };
        public List<string> recentDocs = new List<string>() { };

        public Obex()
        {
            //add some windows commands
            this.winCmds.Add("netstat -r");
            this.winCmds.Add("netsh dump");

            this.LoadWifiInfo();
            this.LoadWinDirs();
            this.LoadRecentDocs();
        }

        public void testVal(object o)
        {
            try
            {
                MessageBox.Show(o.ToString());
            }
            catch { };
        }

        public void AddWinCmd(string c)
        {
            this.winCmds.Add(c);
        }

        public void LoadWifiInfo()
        {
            foreach (WlanClient.WlanInterface iface in client.Interfaces)
            {
                IfaceInfo winfo = new IfaceInfo();
                winfo.descr = iface.InterfaceDescription;
                winfo.name = iface.InterfaceName;
                winfo.state = iface.InterfaceState.ToString();
                winfo.guid = iface.InterfaceGuid.ToString();
                winfo.iface = iface.NetworkInterface.Name;
                this.wifiIfaces.Add(winfo);
            }
        }

        public void LoadRecentDocs()
        {
            foreach (string f in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Recent)))
            {
                FileInfo fi = new FileInfo(f);
                this.recentDocs.Add(fi.Name);
            }

        }

        public void LoadWinDirs()
        {
            this.winDirs.Add(new string[] { "Admin Tools", Environment.GetFolderPath(SpecialFolder.AdminTools) });
            this.winDirs.Add(new string[] { "App Data", Environment.GetFolderPath(SpecialFolder.ApplicationData) });
            this.winDirs.Add(new string[] { "System", Environment.GetFolderPath(SpecialFolder.System) });
            this.winDirs.Add(new string[] { "System x86", Environment.GetFolderPath(SpecialFolder.SystemX86) });
            this.winDirs.Add(new string[] { "Windows", Environment.GetFolderPath(SpecialFolder.Windows) });
            this.winDirs.Add(new string[] { "User Profile", Environment.GetFolderPath(SpecialFolder.UserProfile) });
            this.winDirs.Add(new string[] { "Recent Files", Environment.GetFolderPath(SpecialFolder.Recent) });
        }

        public void recordScreen(string saveDir)
        {

            //try
            //{
            //    Collection<EncoderDevice> vidDevs = EncoderDevices.FindDevices(EncoderDeviceType.Video);
            //    LiveJob job = new LiveJob();
            //    EncoderDevice dev = vidDevs[1];

            //    LiveDeviceSource lds = job.AddDeviceSource(dev, null);
            //    LiveFileSource lfs = job.AddFileSource(@"C:\Users\BIGDADDY\Desktop\video.avi");

            //    lfs.JumpTo = lds;
            //    job.ActivateSource(lfs);

            //    job.OutputFormat = new MP4OutputFormat
            //    {
            //        VideoProfile = new MainH264VideoProfile() { SmoothStreaming = true }
            //    };

            //    job.StartEncoding();
            //    job.SendScriptCommand(new Microsoft.Expression.Encoder.Live.ScriptCommand("caption", "Streaming now!"));
            //} catch(Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}

            using (ScreenCaptureJob job = new ScreenCaptureJob())
            {
                // Sets output directory for job. filenames are generated with date and time stamps
                job.OutputPath = saveDir;

                // Sets capture rectangle. This defaults to full screen
                job.CaptureRectangle = Screen.PrimaryScreen.Bounds; //new Rectangle(100, 200, 250, 150);

                // set the duration of the capture this can be set or the job.Stop() method can be called to manually stop
                job.Duration = TimeSpan.FromMinutes(10);

                // Gets all video devices and selects a Microsoft webcam
                Collection<EncoderDevice> videoDevices = EncoderDevices.FindDevices(EncoderDeviceType.Video);
                foreach (EncoderDevice device in videoDevices)
                {
                    if (device.Name.Contains("web"))
                    {
                        job.VideoDeviceSource = device;
                        break;
                    }
                }

                // Iterates through all audio devices and selects the internal audio
                foreach (EncoderDevice device in job.AudioDeviceSources)
                {
                    if (device.Name.Contains("Speakers"))
                    {
                        job.AddAudioDeviceSource(device);
                        break;
                    }
                }

                while (true)
                {
                    // Starts capture
                    job.Start();
                }
            }
        }
    }

    public class IfaceInfo
    {
        public string descr;
        public string guid;
        public string state;
        public string name;
        public string iface;

        public IfaceInfo()
        {
            //
        }

        public void updateInfo(string d, string g, string s, string n, string i)
        {
            this.descr = d;
            this.guid = g;
            this.iface = i;
            this.name = n;
            this.state = s;
        }
    }
}
