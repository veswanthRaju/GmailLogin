using Android.Accounts;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Transitions;
using Android.Widget;
using Java.Net;
using System;
using System.Collections.Generic;
using System.Net;
using Xamarin.Auth;

namespace SampleFbLoginApp
{
    [Activity(Label = "Userdata")]
    public class Userdata : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.UserData);

            var Data = DataClass.userData;

            var username = FindViewById<TextView>(Resource.Id.Username);
            var picture = FindViewById<ImageView>(Resource.Id.userImageView);
            var name = FindViewById<TextView>(Resource.Id.name);

            //Bcz its a key value pair so we will get values like...
            //username.Text = Data["name"];

            var imageUrl = Data["picture"]["data"]["url"];
            var imageBitmap = GetImageBitmapFromUrl(imageUrl);
            picture.SetImageBitmap(imageBitmap);
        }

        private Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }
    }
}