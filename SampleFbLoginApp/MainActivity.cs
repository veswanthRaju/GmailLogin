using Android.App;
using Android.OS;
using Android.Widget;
using System.Threading.Tasks;
using Xamarin.Auth;
using System;
using Android.Content;
using System.Linq;
using System.Json;

namespace SampleFbLoginApp
{
    [Activity(Label = "SampleFbLoginApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        Intent activity2;
        ProgressDialog progressDialog;
        Button facebook, logoutBtn;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            facebook = FindViewById<Button>(Resource.Id.FacebookButton);
            facebook.Click += delegate { LoginToApp(true); };

            activity2 = new Intent(this, typeof(Userdata));
            
            //For Gmail Saved Data Checking
            logoutBtn = FindViewById<Button>(Resource.Id.LogoutButton);
            logoutBtn.Click += Logout;

            var cache = AccountStore.Create().FindAccountsForService("myData").FirstOrDefault();
            if (cache != null)
            {
                Toast.MakeText(this, "Hello " + cache.Properties["Username"], ToastLength.Short).Show();
                facebook.Enabled = false;
                logoutBtn.Enabled = true;
            }
            else
            {
                facebook.Enabled = true;
                logoutBtn.Enabled = false;
            }
        }

        private void Logout(object sender, EventArgs e)
        {
            var data = AccountStore.Create(this).FindAccountsForService("myData").FirstOrDefault();
            if (data != null)
            {
                AccountStore.Create(this).Delete(data, "myData");
                facebook.Enabled = true;
                logoutBtn.Enabled = false;
                Toast.MakeText(this, "You are LoggedOut!!", ToastLength.Short).Show();
            }
        }
        
        void LoginToApp(bool allowCancel)
        {
            var auth = new OAuth2Authenticator(
                                clientId: Constants.GmailID,
                                scope: Constants.Scope,
                                authorizeUrl: new Uri(Constants.gmailAuth),
                                redirectUrl: new Uri(Constants.gmailRed));

            auth.AllowCancel = allowCancel;

            auth.Completed += async (sender, e) =>
            {
                progressDialog = ProgressDialog.Show(this, Constants.wait, Constants.info, true);
                if (!e.IsAuthenticated)
                {
                    Toast.MakeText(this, "Fail to authenticate!", ToastLength.Short).Show();
                    return;
                }

                var request = new OAuth2Request("GET", new Uri(Constants.Req), null, e.Account);
                var response = await request.GetResponseAsync();

                if (response != null)
                {
                    progressDialog.Hide();
                    string userJson = response.GetResponseText();
                    var data = JsonValue.Parse(userJson);

                    Account account = new Account();
                    account.Properties.Add("Username", data["name"]);
                    AccountStore.Create(this).Save(account, "myData");

                    Toast.MakeText(this, "Welcome " + data["name"], ToastLength.Short).Show();
                    facebook.Enabled = false;
                    logoutBtn.Enabled = true;
                }
            };
            StartActivity(auth.GetUI(this));
        }
    }
}