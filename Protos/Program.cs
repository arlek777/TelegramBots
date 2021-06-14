using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Protos
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var filePath in Directory.GetFiles("TestSet"))
            {
                var file = File.OpenRead(filePath);
                var bitmap = new Bitmap(file);
                var image = bitmap.GetThumbnailImage(bitmap.Size.Width / 10, bitmap.Size.Height / 10, () => true, IntPtr.Zero);
                var resizedImage = new Bitmap(image);
                file.Close();

                PictureAnalysis.GetMostUsedColor(resizedImage);
                var res = PictureAnalysis.MostUsedColor;
                continue;

                var hasset = new Dictionary<string, long>();

                for (int i = 0; i < resizedImage.Size.Width; i++)
                {
                    for (int j = 0; j < resizedImage.Size.Height; j++)
                    {
                        var color = resizedImage.GetPixel(i, j);
                        if (hasset.ContainsKey(color.Name))
                        {
                            hasset[color.Name] += 1;
                        }
                        else
                        {
                            hasset.Add(color.Name, 1);
                        }
                    }
                }

                var result = hasset.OrderByDescending(h => h.Value).FirstOrDefault();

                if (Directory.Exists("Result/" + result.Key))
                {
                    var fileInfo = new FileInfo(filePath);
                    File.Copy(filePath, "Result/" + result.Key + "/" + fileInfo.Name);
                }
                else
                {
                    Directory.CreateDirectory("Result/" + result.Key);
                    var fileInfo = new FileInfo(filePath);
                    File.Copy(filePath, "Result/" + result.Key + "/" + fileInfo.Name);
                }
            }
        }
    }

    public static class PictureAnalysis
    {
        public static List<Color> TenMostUsedColors { get; private set; }
        public static List<int> TenMostUsedColorIncidences { get; private set; }

        public static Color MostUsedColor { get; private set; }
        public static int MostUsedColorIncidence { get; private set; }

        private static int pixelColor;

        private static Dictionary<int, int> dctColorIncidence;

        public static void GetMostUsedColor(Bitmap theBitMap)
        {
            TenMostUsedColors = new List<Color>();
            TenMostUsedColorIncidences = new List<int>();

            MostUsedColor = Color.Empty;
            MostUsedColorIncidence = 0;

            // does using Dictionary<int,int> here
            // really pay-off compared to using
            // Dictionary<Color, int> ?

            // would using a SortedDictionary be much slower, or ?

            dctColorIncidence = new Dictionary<int, int>();

            // this is what you want to speed up with unmanaged code
            for (int row = 0; row < theBitMap.Size.Width; row++)
            {
                for (int col = 0; col < theBitMap.Size.Height; col++)
                {
                    pixelColor = theBitMap.GetPixel(row, col).ToArgb();

                    if (dctColorIncidence.Keys.Contains(pixelColor))
                    {
                        dctColorIncidence[pixelColor]++;
                    }
                    else
                    {
                        dctColorIncidence.Add(pixelColor, 1);
                    }
                }
            }

            // note that there are those who argue that a
            // .NET Generic Dictionary is never guaranteed
            // to be sorted by methods like this
            var dctSortedByValueHighToLow = dctColorIncidence.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            // this should be replaced with some elegant Linq ?
            foreach (KeyValuePair<int, int> kvp in dctSortedByValueHighToLow.Take(10))
            {
                TenMostUsedColors.Add(Color.FromArgb(kvp.Key));
                TenMostUsedColorIncidences.Add(kvp.Value);
            }

            MostUsedColor = Color.FromArgb(dctSortedByValueHighToLow.First().Key);
            MostUsedColorIncidence = dctSortedByValueHighToLow.First().Value;
        }

    }
}
