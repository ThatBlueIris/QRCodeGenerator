using QRCoder;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace QRCodeGenerator
{
    public partial class Form1 : Form
    {
        private Color currentQrColor = Color.Black;
        private bool isUpdatingColor = false;
        private string customLogoPath = "";

        public Form1()
        {
            InitializeComponent();

            // --- Handler Wire-up ---
            textBox2.TextChanged += txtHex_TextChanged;
            button3.Click += btnPickColor_Click;
            button4.Click += btnAddLogo_Click;
            button5.Click += btnBatch_Click;
            button6.Click += btnCopy_Click;

            // Branding Buttons
            button7.Click += btnHub_Click;      // Project Hub
            button8.Click += btnFeedback_Click; // Feedback Portal
        }

        private void Form1_Load(object sender, EventArgs e) { }

        // --- 1. Logo Management ---
        private void btnAddLogo_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(customLogoPath))
            {
                customLogoPath = "";
                button4.Text = "Add Logo";
                button4.BackColor = default(Color);
                button4.ForeColor = Color.Black;
                button1.PerformClick();
                return;
            }

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select a Logo";
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    customLogoPath = ofd.FileName;
                    button4.Text = "Clear Logo";
                    button4.BackColor = Color.IndianRed;
                    button4.ForeColor = Color.White;
                    button1.PerformClick();
                }
            }
        }

        // --- 2. Color Management ---
        private void btnPickColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                cd.Color = currentQrColor;
                if (cd.ShowDialog() == DialogResult.OK) UpdateColors(cd.Color);
            }
        }

        private void txtHex_TextChanged(object sender, EventArgs e)
        {
            if (isUpdatingColor) return;
            try
            {
                string hex = textBox2.Text.Trim();
                if (!hex.StartsWith("#")) hex = "#" + hex;
                if (hex.Length == 7) UpdateColors(ColorTranslator.FromHtml(hex));
            }
            catch { }
        }

        private void UpdateColors(Color newColor)
        {
            isUpdatingColor = true;
            currentQrColor = newColor;
            textBox2.Text = ColorTranslator.ToHtml(newColor);
            button3.BackColor = newColor;
            button3.ForeColor = newColor.GetBrightness() < 0.5f ? Color.White : Color.Black;
            isUpdatingColor = false;
        }

        // --- 3. The Core Engines ---
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text)) return;

            using (QRCoder.QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(textBox1.Text, QRCoder.QRCodeGenerator.ECCLevel.H);
                Color bgColor = checkBox1.Checked ? Color.Transparent : Color.White;

                if (checkBox1.Checked)
                {
                    pictureBox1.BackColor = Color.Transparent;
                    pictureBox1.BackgroundImage = new Bitmap(2, 2);
                    using (Graphics g = Graphics.FromImage(pictureBox1.BackgroundImage))
                    {
                        g.FillRectangle(Brushes.LightGray, 0, 0, 1, 1);
                        g.FillRectangle(Brushes.White, 1, 0, 1, 1);
                        g.FillRectangle(Brushes.White, 0, 1, 1, 1);
                        g.FillRectangle(Brushes.LightGray, 1, 1, 1, 1);
                    }
                    pictureBox1.BackgroundImageLayout = ImageLayout.Tile;
                }
                else
                {
                    pictureBox1.BackgroundImage = null;
                    pictureBox1.BackColor = Color.White;
                }

                Bitmap customLogo = !string.IsNullOrEmpty(customLogoPath) ? new Bitmap(customLogoPath) : null;
                var qrCode = new QRCode(qrCodeData);
                pictureBox1.Image = qrCode.GetGraphic(20, currentQrColor, bgColor, customLogo, 15);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null) return;
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG Image|*.png";
                sfd.FileName = "MyQRCode.png";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
                    lblStatus.Text = "SYSTEM: QR CODE SAVED SUCCESSFULLY";
                }
            }
        }

        private void btnBatch_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "CSV Files|*.csv|Text Files|*.txt";
                if (ofd.ShowDialog() != DialogResult.OK) return;

                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    if (fbd.ShowDialog() != DialogResult.OK) return;

                    string[] lines = System.IO.File.ReadAllLines(ofd.FileName);
                    int success = 0; int errors = 0;

                    Bitmap customLogo = string.IsNullOrEmpty(customLogoPath) ? null : new Bitmap(customLogoPath);
                    Color bgColor = checkBox1.Checked ? Color.Transparent : Color.White;

                    using (QRCoder.QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator())
                    {
                        foreach (string line in lines)
                        {
                            try
                            {
                                if (string.IsNullOrWhiteSpace(line) || !line.Contains(",")) { errors++; continue; }
                                string[] parts = line.Split(',');
                                string fileName = parts[0].Trim();
                                string linkData = parts[1].Trim();

                                foreach (char c in System.IO.Path.GetInvalidFileNameChars()) fileName = fileName.Replace(c, '_');

                                var qrCodeData = qrGenerator.CreateQrCode(linkData, QRCoder.QRCodeGenerator.ECCLevel.H);
                                var qrCode = new QRCode(qrCodeData);
                                using (Bitmap finalImage = qrCode.GetGraphic(20, currentQrColor, bgColor, customLogo, 15))
                                {
                                    finalImage.Save(System.IO.Path.Combine(fbd.SelectedPath, fileName + ".png"), System.Drawing.Imaging.ImageFormat.Png);
                                }
                                success++;
                            }
                            catch { errors++; }
                        }
                    }
                    MessageBox.Show($"Batch Complete!\nSuccess: {success}\nErrors: {errors}", "Batch Report");
                }
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null) return;
            Clipboard.SetImage(pictureBox1.Image);
            button6.Text = "Copied!";
            button6.BackColor = Color.LightGreen;

            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer { Interval = 1500 };
            t.Tick += (s, args) => {
                button6.Text = "Copy Image";
                button6.BackColor = Color.DodgerBlue;
                t.Stop(); t.Dispose();
            };
            t.Start();
        }

        // --- 4. Branding & Feedback Hub ---

        private void btnHub_Click(object sender, EventArgs e)
        {
            textBox1.Text = "https://github.com/ThatBlueIris/QRCodeGenerator";
            UpdateColors(Color.FromArgb(27, 42, 89)); // Brand Blue #1B2A59
            customLogoPath = "";

            ShowToast("ACTIVE: REPOSITORY DEMO MODE", Color.FromArgb(27, 42, 89));
            button1.PerformClick();
        }

        private void btnFeedback_Click(object sender, EventArgs e)
        {
            ShowToast("SYSTEM: OPENING FEEDBACK PORTAL...", Color.DarkOrange);
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://github.com/ThatBlueIris/QRCodeGenerator/issues",
                    UseShellExecute = true
                });
            }
            catch { MessageBox.Show("Visit: github.com/ThatBlueIris/QRCodeGenerator/issues"); }
        }

        // Helper for the Toast message to avoid code duplication
        private void ShowToast(string message, Color highlightColor)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = highlightColor;
            lblStatus.Font = new Font(lblStatus.Font, FontStyle.Bold);

            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer { Interval = 3000 };
            t.Tick += (s, args) => {
                lblStatus.Text = "SYSTEM READY | BLUEIRIS-QR v1.0";
                lblStatus.ForeColor = Color.Gray;
                lblStatus.Font = new Font(lblStatus.Font, FontStyle.Regular);
                t.Stop(); t.Dispose();
            };
            t.Start();
        }

        // --- 5. Stability Handlers ---
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void checkBox1_CheckedChanged(object sender, EventArgs e) { }
        private void button6_Click(object sender, EventArgs e) { }
        private void button4_Click(object sender, EventArgs e) { }
        private void button3_Click(object sender, EventArgs e) { }
        private void button7_Click(object sender, EventArgs e) { }
        private void button8_Click(object sender, EventArgs e) { }
        private void lblStatus_Click(object sender, EventArgs e) { }
    }
}