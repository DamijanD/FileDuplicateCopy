using ExifLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileDuplicateCopy
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            var files = System.IO.Directory.GetFiles(tbOriginalLocation.Text, tbExt.Text);

            progressBar1.Value = 0;
            progressBar1.Maximum = files.Length;
            progressBar1.Step = 1;

            foreach (var file in files)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(file);

                string destinationPath = "";
                if (fi.LastWriteTime.Year == DateTime.Now.Year)
                {
                    destinationPath = System.IO.Path.Combine(tbDestination.Text, fi.LastWriteTime.ToString("yyyy_MM_dd"), fi.Name);
                }
                else
                {
                    destinationPath = System.IO.Path.Combine(tbDestination.Text, fi.LastWriteTime.ToString("yyyy\\\\yyyy_MM_dd"), fi.Name);
                }


                System.IO.FileInfo fiDest = new System.IO.FileInfo(destinationPath);

                if (fiDest.Exists)
                {
                    try
                    {
                        bool duplicate = false;
                        using (var exifDest = new ExifReader(fiDest.FullName))
                        using (var exifSource = new ExifReader(fi.FullName))
                        {
                            exifDest.GetTagValue<DateTime>(ExifTags.DateTimeOriginal, out DateTime destDate);
                            exifSource.GetTagValue<DateTime>(ExifTags.DateTimeOriginal, out DateTime sourceDate);

                            if (destDate == sourceDate)
                            {
                                duplicate = true;
                            }
                        }

                        if (duplicate)
                        {
                            if (chbDelete.Checked)
                                System.IO.File.Delete(fi.FullName);
                            else
                                textBox1.AppendText("DELETE\t" + fi.FullName + Environment.NewLine);
                        }
                        else
                        {
                            textBox1.AppendText(fi.FullName + Environment.NewLine);
                        }
                    }
                    catch (Exception exc)
                    {
                        textBox1.AppendText(fi.FullName + " exc: " + exc.Message + Environment.NewLine );
                    }

                }
                else
                {
                    textBox1.AppendText(fi.FullName + Environment.NewLine);
                }

                progressBar1.PerformStep();

            }
        }
    }
}
