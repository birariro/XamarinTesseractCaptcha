using System;
 
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Tesseract.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin_Test.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(Tesseract_Android))]
namespace Xamarin_Test.Droid
{
    
    class Tesseract_Android : I_tesseract
    {
       
        [Obsolete]
        public async Task<Tuple<string, ImageSource>> AndroidTesseractAsync(ImageSource image)
        {
            Tuple<string, ImageSource> result = await AndroidTesseractStart(image);
            return result;
        }
       


        [Obsolete]
        public async Task<Tuple<string , ImageSource>> AndroidTesseractStart(ImageSource image)
        {
            Context context = Android.App.Application.Context;
            string result = "";
            ImageSource resultImg = image; // Default
            try
            {
                string whitelist = "01234556789"; whitelist += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; whitelist += "abcdefghijklmnopqrstuvwxyz";
                

                TesseractApi api = new TesseractApi(context, AssetsDeployment.OncePerInitialization);
                await api.Init("eng");
                api.SetWhitelist(whitelist);

                Bitmap bitmap = await GetBitmapFromImageSourceAsync(image, context); //ImageSource -> Bitmap
                Bitmap rebitmap = BitMapWidthCutting(bitmap); //BitMap  Width  cut in half
                rebitmap = BitMapLineDelete(rebitmap ,2);//BitMap Line Delete

                //BitMapChack(rebitmap); //Console Write BitMap
                resultImg= BMPtoImgsource(rebitmap); // BitMap ->  ImageSource
                byte[] bitmapData = ConvertBitmapToByte(rebitmap); //BitMap - > Byte[]


                bool success = await api.SetImage(bitmapData);
                if (success)
                {
                    result = api.Text;
                }

                return Tuple.Create(result, resultImg);
               
                
            }catch(Exception e)
            {
                return Tuple.Create(e.Message, resultImg); ;
            }

        }
        public ImageSource BMPtoImgsource(Bitmap bitmap)
        {
            var imgsrc = ImageSource.FromStream(() =>
            {
                MemoryStream ms = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Png, 100, ms);


                ms.Seek(0L, SeekOrigin.Begin);
                return ms;
            });

            return imgsrc;
        }

        public void BitMapChack(Bitmap bitmap)
        {
            try
            {
                int Width = bitmap.Width;
                int Height = bitmap.Height;

               
                for (int y = 0; y < Height; y++)
                {
                    string tmp = "";
                    for (int x = 0; x < Width; x++)
                    {
                        int argb = bitmap.GetPixel(x, y);
                        int AS = (argb >> 24) & 0xff;
                        int RS = (argb >> 16) & 0xff;
                        int GS = (argb >> 8) & 0xff;
                        int BS = (argb) & 0xff;
                        if (argb == Android.Graphics.Color.Black)
                        {
                            tmp += "A";
                        }
                        else
                        {
                            tmp += "B";
                        }
                    }
                    System.Diagnostics.Debug.WriteLine(tmp);
                }

                
            }
            finally
            {
               
            }

        }



        public Bitmap BitMapLineDelete(Bitmap bitmap , byte lineHeight)
        {
            try
            {                
                int Width = bitmap.Width;
                int Height = bitmap.Height;
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        int argb = bitmap.GetPixel(x, y);
                        if (argb == Android.Graphics.Color.Black && (((Height-1)-y) > lineHeight))
                        {
                            byte lineChack = 0;
                            for(byte i =1; i < lineHeight; i++) 
                            {
                                if (bitmap.GetPixel(x, y + i) == Android.Graphics.Color.Black) lineChack++;
                                
                                else break;
                                
                            }
                           
                            if (lineChack == (lineHeight-1) && bitmap.GetPixel(x, y+ lineHeight) == Android.Graphics.Color.White)
                            {
                                for (byte i = 0; i < lineHeight; i++)
                                {
                                     bitmap.SetPixel(x, y+i, Android.Graphics.Color.White);
                                }
                                lineChack = 0;
                            }
                        }
                    }
                }
                bitmap.Height = Height;
                bitmap.Width = Width;
                return bitmap;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("ERR : " + e.Message);
                return bitmap;
            }
        }
        public Bitmap BitMapWidthCutting(Bitmap bitmap)
        {   
            Bitmap rebitmap = null;
            try
            {   int Width = bitmap.Width ;
                int Height = bitmap.Height ;


                Width = Width / 2;

                rebitmap = Bitmap.CreateBitmap(Width, Height, Bitmap.Config.Argb8888);


                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        int argb = bitmap.GetPixel(x, y);


                        if (argb == Android.Graphics.Color.Black)
                        {
                            rebitmap.SetPixel(x, y, Android.Graphics.Color.Black);
                        }
                        else
                        {
                            rebitmap.SetPixel(x, y, Android.Graphics.Color.White);

                        }
                    }
                }

                
                rebitmap.Height = Height;
                rebitmap.Width = Width;
                return rebitmap;
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("ERR : "+e.Message);
                return rebitmap;
            }
           
            
        }

        public Bitmap ImageReSize(Bitmap newbitmap)
        {
           
            Bitmap resizedBitmap = Bitmap.CreateScaledBitmap(newbitmap, (int)(newbitmap.Width * 0.5), (int)(newbitmap.Height * 0.5), true);
            return resizedBitmap;
        }

        public byte[] ConvertBitmapToByte(Bitmap bitmap)
        {
            byte[] bitmapData;
            using (var stream = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Png,100,stream);
                bitmapData = stream.ToArray();
            }
            return bitmapData;
        }

        
        public async Task<Bitmap> GetBitmapFromImageSourceAsync(ImageSource source, Context context)
        {

            IImageSourceHandler handler;

            if (source is FileImageSource)
            {
                handler = new FileImageSourceHandler();
            }
            else if (source is StreamImageSource)
            {
                handler = new StreamImagesourceHandler(); // sic
            }
            else if (source is UriImageSource)
            {
                handler = new ImageLoaderSourceHandler(); // sic
            }
            else
            {
                throw new NotImplementedException();
            }

        

            var returnValue = (Bitmap)null;

            
            returnValue = await handler.LoadImageAsync(source, context);
            return returnValue;
        }

       
    }
}