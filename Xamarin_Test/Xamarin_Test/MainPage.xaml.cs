using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xamarin_Test
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private string _ImageText;
        public string ImageText
        {
            get => _ImageText;
            set
            {
                _ImageText = value;
                OnPropertyChanged();
            }
        }

        private ImageSource _ImgSource;
        public ImageSource ImgSource
        {
            get => _ImgSource;
            set
            {
                _ImgSource = value;
                OnPropertyChanged();
            }
        }

        private ImageSource _ReImgSource;
        public ImageSource ReImgSource
        {
            get => _ReImgSource;
            set
            {
                _ReImgSource = value;
                OnPropertyChanged();
            }
        }
        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = this;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Random r = new Random();
            string randomTime = Convert.ToString(r.Next(555512, 999999));
            randomTime += Convert.ToString(r.Next(555512, 999999));
            ImgSource = ImageSource.FromUri(new Uri("https://url?time=" + randomTime + "&captchaId=3da5426cd1"));
            

            callTesseract(ImgSource);
        }

        public async Task callTesseract(ImageSource imageSource)
        {


            Tuple<string, ImageSource> result;
            result = await DependencyService.Get<I_tesseract>().AndroidTesseractAsync(imageSource);
           
            ImageText = "-> "+result.Item1;
            ReImgSource = result.Item2;


        }


        

    }
}
