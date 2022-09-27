#if WINDOWS
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tesseract;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.System.UserProfile;

/*
 * 需要安装nuget Microsoft.Windows.SDK.Contracts
 */
namespace ApplePDF.Demo.Maui.Services
{
    public static class OcrService
    {
        /// <summary>
        /// Extracts text from an image using Windows 10 OCR.
        /// The extraction is done using a machine's active preferred language.
        /// </summary>
        /// <param name="imagePath">The image to extract text from.</param>
        /// <returns>The text extracted from an image.</returns>
        public static async Task<string> RecognizeText(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                throw new ArgumentNullException("Image can't be null or empty.");

            if (!File.Exists(imagePath))
                throw new ArgumentOutOfRangeException($"'{imagePath}' doesn't exist.");

            CheckIfFileIsImage(imagePath);

            StringBuilder text = new StringBuilder();

            await using (var fileStream = File.OpenRead(imagePath))
            {
                //var bmpDecoder = await BitmapDecoder.CreateAsync(fileStream.AsRandomAccessStream());
                var bmpDecoder = await BitmapDecoder.CreateAsync(fileStream.AsRandomAccessStream());
                var softwareBmp = await bmpDecoder.GetSoftwareBitmapAsync();

                var ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
                var ocrResult = await ocrEngine.RecognizeAsync(softwareBmp);

                foreach (var line in ocrResult.Lines) text.AppendLine(line.Text);
            }

            return text.ToString();
        }

        /// <summary>
        /// Extracts text from an image using Windows 10 OCR.
        /// The extraction is done using a machine's active preferred language.
        /// </summary>
        /// <param name="imagePath">The image to extract text from.</param>
        /// <returns>The text extracted from an image.</returns>
        public static async Task<string> RecognizeText(Stream image)
        {
            StringBuilder text = new StringBuilder();

            var bmpDecoder = await BitmapDecoder.CreateAsync(image.AsRandomAccessStream());
            var softwareBmp = await bmpDecoder.GetSoftwareBitmapAsync();

            var ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
            var ocrResult = await ocrEngine.RecognizeAsync(softwareBmp);

            foreach (var line in ocrResult.Lines) text.AppendLine(line.Text);

            return text.ToString();
        }

        /// <summary>
        /// Extracts text from an image using Windows 10 OCR.
        /// The extraction is done using a machine's active preferred language.
        /// </summary>
        /// <param name="image">The image to extract text from.</param>
        /// <returns>The text extracted from an image.</returns>
        public static async Task<List<OcrData>> RecognizeWords(Stream image)
        {
            var bmpDecoder = await BitmapDecoder.CreateAsync(image.AsRandomAccessStream());
            var softwareBmp = await bmpDecoder.GetSoftwareBitmapAsync();

            var ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
            var ocrResult = await ocrEngine.RecognizeAsync(softwareBmp);

            List<OcrData> lines = new List<OcrData>();
            foreach (var line in ocrResult.Lines)
            {
                var lineText = new OcrData() { Text = line.Text, Childs = new List<OcrData>() };
                lines.Add(lineText);
                foreach (var word in line.Words)
                {
                    lineText.Childs.Add(new OcrData() { Text = word.Text, Bounds = new Microsoft.Maui.Graphics.Rect(word.BoundingRect.X, word.BoundingRect.Y, word.BoundingRect.Width, word.BoundingRect.Height) });
                }
            };

            return lines;
        }

        /// <summary>
        /// Extracts text from an image using Windows 10 OCR.
        /// </summary>
        /// <param name="imagePath">The image to extract text from.</param>
        /// <param name="languageCode">The language code of the language in the image.
        /// The language code should be an installed language supported by Windows 10 OCR.</param>
        /// <returns>The text extracted from an image.</returns>
        public static async Task<string> RecognizeText(string imagePath, string languageCode)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                throw new ArgumentNullException("Image can't be null or empty.");

            if (string.IsNullOrWhiteSpace(languageCode))
                throw new ArgumentNullException("Language can't be null or empty.");

            if (!File.Exists(imagePath))
                throw new ArgumentOutOfRangeException($"'{imagePath}' doesn't exist.");

            CheckIfFileIsImage(imagePath);

            if (!GlobalizationPreferences.Languages.Contains(languageCode))
                throw new ArgumentOutOfRangeException($"{languageCode} is not installed.");

            StringBuilder text = new StringBuilder();

            await using (var fileStream = File.OpenRead(imagePath))
            {
                var bmpDecoder = await BitmapDecoder.CreateAsync(fileStream.AsRandomAccessStream());
                var softwareBmp = await bmpDecoder.GetSoftwareBitmapAsync();

                var ocrEngine = OcrEngine.TryCreateFromLanguage(new Language(languageCode));
                var ocrResult = await ocrEngine.RecognizeAsync(softwareBmp);

                foreach (var line in ocrResult.Lines) text.AppendLine(line.Text);
            }

            return text.ToString();
        }

        private static void CheckIfFileIsImage(string file)
        {
            var isImage = Regex.IsMatch(Path.GetExtension(file).ToLower(),
                "(jpg|jpeg|jfif|png|bmp)$", RegexOptions.Compiled);

            if (!isImage)
                throw new ArgumentOutOfRangeException($"'{file}' is not an imagePath.");
        }

        /// <summary>
        /// Extracts the text.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="languageCode">The language code.Chinese use "zh-Hans-CN" <see cref="https://docs.microsoft.com/en-us/previous-versions/windows/apps/jj673578(v=win.10)"/></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static async Task<string> RecognizeText(SoftwareBitmap image, string languageCode)
        {
            if (!GlobalizationPreferences.Languages.Contains(languageCode))
                throw new ArgumentOutOfRangeException($"{languageCode} is not installed.");

            StringBuilder text = new StringBuilder();

            var ocrEngine = OcrEngine.TryCreateFromLanguage(new Language(languageCode));
            var ocrResult = await ocrEngine.RecognizeAsync(image);

            foreach (var line in ocrResult.Lines) text.AppendLine(line.Text);

            return text.ToString();
        }
    }
}
#endif