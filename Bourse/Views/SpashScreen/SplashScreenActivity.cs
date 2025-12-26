using Acr.UserDialogs;
using Android;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Bourse.Views.Main;

namespace Bourse.Views.SpashScreen;

[Activity(Label = "Bourse", MainLauncher = true, Icon = "@mipmap/appicon", NoHistory = true)]
public class SplashScreenActivity : BaseActivity
{
    public const int RequestCodePermission = 1000;

    /// <summary>
    /// Layout de notre activité
    /// </summary>
    protected override int LayoutResourceId => Resource.Layout.SplashScreen;


    private static List<string> PERMISSIONS = new List<string>()
    {
        Manifest.Permission.WriteExternalStorage,
        Manifest.Permission.ReadExternalStorage,
        Manifest.Permission.AccessNetworkState,
        Manifest.Permission.AccessWifiState,
    };

    #region Life cycle

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        FindViewById<TextView>(Resource.Id.tvLabel).Text = "Permissions...";
        FindViewById<TextView>(Resource.Id.tvAppVersion).Text = $"Version {PackageManager.GetPackageInfo(PackageName, 0).VersionName}";
    }

    protected override void OnResume()
    {
        base.OnResume();
        Task startupWork = new Task(() => { Startup(); });
        startupWork.Start();
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        switch (requestCode)
        {
            case RequestCodePermission:
                Startup();
                break;
        }
    }

    #endregion

    /// <summary>
    /// Demande les permissions, crée les répertoires nécessaires à l'applicationet et la base de données 
    /// </summary>
    public void Startup()
    {
        if (RequestPermissions().Any())
        {
            RequestPermissions(RequestPermissions().ToArray(), RequestCodePermission);
        }
        else
        {
            StartApplication();
        }
    }

    private List<string> RequestPermissions()
    {
        List<string> demandes = new List<string>();
        foreach (var permission in PERMISSIONS)
        {
            if (CheckSelfPermission(permission) != Permission.Granted)
            {
                demandes.Add(permission);
            }
        }
        return demandes;
    }

    private void StartApplication()
    {
        try
        {
            UserDialogs.Init(this);
            Setup.Init(Application.Context.GetString(Resource.String.Project));
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }
        catch(Exception ex) 
        {
            UserDialogs.Instance.Alert(ex.Message, Application.Context.GetString(Resource.String.Exception));
        }
    }
}
