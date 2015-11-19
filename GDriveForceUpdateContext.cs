using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GDriveForceUpdate
{
    public class GDriveForceUpdateContext : ApplicationContext
    {
        public static Boolean running = false;
        public static Boolean paused = false;

        private System.ComponentModel.IContainer mComponents;
        private NotifyIcon mNotifyIcon;
        private ContextMenuStrip mContextMenu;
        private ToolStripMenuItem mDisplayForm;
        private ToolStripMenuItem pauseUpdate;
        private ToolStripMenuItem mExitApplication;

        BackgroundWorker m_oWorker;

        public GDriveForceUpdateContext()
        {
            running = true;

            mComponents = new System.ComponentModel.Container();

            mNotifyIcon = new NotifyIcon(this.mComponents);
            //System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            //Stream myStream = myAssembly.GetManifestResourceStream("icon.png");
            Stream s = GetType().Assembly.GetManifestResourceStream("GDriveForceUpdate.icon.png");
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(s);
            s.Close();
            mNotifyIcon.Icon = System.Drawing.Icon.FromHandle(bmp.GetHicon());
            mNotifyIcon.Text = "GDrive Force Update";
            mNotifyIcon.Visible = true;

            mContextMenu = new ContextMenuStrip();
            mDisplayForm = new ToolStripMenuItem();
            mExitApplication = new ToolStripMenuItem();
            pauseUpdate = new ToolStripMenuItem();

            mNotifyIcon.ContextMenuStrip = mContextMenu;

            mDisplayForm.Text = "About...";
            mDisplayForm.Click += new EventHandler(mDisplayForm_Click);
            mContextMenu.Items.Add(mDisplayForm);

            pauseUpdate.Text = "Pause";
            pauseUpdate.Click += new EventHandler(mPauseUpdate_Click);
            mContextMenu.Items.Add(pauseUpdate);

            mExitApplication.Text = "Exit";
            mExitApplication.Click += new EventHandler(mExitApplication_Click);
            mContextMenu.Items.Add(mExitApplication);

            

            m_oWorker = new BackgroundWorker();
            m_oWorker.DoWork += new DoWorkEventHandler(m_oWorker_DoWork);
            //m_oWorker.ProgressChanged += new ProgressChangedEventHandler(m_oWorker_ProgressChanged);
            //m_oWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(m_oWorker_RunWorkerCompleted);
            m_oWorker.WorkerReportsProgress = true;
            m_oWorker.WorkerSupportsCancellation = true;
            m_oWorker.RunWorkerAsync();
        }

        void m_oWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            String taskKillCmd = "taskkill";
            String taskKillArgs = "/F /IM googledrivesync.exe";
            String taskStartCmd = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\Google\Drive\googledrivesync.exe";

            Process taskKillProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = taskKillCmd,
                    Arguments = taskKillArgs,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    Verb = "runas"
                }
            };


            Process taskStartProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = taskStartCmd,
                    //Arguments = "",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    //WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    WorkingDirectory = @"C:\Program Files (x86)\Google\Drive\",
                    Verb = "runas"
                }
            };

            do
            {
                if (paused == false)
                {
                    taskKillProcess.Start();
                    taskKillProcess.WaitForExit();

                    taskStartProcess.Start();
                }

                Thread.Sleep(3 * 60 * 1000);

            } while (running) ;
        }

        void mDisplayForm_Click(object sender, EventArgs e)
        {
            new GDriveForceUpdateForm().Show();
        }

        void mPauseUpdate_Click(object sender, EventArgs e)
        {
            if (paused == true)
            {
                pauseUpdate.Text = "Pause";
                paused = false;
            }
            else
            {
                pauseUpdate.Text = "Activate";
                paused = true;
            }
            
        }

        void mExitApplication_Click(object sender, EventArgs e)
        {
            running = false;
            ExitThreadCore();
        }

        protected override void ExitThreadCore()
        {
            base.ExitThreadCore();
        }
    }
}
