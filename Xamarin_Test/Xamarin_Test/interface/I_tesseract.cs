using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xamarin_Test
{ 
    public interface I_tesseract
    {
        Task<Tuple<string, ImageSource>> AndroidTesseractAsync(ImageSource image);
    }
}
