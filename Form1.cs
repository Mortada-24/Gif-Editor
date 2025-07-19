using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AnimationImage
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int frame_count;
        Image image;
        Image img;
        ImageList imageList = new ImageList();
        int currentFrameIndex = 0;



        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Interval = 150;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;

            // Add values to the combo box
            comboBox1.Items.AddRange(new object[] { "50", "100", "150", "200", "250" });

            // Set the default selected item
            comboBox1.SelectedIndex = 1;
        }



        //Load Gif Image
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "GIF(*.gif)|" + "*.gif";
            string img_path;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                img_path = openFileDialog.FileName;
                pictureBox1.Image = new Bitmap(img_path);
            }
        }



        //Gif image Framing
        private void button2_Click(object sender, EventArgs e)
        {
            image = pictureBox1.Image;
            FrameDimension dimenation = new FrameDimension(image.FrameDimensionsList[0]);
            frame_count = image.GetFrameCount(dimenation);  // Counting frames
            label1.Text = "Frame : " + frame_count.ToString();
            label1.ForeColor = Color.Yellow;
            imageList.Images.Clear();
            listView1.Items.Clear();
            pictureBox2.Image = null;
            pictureBox3.Image = null;

            for (int i = 0; i < frame_count; i++)
            {
                image.SelectActiveFrame(new FrameDimension(image.FrameDimensionsList[0]), i);
                pictureBox1.Invalidate();
                imageList.ImageSize = new Size(256, 256);
                listView1.LargeImageList = imageList;
                imageList.Images.Add((Image)image.Clone());
                ListViewItem item = new ListViewItem("Frame" + i.ToString() + ".png");

                item.ImageIndex = i;
                listView1.Items.Add(item);
                listView1.ForeColor = Color.WhiteSmoke;
                Application.DoEvents();
                System.Threading.Thread.Sleep(30);
            }
            MessageBox.Show(frame_count + "  Frames Done ");
        }



        //Go back 5 frames
        private void button3_Click(object sender, EventArgs e)
        {
            int framesToGoBack = 5;

            if (currentFrameIndex >= framesToGoBack)
            {
                currentFrameIndex -= framesToGoBack;
            }
            else
            {
                currentFrameIndex = 0;
            }

            if (currentFrameIndex < imageList.Images.Count)
            {
                pictureBox2.Image = imageList.Images[currentFrameIndex];
            }
        }



        //Start Image animation
        private void button4_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < frame_count; i++)
            {
                if (listView1.Items.Count > 0)
                {
                    currentFrameIndex = 0; // Reset the frame index
                    timer1.Start();
                }
                else
                {
                    MessageBox.Show("No frames to animate.");
                }
            }
        }


        private void button5_Click(object sender, EventArgs e)  // Pause or Stop Image animation
        {
            timer1.Enabled = false;
        }


        private void button6_Click(object sender, EventArgs e)   //Reverse the Image Animation
        {
            if (listView1.Items.Count > 0)
            {
                // Reverse the order of images in the ImageList
                Image[] reversedImages = new Image[imageList.Images.Count];
                for (int i = 0; i < imageList.Images.Count; i++)
                {
                    reversedImages[i] = (Image)imageList.Images[imageList.Images.Count - 1 - i].Clone();
                }

                imageList.Images.Clear();
                imageList.Images.AddRange(reversedImages);

            }
        }



        //Go skipe next 5 frames
        private void button7_Click(object sender, EventArgs e)
        {
            int framesToSkip = 5;

            if (currentFrameIndex + framesToSkip < imageList.Images.Count)
            {
                currentFrameIndex += framesToSkip;  // + 5 
            }
            else
            {
                currentFrameIndex = imageList.Images.Count - 1;
            }

            if (currentFrameIndex < imageList.Images.Count)
            {
                pictureBox2.Image = imageList.Images[currentFrameIndex];
            }
        }



        //Save all frames from the List view
        private void button8_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string filepath = dialog.SelectedPath;
                int index = 1;
                foreach (Image frm in imageList.Images)
                {
                    string path = Path.Combine(filepath, string.Format("Frame {0}.png", index));
                    frm.Save(path, ImageFormat.Png);
                    index++;
                }
                MessageBox.Show(index + "Frames saved successfully!");
            }
        }



        //Save the selected frame from List view
        private void button9_Click(object sender, EventArgs e)
        {
            if (img != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Png Image(*.png)|" + "*.png";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    img.Save(saveFileDialog.FileName, ImageFormat.Bmp);
                    MessageBox.Show("Image saved successfully!");
                }
            }
            else
            {
                MessageBox.Show("No image selected.");
            }
        }



        // Send the frames to List view
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
            {
                var selectedIndex = listView1.SelectedItems[0];
                img = imageList.Images[selectedIndex.ImageIndex];
                pictureBox3.Image = img;
            }
        }



        //Put The frames in the PictureBox2 Frame by frame
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (currentFrameIndex < imageList.Images.Count)
            {
                pictureBox2.Image = imageList.Images[currentFrameIndex];
                currentFrameIndex++;
            }
        }



        //get the time intervle frome the combo box
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.TryParse(comboBox1.SelectedItem.ToString(), out int interval))
            {
                timer1.Interval = interval;
            }
        }

        
        private void button12_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                // Remove selected items from ListView and corresponding images from ImageList
                for (int i = listView1.SelectedItems.Count - 1; i >= 0; i--)
                {
                    int itemIndex = listView1.SelectedItems[i].Index;
                    listView1.Items.RemoveAt(itemIndex);

                    if (itemIndex < imageList.Images.Count)
                    {
                        imageList.Images.RemoveAt(itemIndex);
                    }
                }

                // Clear pictureBox3
                pictureBox3.Image = null;

                // Clear the img variable if it's a Bitmap
                img = null;

                // Update frame count and label
                frame_count = listView1.Items.Count;
                label1.Text = "Frame: " + frame_count.ToString();
            }
            else
            {
                MessageBox.Show("No items selected to delete.");
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (imageList.Images.Count > 0)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "GIF Image|*.gif";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    // Create a new GIF file
                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        // Set the GIF encoder type
                        ImageCodecInfo gifEncoder = GetEncoder(ImageFormat.Gif);

                        // Save the first image as the main image of the GIF
                        Image firstImage = imageList.Images[0];

                        // Create an encoder parameter for multi-frame
                        EncoderParameters encoderParams = new EncoderParameters(1);
                        encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)EncoderValue.MultiFrame);

                        firstImage.Save(fs, gifEncoder, encoderParams);

                        // Add subsequent frames to the GIF
                        for (int i = 1; i < imageList.Images.Count; i++)
                        {
                            encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)EncoderValue.FrameDimensionTime);
                            firstImage.SaveAdd(imageList.Images[i], encoderParams);
                        }

                        // Close the file
                        encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)EncoderValue.Flush);
                        firstImage.SaveAdd(encoderParams);
                    }

                    MessageBox.Show("GIF image saved successfully!");
                }
            }
            else
            {
                MessageBox.Show("No frames available to save as GIF.");
            }
        }

        // Helper method to get encoder info for a specific image format
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            throw new NotImplementedException("Encoder not found");
        }
    }
}
