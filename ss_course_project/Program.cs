﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net.Mqtt;

using ss_course_project.services;
using ss_course_project.gui.Forms;

namespace ss_course_project.gui
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            form = new MainForm(controller);
            lost_form = new ConnectionLostForm();

            form.Show();

            Start();
            
            form.FormClosed += m_mainForm_FormClosed;

            Application.Run();
        }

        static void exit(bool isForced = false)
        {
            var result = MessageBox.Show(
                "Save settings?"
                , "Exit dialog"
                , MessageBoxButtons.YesNo
                , MessageBoxIcon.Exclamation
                , isForced ? MessageBoxDefaultButton.Button2 : MessageBoxDefaultButton.Button1
                );

            if (result == DialogResult.Yes)
            {
                controller.SaveSettings();
            }

            controller.Dispose();
            Application.ExitThread();
        }

        static void m_mainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            exit();
        }
        
        static async void Start()
        {
            while (true)
            {
                try
                {
                    await controller.Init();
                    await controller.ConnectAll();
                }
                catch (MqttClientException)
                {
                    DialogResult result = lost_form.ShowDialog();

                    controller.Dispose();

                    if (result == DialogResult.Abort)
                    {
                        exit(isForced: true);
                    }
                    else
                    {
                        continue;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    exit(isForced: true);
                }

                break;
            }
            
            form.FillByController();
        }

        static MainForm form;
        static Controller controller = new Controller();
        static Form lost_form;
    }
}
