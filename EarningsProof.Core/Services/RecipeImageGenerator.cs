using System.Drawing;
using System.Drawing.Imaging;

namespace EarningsProof.Core.Services;

public class RecipeImageGenerator
{
    public async Task<Stream> Generate(string sum)
    {
        PointF firstLocation = new Point(65, 160);

        // TODO date, sum balance, walletId

        string imageFilePath = "check.jpg"; // TODO move it

        Bitmap bitmap = new Bitmap(File.Open(imageFilePath, new FileStreamOptions() { Access = FileAccess.ReadWrite, Mode = FileMode.Open }));

        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            using (Font arialFont = new Font("Arial", 48))
            {
                graphics.DrawString("694,400.00", arialFont, Brushes.Black, firstLocation);
            }
        }

        try
        {
            File.Delete("img1.jpg");
	        bitmap.Save("img1.jpg", ImageFormat.Jpeg);
		}
		catch (Exception e)
        {
        }

        return null;
    }
}