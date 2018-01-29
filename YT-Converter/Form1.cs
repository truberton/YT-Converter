﻿using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace YT_Converter
{
    public partial class Form1 : Form
    {
        public string path;
        public string link;
        public string formaat;
        public string kõik;
        public int playlistVideoNumber;
        public bool esimeneValueOlemas = false;
        public string TXTFail { get; set; }

        public Form1()
        {
            InitializeComponent();
            formatBox.Items.Add("m4a");
            formatBox.Items.Add("mp3");
            formatBox.Items.Add("wav");
            formatBox.Items.Add("mp4");
        }
        private void linkBox_TextChanged(object sender, EventArgs e)
        {
            link = linkBox.Text;
        }

        private void formatBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void chooseDirectory_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                path = fbd.SelectedPath;
            }
            File.WriteAllText("Directory.txt", path);
        }

        private void checkKonsool_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void tõmba_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(path) && !File.Exists("Directory.txt"))
            {
                path = Directory.GetCurrentDirectory();
            }
            else if (File.Exists("Directory.txt"))
            {
                path = File.ReadAllText("Directory.txt");
                if (!Directory.Exists(path))
                {
                    path = Directory.GetCurrentDirectory();
                }
            }
            //Alumine teeb kindlaks, et link ja formaat või txtfail ja formaat on sisestatud
            if (Convert.ToString(formatBox.SelectedItem) != "" && link != "" || !string.IsNullOrWhiteSpace(TXTFail) && Convert.ToString(formatBox.SelectedItem) != "")
            {
                Process convert = new Process();
                SetStartInfo set = new SetStartInfo();
                FileExists fileExists = new FileExists();

                formaat = Convert.ToString(formatBox.SelectedItem);
                convert.StartInfo.FileName = "youtube-dl.exe";
                var failiNimi = set.SetArgument(convert, formaat, path, link, TXTFail);

                if (!fileExists.Exist(failiNimi, formaat, path))
                {
                    if (!checkKonsool.Checked)
                    {
                        StartConverter startConverter1 = new StartConverter();
                        if (!string.IsNullOrWhiteSpace(TXTFail))
                        {
                            startConverter1.StartTXTFile(convert, linkBox, progressBar1, link, TXTFail);
                        }
                        else if (link.Contains("playlist"))
                        {
                            startConverter1.StartPlaylist(convert, linkBox, progressBar1, link);
                        }
                        else
                        {
                            startConverter1.StartLink(convert, linkBox, progressBar1, link);
                        }
                    }
                    else
                    {
                        convert.Start();
                        convert.WaitForExit();
                    }
                    if (!string.IsNullOrWhiteSpace(TXTFail))
                    {
                        MessageBox.Show("Fail lõpetas tõmbamise");
                        progressBar1.Value = 0;
                        linkBox.Text = "";
                    }
                    else if (link.Contains("playlist"))
                    {
                        MessageBox.Show("Fail lõpetas tõmbamise");
                        progressBar1.Value = 0;
                        linkBox.Text = "";
                    }
                    else
                    {
                        if (fileExists.Exist(failiNimi, formaat, path))
                        {
                            MessageBox.Show("Fail lõpetas tõmbamise");
                            progressBar1.Value = 0;
                            linkBox.Text = "";
                        }
                        else
                        {
                            MessageBox.Show("Miski on valesti");
                            progressBar1.Value = 0;
                            linkBox.Text = "";
                        }
                    }
                    TXTFail = "";
                }
                else
                {
                    MessageBox.Show("See fail juba eksisteerib");
                    linkBox.Text = "";
                }
            }
            else
            {
                MessageBox.Show("Palun täida kõik väljad");
            }
        }
        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string filePath in files)
            {
                TXTFail = filePath;
            }
            MessageBox.Show("TXTFail sisestatud");
        }
    }
}
