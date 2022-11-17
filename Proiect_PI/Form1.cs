using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proiect_PI
{
    public partial class Form1 : Form
    {
        #region Private fiels
        private Image image;
        private bool opened = false;
        private bool sidebarExpand = true;
        private float contrast = 0;
        #endregion

        #region Private methods
        private void ImagesValues()
        {
            OFormatValue.Text = openFileDialog1.FileName.Substring(openFileDialog1.FileName.Length - 3).ToLower();
            OPixelValue.Text = image.PixelFormat.ToString();
            OWidthValue.Text = image.Width.ToString() + " px";
            OHeightValue.Text = image.Height.ToString() + " px";

            MFormatValue.Text = openFileDialog1.FileName.Substring(openFileDialog1.FileName.Length - 3).ToLower();
            MPixelValue.Text = image.PixelFormat.ToString();
            MWidthValue.Text = image.Width.ToString() + " px";
            MHeightValue.Text = image.Height.ToString() + " px";
        }
        #endregion

        public Form1()
        {
            InitializeComponent();
            renameFileLabel.Hide();
            renameTextBox.Hide();
            arrowPicture.Hide();
            loadingPicture1.Hide();
            loadingPicture2.Hide();
        }

        private async void button_open_Click(object sender, EventArgs e)
        {
            DialogResult dr = openFileDialog1.ShowDialog();

            if (dr == DialogResult.OK)
            {
                image = Image.FromFile(openFileDialog1.FileName);
                if (!(image.PixelFormat.ToString() == "Format24bppRgb" || image.PixelFormat.ToString() == "Format8bppIndexed"))
                {
                    MessageBox.Show("Please choose an image that has 24 or 8 bits format.");
                }
                else
                {
                    if (sidebar.Width == sidebar.MaximumSize.Width)
                    {
                        sidebarTimer.Start();
                    }
                    pictureBox1.Image = null;
                    pictureBox4.Image = null;
                    loadingPicture1.Show();
                    loadingPicture2.Show();

                    await Task.Run(() => LoadImages());
                    loadingPicture1.Hide();
                    loadingPicture2.Hide();

                    pictureBox1.Image = image;
                    pictureBox4.Image = image;
                    opened = true;
                    ImagesValues();
                    renameFileLabel.Show();
                    renameTextBox.Show();
                    arrowPicture.Show();
                    
                }
            }
        }

        private void LoadImages()
        {
            Random numberInstance = new Random();
            var number = numberInstance.Next(1000,3000);
            Thread.Sleep(number);
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            if (opened)
            {
                DialogResult dr = saveFileDialog1.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    switch (saveFileDialog1.FileName.Substring(saveFileDialog1.FileName.Length - 3).ToLower())
                    {
                        case "bmp":
                            pictureBox1.Image.Save(saveFileDialog1.FileName, ImageFormat.Bmp);
                            break;
                        case "jpg":
                            pictureBox1.Image.Save(saveFileDialog1.FileName, ImageFormat.Jpeg);
                            break;
                        case "png":
                            pictureBox1.Image.Save(saveFileDialog1.FileName, ImageFormat.Png);
                            break;
                        case "gif":
                            pictureBox1.Image.Save(saveFileDialog1.FileName, ImageFormat.Gif);
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Please open an image first.");
            }
        }

        private void sidebarTimer_Tick(object sender, EventArgs e)
        {
            if (sidebarExpand)
            {
                sidebar.Width -= 10;
                if (sidebar.Width == sidebar.MinimumSize.Width)
                {
                    sidebarExpand = false;
                    sidebarTimer.Stop();
                }
            }
            else
            {
                sidebar.Width += 10;
                if (sidebar.Width == sidebar.MaximumSize.Width)
                {
                    sidebarExpand = true;
                    sidebarTimer.Stop();
                }
            }
        }

        private void menuButton_Click(object sender, EventArgs e)
        {
            sidebarTimer.Start();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (opened)
            {
                contrastValue.Text = trackBar1.Value.ToString();
                contrast = 0.08f * trackBar1.Value;

                Bitmap bm = new Bitmap(image.Width, image.Height);
                Graphics g = Graphics.FromImage(bm);
                ImageAttributes ia = new ImageAttributes();
                ColorMatrix colorMatrix = new ColorMatrix(new float[][] {
                                                     new float[] { contrast, 0f, 0f, 0f, 0f },
                                                     new float[] { 0f, contrast, 0f, 0f, 0f },
                                                     new float[] { 0f, 0f, contrast, 0f, 0f },
                                                     new float[] { 0f, 0f, 0f, 1f, 0f },
                                                     new float[] { 0.001f, 0.001f, 0.001f, 0f, 1f }});
                ia.SetColorMatrix(colorMatrix);
                g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, ia);
                g.Dispose();
                ia.Dispose();
                pictureBox1.Image = bm;

            }
            else
            {
                MessageBox.Show("Please open an image first.");
                button_open_Click(sender, e);
            }
        }

        private void openFileButton_Click(object sender, EventArgs e)
        {
            button_open_Click(sender, e);
        }

        private void saveFileButton_Click(object sender, EventArgs e)
        {
            button_save_Click(sender, e);
        }
        private void renameFileButton_Click(object sender, EventArgs e)
        {
            button_rename_Click(sender, e);
        }

        private void button_rename_Click(object sender, EventArgs e)
        {
            if(opened && renameTextBox.Text != string.Empty)
            {
                if(renameTextBox.Text.Substring(renameTextBox.Text.Length - 3).ToLower() == "jpg" ||
                   renameTextBox.Text.Substring(renameTextBox.Text.Length - 3).ToLower() == "bmp" ||
                   renameTextBox.Text.Substring(renameTextBox.Text.Length - 3).ToLower() == "png" ||
                   renameTextBox.Text.Substring(renameTextBox.Text.Length - 3).ToLower() == "gif")
                {
                    var oldFileName = openFileDialog1.FileName;
                    var path = "C:\\VS Projects\\Proiect_PI\\Proiect_PI\\images\\";
                    image.Dispose();
                    File.Move(oldFileName, path + renameTextBox.Text);
                    MessageBox.Show("File renamed successfully!");
                    renameTextBox.Text = string.Empty;
                }
                else
                {
                    MessageBox.Show("Please enter an image format. JPG | BMP | PNG | GIF");
                }
            }
            else
            {
                MessageBox.Show("Please open an image first or enter a new name for the file.");
            }
        }


    }
}
