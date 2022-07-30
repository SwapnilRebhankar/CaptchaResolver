using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Tesseract;

namespace WinCaptchaResolver
{
    public partial class MainForm : Form
    {
        OpenFileDialog openFileDialog;

        private static readonly List<string> lstBlackColorCodeFrame = new List<string>()
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

            DrawAllBackgroundColors();
        }

        /// <summary>
        /// convert lstBlackColorCodeFrame colors from hex code to image
        /// </summary>
        private void DrawAllBackgroundColors()
        {
            var bitmap = new Bitmap(bckColorsPictBox.Width, bckColorsPictBox.Height);
            var colors = lstBlackColorCodeFrame.Select(c => ConvertsColorFromString(c)).ToList();

            using (var g = Graphics.FromImage(bitmap))
            {
                for (var i = 0; i < colors.Count; i++) 
                    DrawSingleRectangle(i, g, colors[i]);

                //bitmap.SetPixel(0, 0, color);
                //var image = bitmap.Clone(new Rectangle(0, 0, 1, 1), PixelFormat.Format32bppArgb);
                bckColorsPictBox.Image = bitmap;
            }
        }

        private static void DrawSingleRectangle(int i, Graphics g, Color color)
        {
            var rectangle = new Rectangle(i * 11, 1, 10, 10);
            g.DrawRectangle(new Pen(Color.Red), rectangle);
            g.FillRectangle(new SolidBrush(color), rectangle);
        }

        private static Color ConvertsColorFromString(string item)
        {
            var color = ColorTranslator.FromHtml(Regex.Replace(item, "^FF", "#", RegexOptions.IgnoreCase));
            return color;
        }

        #region OpenFileDialog

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
            lblCaptchaText.Text = RecognizeCaptcha(image, image.Width, image.Height, 8);
        }

        #endregion

        private string RecognizeCaptcha(Image img, int varWidth, int varHeight, int varCaptchaLength)
        {
            Bitmap imagem = new Bitmap(img);
            var rectangleF = new Rectangle(0, 0, varWidth, varHeight);
            imagem = imagem.Clone(rectangleF, PixelFormat.Format24bppRgb);

            var immb = AppliesSomeImageFilters(imagem);

            AppliesSomePixelLevelElaboration(immb);

            ////EuclideanColorFiltering filt = new EuclideanColorFiltering();
            ////filt.CenterColor = new AForge.Imaging.RGB(Color.Blue); //Pure White
            ////filt.Radius = 200; //Increase this to allow off-whites
            ////filt.FillColor = new AForge.Imaging.RGB(Color.Transparent); //Replacement Colour
            ////filt.FillOutside = false;
            ////filt.ApplyInPlace(immb);            

            pictureBox.Image = immb;

            string reconhecido = OCR(immb, varCaptchaLength);
            return reconhecido;
        }

        private void AppliesSomePixelLevelElaboration(Bitmap immb)
        {
            // The next 2 methods are just calling a specific routine on each pixel of the image.
            // The external part of the 2 methods was in common so I refactored to 1 method that accepts a delegate (per the specific inner part)

            // Make Image Clear Black and Line Blue.
            DoForAllPixelGeneric(immb, MakeImageClearBlackAndLineBlueDelegate);

            // Remove Blue Line From Image and Replace With Transparent.
            DoForAllPixelGeneric(immb, RemoveBlueLineFromImageandReplaceWithTransparent);
        }

        private Bitmap AppliesSomeImageFilters(Bitmap imagem)
        {
            Invert inverter = new Invert();
            ColorFiltering colorFiltering = new ColorFiltering
            {
                Blue = new AForge.IntRange(200, 255)
            };
            //cor.Red = new AForge.IntRange(1, 255);
            //cor.Green = new AForge.IntRange(255, 255);

            Opening opening = new Opening();
            BlobsFiltering blobsFiltering = new BlobsFiltering
            {
                MinHeight = 10
            };
            BilateralSmoothing bilateralSmoothing = new BilateralSmoothing();

            ////AForge.Imaging.Filters.Grayscale filter1 =
            ////new AForge.Imaging.Filters.Grayscale(1, 1, 1);
            ////-Working//FiltersSequence seq = new FiltersSequence(inverter, open, inverter, bc, inverter, open, cc, cor, bc, inverter, filter1, aa);
            ////WorkingPureWhiteAndRED--//FiltersSequence seq = new FiltersSequence(inverter, open, inverter, bc, inverter, open, cc, cor, bc, inverter, aa);

            FiltersSequence filtersSequence =
                new FiltersSequence(inverter, opening, colorFiltering, blobsFiltering, bilateralSmoothing, inverter);

            Bitmap immb = filtersSequence.Apply(imagem);

            EuclideanColorFiltering filter = new EuclideanColorFiltering
            {
                CenterColor = new AForge.Imaging.RGB(Color.Red), //Pure White
                Radius = 200, //Increase this to allow off-whites
                FillColor = new AForge.Imaging.RGB(Color.Blue), //Replacement Colour
                FillOutside = false
            };
            filter.ApplyInPlace(immb);
            pictureBox.Image = immb;
            return immb;
        }

        private void RemoveBlueLineFromImageandReplaceWithTransparent(Bitmap immb, Color now_color, int i, int j)
        {
            //Compare Pixel's Color ARGB property with the picked color's ARGB property
            if (now_color.Name != "ff0000ff") 
                return;
            
            if (i == 0) 
                return;

            bool varChangeColor = CheckIfNearbyPixelsAreBckgroundColor(immb, i, j);

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

        private bool CheckIfNearbyPixelsAreBckgroundColor(Bitmap immb, int i, int j)
        {
            bool varChangeColor = false;

            for (int k = 1; k <= 5; k++)
            {
                if ((i - k) <= 0)
                    continue;

                varChangeColor = IsNearbyPixelBackground(immb, j, i - k);

                if (varChangeColor)
                    break;
            }

            //TODO: I guess it could be done in just one for
            if (!varChangeColor)
            {
                for (int k = 1; k <= 5; k++)
                {
                    if ((i + k) >= pictureBox.Image.Height)
                        continue;

                    varChangeColor = IsNearbyPixelBackground(immb, j, i + k);

                    if (varChangeColor)
                        break;
                }
            }

            return varChangeColor;
        }

        private static bool IsNearbyPixelBackground(Bitmap immb, int j, int offset)
        {
            //Takes the color of a pixel in the nearby
            Color iRefValue = immb.GetPixel(j, offset);

            //Checks if the pixel color is contained in the list lstBlackColorCodeFrame or if it is red
            return lstBlackColorCodeFrame.Contains(iRefValue.Name) || iRefValue == Color.Red;
        }

        /// <summary>
        /// A generic method that applies the delegate method "specificAction" to all pixel of the image transforming them
        /// </summary>
        /// <param name="immb">The Image</param>
        /// <param name="specificAction">The specific delegate which will be applied to each pixel</param>
        private void DoForAllPixelGeneric(Bitmap immb, Action<Bitmap, Color, int, int> specificAction)
        {
            for (int i = 0; i < pictureBox.Image.Height; i++)
            {
                for (int j = 0; j < pictureBox.Image.Width; j++)
                {
                    //Get the color at each pixel
                    Color now_color = immb.GetPixel(j, i);
                    
                    specificAction(immb, now_color, j, i);
                }
            }
        }

        private static void MakeImageClearBlackAndLineBlueDelegate(Bitmap immb, Color now_color, int j, int i)
        {
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

        /// <summary>
        /// Teseract OCR : To Read Text From Image.
        /// </summary>
        /// <param name="varBitMap"></param>
        /// <returns></returns>
        private static string OCR(Bitmap varBitMap, int varCaptchaLength)
        {
            string res;
            using (var engine = new TesseractEngine(@"tessdata", "eng", EngineMode.TesseractAndCube))
            {
                //For Alpha Numeric Captcha//
                //engine.SetVariable("tessedit_char_whitelist", "0123456789");
                //For Alphabetic Captcha//
                //engine.SetVariable("tessedit_char_whitelist", "0123456789");

                string alphabet = "abcdefghijklmnopqrstuvwxyz";
                //create a string with the whole alphabet in uppercase
                string alphabetUpper = alphabet.ToUpper();

                //For Numeric Captcha//
                engine.SetVariable("tessedit_char_whitelist", alphabetUpper + "0123456789");
                engine.SetVariable("tessedit_unrej_any_wd", true);

                using (var page = engine.Process(varBitMap, PageSegMode.SingleBlock))
                {
                    res = page.GetText();
                    res = res.Substring(0, Math.Min(res.Length, varCaptchaLength));
                }
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
