using AndroidX.Fragment.App;
using System.Reflection;

namespace Bourse.Views;

public abstract class BaseActivity : FragmentActivity
{
    public const string ParamCode = "Parametre.Code";

    public const string ParamUrl = "Parametre.Url";

    /// <summary>
    /// Notre layout pour cette activité
    /// </summary>
    protected abstract int LayoutResourceId { get; }

    #region Life Cycle

    /// <summary>
    /// OnCreate est la première méthode à appeler lorsqu’une activité est créée. 
    /// OnCreate est toujours remplacée pour effectuer toutes les initialisations de démarrage qui peuvent être requis par une activité telles que :
    /// - Création de vues
    /// - Initialiser des variables
    /// - Liaison de données statiques aux listes
    /// Aide Windows: https://docs.microsoft.com/fr-fr/xamarin/android/
    /// https://developer.xamarin.com/guides/android/application_fundamentals/activity_lifecycle/
    /// Icon : http://modernuiicons.com/ 
    /// </summary>
    /// <param name="savedInstanceState"></param>
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(LayoutResourceId);

        //var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
        //try
        //{
        //    SetActionBar(toolbar);

        //    // Most Android apps rely on the Back button for app navigation; pressing the Back button takes the user to the previous screen.
        //    // For apps with multiple activities, it often makes sense to instead provide an Up button so that the user can move up to a higher level in the app hierarchy
        //    // (that is, the app pops the user back multiple activities in the back stack rather than popping back to the previously - visited Activity).
        //    // To enable the Up button in a second activity that uses a Toolbar as its action bar, call the SetDisplayHomeAsUpEnabled and SetHomeButtonEnabled methods in the second activity's OnCreate method: 
        //    ActionBar.SetDisplayHomeAsUpEnabled(true);
        //    ActionBar.SetHomeButtonEnabled(true);
        //    ActionBar.Title = "Toolbar";
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine(ex.ToString());
        //}
    }

    #endregion

    #region Message

    public void Message(string text, ToastLength length = ToastLength.Short)
    {
        Toast.MakeText(this, text, length).Show();
    }

    public void MessageBox(string text, ToastLength length = ToastLength.Short)
    {
        Toast.MakeText(this, text, length).Show();
    }

    #endregion
}