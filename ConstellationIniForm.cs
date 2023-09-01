using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConstellationIni
{
    /// <summary>
    /// 
    /// make ico from png with ffmpeg
    /// https://superuser.com/a/679559
    /// 
    /// 
    /// </summary>
    public partial class ConstellationIniForm : Form
    {
        //%USERPROFILE%\Documents\My Games\Starfield
        static string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        static string starfieldCustomIniPath = myDocumentsPath + "\\MyGames" + "\\StarfieldCustom.ini";
        
        static IniFile ini;

        //Sections
        static readonly string CAMERA_SECTION = "Camera";
        static readonly string FLIGHT_CAMERA_SECTION = "FlightCamera";
        static readonly string DISPLAY_SECTION = "Display";
        static readonly string CONTROLS_SECTION = "Controls";

        //Keys
        static readonly string FIRST_PERSON_FOV = "fFPWorldFOV";
        static readonly string THIRD_PERSON_FOV = "fTPWorldFOV";
        static readonly string FLIGHT_CAMERA_FOV = "fFlightCameraFOV";

        //ErrorCodes
        static readonly int EXIT_SUCCESS = 0; //NOT an error code ;)
        static readonly int FILE_NOT_FOUND = 1;
        static readonly int MISSING_INI_SECTION = 100;
        static readonly int MISSING_INI_KEY = 101;

        public ConstellationIniForm()
        {
            InitializeComponent();

            if (!File.Exists(starfieldCustomIniPath))
            {
                string errorMessage = "You need to launch Starfield to generate the StarfieldCustom.ini file found in " + myDocumentsPath + "\\MyGames\\";
                MessageBox.Show(errorMessage,
                                "Error finding StarfieldCustom.ini",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                Environment.Exit(FILE_NOT_FOUND);
            }

            LoadIniFile();
            //textBox_Log.Text = ""; //hack to clear on init of value changed events firing saying the values have been updated.
            Log("Ini File Loaded.");

        }

        private void Log(string message, bool includeNewLine = true)
        {
            message = DateTime.Now.ToString() + ": " + message;
            if (includeNewLine)
            {
                message += Environment.NewLine;
            }
            //append new text in front of "old" Log text
            //textBox_Log.Text = message + textBox_Log.Text;

            //idea to avoid the Log being over cluttered
            //if (textBox_Log.Lines.Length > 1000)
            //{
            //    textBox_Log.Lines = textBox_Log.Lines[0...800];
            //}

        }


        private void LoadIniFile()
        {
            try
            {
                //for testing: see 
                ini = new IniFile(starfieldCustomIniPath);

                //if (!ini.KeyExists(FIRST_PERSON_FOV, CAMERA_SECTION))
                //{
                //    Environment.Exit(1);
                //}

                //if (!ini.KeyExists(THIRD_PERSON_FOV, CAMERA_SECTION))
                //{
                //    Environment.Exit(1);
                //}

                int firstFOV = (int)double.Parse(ini.Read(FIRST_PERSON_FOV, CAMERA_SECTION));
                trackBar1.Value = firstFOV;
                numericUpDown1.Value = trackBar1.Value;

                int thirdFOV = (int)double.Parse(ini.Read(THIRD_PERSON_FOV, CAMERA_SECTION));
                trackBar2.Value = thirdFOV;
                numericUpDown2.Value = trackBar2.Value;

                int flightFOV = (int)double.Parse(ini.Read(FLIGHT_CAMERA_FOV, FLIGHT_CAMERA_SECTION));
                trackBar3.Value = flightFOV;
                numericUpDown3.Value = flightFOV;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error parsing StarfieldCustom.ini" + Environment.NewLine + ex.Message);
                Environment.Exit(1);
            }
        }


        //Explicitly save the Ini File
        //private void SaveIniFile(object sender, EventArgs e)
        //{
        //    ini.Write(FIRST_PERSON_FOV, trackBar1.Value.ToString(), CAMERA_SECTION);
        //    ini.Write(THIRD_PERSON_FOV, trackBar2.Value.ToString(), CAMERA_SECTION);
        //}

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            numericUpDown1.Value = trackBar1.Value;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            numericUpDown2.Value = trackBar2.Value;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            numericUpDown3.Value = trackBar3.Value;

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            trackBar1.Value = (int)numericUpDown1.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            trackBar2.Value = (int)numericUpDown2.Value;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            trackBar3.Value = (int)numericUpDown3.Value;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            ini.Write(FIRST_PERSON_FOV, trackBar1.Value.ToString(), CAMERA_SECTION);
            Log(FIRST_PERSON_FOV + " updated to " + trackBar1.Value + ".");
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            ini.Write(THIRD_PERSON_FOV, trackBar2.Value.ToString(), CAMERA_SECTION);
            Log(THIRD_PERSON_FOV + " updated to " + trackBar2.Value + ".");

        }

        private void trackBar3_ValueChanged(object sender, EventArgs e)
        {
            ini.Write(FLIGHT_CAMERA_FOV, trackBar3.Value.ToString(), FLIGHT_CAMERA_SECTION);
            Log(FLIGHT_CAMERA_FOV + " updated to " + trackBar3.Value + ".");

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/insane0hflex/ConstellationIni");
        }

        private void button_OpenIniFolder_Click(object sender, EventArgs e)
        {
            Process.Start(starfieldCustomIniPath.Replace("StarfieldCustom.ini", ""));
        }


        private void button_CreateIniBackup_Click(object sender, EventArgs e)
        {
            try
            {
                string dateTimeStamp = DateTime.Now.ToString("M/d/yyyy HH:mm")
                    .Replace("/", "_")
                    .Replace(" ", "_")
                    .Replace(":", "_");

                string backupName = starfieldCustomIniPath + "." + dateTimeStamp + ".bak";
                File.Copy(starfieldCustomIniPath, backupName);
                MessageBox.Show("Created: " + backupName);
            }
            catch (Exception ex)
            {
                string errorMessage = "Could not create a backup of " + starfieldCustomIniPath
                    + Environment.NewLine + ex.Message;
                MessageBox.Show(errorMessage);
            }
        }

        private void checkBox_MouseAcceleration_CheckedChanged(object sender, EventArgs e)
        {
            //bMouseAcceleration=0
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //foreach control in form
            //write to ini
            
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(EXIT_SUCCESS);
        }
    }


}
