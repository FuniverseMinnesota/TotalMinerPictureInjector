﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TotalMinerPictureInjector
{
    public partial class Form1 : Form
    {
        public List<TmPic> pics = new List<TmPic>();

        public string Selected_File;

        public Form1()
        {
            InitializeComponent();

            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            checkBox1.Enabled = false;
        }

        private void Extract_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count < 0 && pics.Count < 0)
                return;

            var index = listBox1.SelectedIndex;

            using (SaveFileDialog file = new SaveFileDialog() { Filter = "img file|*.png", })
            {
                var dialog = file.ShowDialog();

                if (dialog == DialogResult.OK)
                    pics[index].Extract(pics[index].Hd, file.FileName);
            }
        }

        private void Replace_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count < 0 && pics.Count < 0)
                return;

            var index = listBox1.SelectedIndex;


            using (FileDialog file = new OpenFileDialog() { Filter = "img file|*.png",})
            {
                var dialog = file.ShowDialog();

                if (dialog == DialogResult.OK)
                {
                    pics[index].Replace(ref pics[index].Hd, file.FileName);
                    pics[index].Replace(ref pics[index].Lod, file.FileName);
                    pictureBox1.Image = pics[index].Hd.Item1;
                }
            }
        }

        private void Load_Click(object sender, EventArgs e)
        {
            pics.Clear();
            listBox1.Items.Clear();
            pictureBox1.Image = null;

            if (listBox1.Items.Count < 0 && pics.Count < 0)
                return;


            string mygames = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"My Games\TotalMiner\Maps\");
            using (FileDialog file =  new OpenFileDialog(){ FileName = "photos.dat", Filter = "TM Photo File|*.dat", InitialDirectory = mygames, Title = "Select photos.dat" })
            {
                var dialog = file.ShowDialog();

                if (string.IsNullOrEmpty(file.FileName) || Path.GetFileName(file.FileName) != "photos.dat")
                {
                    MessageBox.Show($"{Path.GetFileName(file.FileName)} is not a valid file. Please select \"photos.dat\".", "Invalid File");
                    return;
                }

                if (dialog == DialogResult.OK)
                    LoadPics(file.FileName);

                return;
            }
        }

        private void LoadPics(string path)
        {
            Selected_File = path;

            using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
            {
                int count = reader.ReadInt32();

                if (count < 0)
                    return;

                for (int i = 0; i < count; i++)
                {
                    listBox1.Items.Add($"Index: {i}");
                    pics.Add(new TmPic(reader));
                }
            }
            checkBox1.Enabled = listBox1.Items.Count > 0;
        }

        private void clear_Click(object sender, EventArgs e)
        {
            pics.Clear();
            listBox1.Items.Clear();
            pictureBox1.Image = null;
            Selected_File = "";
            checkBox1.Enabled = false;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = listBox1.SelectedIndex;

            if(checkBox1.Checked)
            {
                pictureBox1.Image = pics[index].Lod.Item1;
            }
            else
            {
                pictureBox1.Image = pics[index].Hd.Item1;
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Selected_File))
                return;

            using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(Selected_File, FileMode.Open)))
            {
                foreach (var item in pics.FindAll(x => x.Edited || x.Hd.Item1.Width != 64 || x.Lod.Item1.Width != 16))
                {
                    binaryWriter.BaseStream.Position = item.Hd.Item2;
                    binaryWriter.Write(Helper.ToColorDataAll(new Bitmap(item.Hd.Item1, new Size(64, 64)), out int sroot1));

                    binaryWriter.BaseStream.Position = item.Lod.Item2;
                    binaryWriter.Write(Helper.ToColorDataAll(new Bitmap(item.Lod.Item1, new Size(16, 16)), out int sroot2));
                }
            }

            clear_Click(this, EventArgs.Empty);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            var index = listBox1.SelectedIndex;

            if (index == -1)
            {
                MessageBox.Show("Please select a photo first.");
                return;
            }

            if (checkBox1.Checked)
            {
                pictureBox1.Image = pics[index].Lod.Item1;
            }
            else
            {
                pictureBox1.Image = pics[index].Hd.Item1;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Add_picture_button_Click(object sender, EventArgs e)
        {

        }
    }
}
