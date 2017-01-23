using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.Storage.FileProperties;
using Windows.UI.Popups;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;
using System.Net;
using System.Collections.Specialized;
using System.Text.RegularExpressions;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

    namespace CamDemo
    {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
        public sealed partial class MainPage : Page
        {
            private StorageFile storeFile;
            private IRandomAccessStream stream;
            private bool captureStart;

            public MainPage()
            {
                this.InitializeComponent();
                captureStart = false;
            }

            private async void captureBtn_Click(object sender, RoutedEventArgs e)
            {
                ////Sample Code 3
                captureStart = true;
                MediaCapture _mediaCapture;

                while (captureStart)
                {
                    _mediaCapture = new MediaCapture();
                    bool _isPreviewing;
                    await _mediaCapture.InitializeAsync();
                    //_mediaCapture.Failed += MediaCapture_Failed;
                    var myPictures = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Pictures);
                    StorageFile file = await myPictures.SaveFolder.CreateFileAsync("photo.jpg", CreationCollisionOption.ReplaceExisting);

                    using (var captureStream = new InMemoryRandomAccessStream())
                    {
                        await _mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), captureStream);

                        using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            var decoder = await BitmapDecoder.CreateAsync(captureStream);
                            var encoder = await BitmapEncoder.CreateForTranscodingAsync(fileStream, decoder);

                            var properties = new BitmapPropertySet {
                                { "System.Photo.Orientation", new BitmapTypedValue(PhotoOrientation.Normal, PropertyType.UInt16) }
                                };  
                            await encoder.BitmapProperties.SetPropertiesAsync(properties);

                            await encoder.FlushAsync();
                        
                        }
                    }
                }
            }

            private async void stopBtn_Click(object sender, RoutedEventArgs e)
            {
                    captureStart = false;
            }

            static async void MakeRequest()
            {
                var client = new HttpClient();
                var queryString = ParseQueryString(string.Empty);

                // Request headers
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "{59ec5dec2e7e4de09462db9f9ac8dddf}");

                // Request parameters
                queryString["returnFaceId"] = "true";
                queryString["returnFaceLandmarks"] = "false";
                queryString["returnFaceAttributes"] = "{string}";
                var uri = "https://api.projectoxford.ai/face/v1.0/detect?" + queryString;

                HttpResponseMessage response;

                // Request body
                byte[] byteData = Encoding.UTF8.GetBytes("{body}");

                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("< your content type, i.e. application/json >");
                    response = await client.PostAsync(uri, content);
                }

            }

        public static byte[] GetBitStream(string fileName)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                LargeIcon.Save(ms, ImageFormat.Bmp);
                return ms.ToArray();
            }
        }

            public static NameValueCollection ParseQueryString(string s)
            {
                NameValueCollection nvc = new NameValueCollection();

                    // remove anything other than query string from url
                    if (s.Contains("?"))
                {
                    s = s.Substring(s.IndexOf('?') + 1);
                }

                foreach (string vp in Regex.Split(s, "&"))
                {
                    string[] singlePair = Regex.Split(vp, "=");
                    if (singlePair.Length == 2)
                    {
                        nvc.Add(singlePair[0], singlePair[1]);
                    }
                    else
                    {
                            // only one key with no value specified in query string
                            nvc.Add(singlePair[0], string.Empty);
                    }
                }

                return nvc;
            }

        }
    }

//////////////////////////////////////////// Original Code /////////////////////////////////////////////////////////
//CameraCaptureUI capture = new CameraCaptureUI();
//capture.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
//capture.PhotoSettings.CroppedAspectRatio = new Size(3, 5);
//capture.PhotoSettings.MaxResolution = CameraCaptureUIMaxPhotoResolution.HighestAvailable;

//storeFile = await capture.CaptureFileAsync(CameraCaptureUIMode.Photo);
//if (storeFile != null)
//{
//    BitmapImage bimage = new BitmapImage();
//    stream = await storeFile.OpenAsync(FileAccessMode.Read); ;
//    bimage.SetSource(stream);
//    captureImage.Source = bimage;

//}

//// Sample Code 1
//MediaCapture _mediaCapture;
//bool _isPreviewing;

//_mediaCapture = new MediaCapture();
//await _mediaCapture.InitializeAsync();
////_mediaCapture.Failed += MediaCapture_Failed;

//var lowLagCapture = await _mediaCapture.PrepareLowLagPhotoCaptureAsync(ImageEncodingProperties.CreateUncompressed(MediaPixelFormat.Bgra8));

//var capturedPhoto = await lowLagCapture.CaptureAsync();
//var softwareBitmap = capturedPhoto.Frame.SoftwareBitmap;

//await lowLagCapture.FinishAsync();


//FileSavePicker fs = new FileSavePicker();
//fs.FileTypeChoices.Add("Image", new List<string>() { ".jpeg" });
//fs.DefaultFileExtension = ".jpeg";
//fs.SuggestedFileName = "Image" + DateTime.Today.ToString();
//fs.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
//fs.SuggestedSaveFile = storeFile;
//// Saving the file
//var s = await fs.PickSaveFileAsync();
//if (s != null)
//{
//    using (var dataReader = new DataReader(stream.GetInputStreamAt(0)))
//    {
//        await dataReader.LoadAsync((uint)stream.Size);
//        byte[] buffer = new byte[(int)stream.Size];
//        dataReader.ReadBytes(buffer);
//        await FileIO.WriteBytesAsync(s, buffer);
//    }
//}

