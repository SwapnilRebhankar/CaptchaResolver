using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;

namespace WinCaptchaResolver
{
    public partial class MainForm : Form
    {
        OpenFileDialog openFileDialog;
        List<string> lstBlackColorCodeFrame = new List<string>()
        {
           "ff010101","ff020202","ff030303","ff040404","ff050505","ff060606","ff070707","ff080808","ff090909","ff020101","ff040202","ff0b0505","ff0b0201",
           "ff030202","ff060404","ff060505","ff0b0b0b","ff0a0a0a","ff171717","ff000000","ff050303","ff060303","ff070303","ff0d0606","ff1a1a1a","ff030101",
           "ff0e0404","ff040303","ff070202","ff080303","ff1d1d1d","ff0c0c0c","ff0d0505","ff040101","ff0e0707","ff0e0707","ff060202","ff070404","ff0c0303",
           "ff090505","ff0b0303","ff090404","ff0d0d0d","ff050202","ff100606","ff090303","ff0f0f0f","ff050101","ff060101","ff070101","ff010000","ff070606",
           "ff050404","ff080202","ff0c0505","ff090202","ff120707","ff120404","ff130404","ff150404","ff1d0b0b","ff0b0707","ff0e0808"
        };

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            openFileDialog = new OpenFileDialog();
            openFileDialog.FileOk += Ofd_FileOk;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
        }

        private void Ofd_FileOk(object sender, CancelEventArgs e)
        {
            Bitmap image = new Bitmap(openFileDialog.FileName);
            if (image != null)
            {
                lblCaptchaText.Text = RecognizeCaptcha(image, 182, 50);
            }
        }

        private string RecognizeCaptcha(Image img, int varWidth, int varHeight)
        {
            Bitmap imagem = new Bitmap(img);
            imagem = imagem.Clone(new Rectangle(0, 0, varWidth, varHeight), System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            Invert inverter = new Invert();
            ColorFiltering cor = new ColorFiltering();
            cor.Blue = new AForge.IntRange(200, 255);
            //cor.Red = new AForge.IntRange(1, 255);
            //cor.Green = new AForge.IntRange(255, 255);

            Opening open = new Opening();
            BlobsFiltering bc = new BlobsFiltering();
            bc.MinHeight = 10;
            AForge.Imaging.Filters.BilateralSmoothing aa = new BilateralSmoothing();

            ////AForge.Imaging.Filters.Grayscale filter1 =
            ////new AForge.Imaging.Filters.Grayscale(1, 1, 1);
            ////-Working//FiltersSequence seq = new FiltersSequence(inverter, open, inverter, bc, inverter, open, cc, cor, bc, inverter, filter1, aa);
            ////WorkingPureWhiteAndRED--//FiltersSequence seq = new FiltersSequence(inverter, open, inverter, bc, inverter, open, cc, cor, bc, inverter, aa);

            FiltersSequence seq = new FiltersSequence(inverter, open, cor, bc, aa, inverter);

            Bitmap immb = seq.Apply(imagem);

            EuclideanColorFiltering filter = new EuclideanColorFiltering();
            filter.CenterColor = new AForge.Imaging.RGB(Color.Red); //Pure White
            filter.Radius = 200; //Increase this to allow off-whites
            filter.FillColor = new AForge.Imaging.RGB(Color.Blue); //Replacement Colour
            filter.FillOutside = false;
            filter.ApplyInPlace(immb);
            pictureBox.Image = immb;

            // Make Image Clear Black and Line Blue.
            for (int i = 0; i < pictureBox.Image.Height; i++)
            {
                for (int j = 0; j < pictureBox.Image.Width; j++)
                {
                    //Get the color at each pixel
                    Color now_color = immb.GetPixel(j, i);
                    if (now_color.Name == "ff0000ff")
                    {
                        immb.SetPixel(j, i, Color.Blue);
                    }
                    else if (lstBlackColorCodeFrame.Contains(now_color.Name))
                    {
                        immb.SetPixel(j, i, Color.Black);
                    }
                    else if (now_color.Name == "ffffffff")
                    {
                        immb.SetPixel(j, i, Color.White);
                    }
                    else
                    {
                        immb.SetPixel(j, i, Color.Red);
                    }
                }
            }

            // Remove Blue Line From Image and Replace With Transparent.
            for (int i = 0; i < pictureBox.Image.Height; i++)
            {
                for (int j = 0; j < pictureBox.Image.Width; j++)
                {
                    //Get the color at each pixel
                    Color now_color = immb.GetPixel(j, i);

                    //Compare Pixel's Color ARGB property with the picked color's ARGB property
                    if (now_color.Name == "ff0000ff")
                    {
                        if (i != 0)
                        {
                            bool varChangeColor = false;
                            for (int k = 1; k <= 5; k++)
                            {
                                if ((i - k) > 0)
                                {
                                    Color iRefValue = immb.GetPixel(j, i - k);
                                    if (lstBlackColorCodeFrame.Contains(iRefValue.Name) || iRefValue == Color.Red)
                                    {
                                        varChangeColor = true;
                                        break;
                                    }
                                }
                            }
                            if (!varChangeColor)
                            {
                                for (int k = 1; k <= 5; k++)
                                {
                                    if ((i + k) < pictureBox.Image.Height)
                                    {
                                        Color iRefValue = immb.GetPixel(j, i + k);
                                        if (lstBlackColorCodeFrame.Contains(iRefValue.Name) || iRefValue == Color.Red)                                        
                                        {
                                            varChangeColor = true;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (varChangeColor)
                            {
                                immb.SetPixel(j, i, Color.Black);
                            }
                            else
                            {
                                // Commented To Make Transperent.
                                //immb.SetPixel(j, i, Color.Transparent);                                                                
                            }
                        }
                    }
                }
            }

            ////EuclideanColorFiltering filt = new EuclideanColorFiltering();
            ////filt.CenterColor = new AForge.Imaging.RGB(Color.Blue); //Pure White
            ////filt.Radius = 200; //Increase this to allow off-whites
            ////filt.FillColor = new AForge.Imaging.RGB(Color.Transparent); //Replacement Colour
            ////filt.FillOutside = false;
            ////filt.ApplyInPlace(immb);            

            pictureBox.Image = immb;

            string reconhecido = OCR(immb);
            return reconhecido;
        }

        /// <summary>
        /// Teseract OCR : To Read Text From Image.
        /// </summary>
        /// <param name="varBitMap"></param>
        /// <returns></returns>
        private string OCR(Bitmap varBitMap)
        {
            string res = string.Empty;
            using (var engine = new TesseractEngine(@"tessdata", "eng", EngineMode.Default))
            {
                //For Alpha Numeric Captcha//
                //engine.SetVariable("tessedit_char_whitelist", "0123456789");
                //For Alphabetic Captcha//
                //engine.SetVariable("tessedit_char_whitelist", "0123456789");

                //For Numeric Captcha//
                engine.SetVariable("tessedit_char_whitelist", "0123456789");
                engine.SetVariable("tessedit_unrej_any_wd", true);

                using (var page = engine.Process(varBitMap, PageSegMode.SingleBlock))
                    res = page.GetText();
            }
            return res;
        }

        /// <summary>
        /// To Make Image Black & White.
        /// </summary>
        /// <param name="imgBmp"></param>
        /// <param name="hasBeenCleared"></param>
        private static void SetPixelColor(Bitmap imgBmp, bool hasBeenCleared = true) //type 0 dafault, image has has been cleared.
        {
            var bgColor = Color.White;
            var textColor = Color.Black;
            for (var x = 0; x < imgBmp.Width; x++)
            {
                for (var y = 0; y < imgBmp.Height; y++)
                {
                    var pixel = imgBmp.GetPixel(x, y);
                    var isCloserToWhite = hasBeenCleared ? ((pixel.R + pixel.G + pixel.B) / 3) > 180 : ((pixel.R + pixel.G + pixel.B) / 3) > 120;
                    imgBmp.SetPixel(x, y, isCloserToWhite ? bgColor : textColor);
                }
            }
        }
    }
}
