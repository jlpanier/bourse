
namespace Bourse.Pages;

public partial class BoursoramaPage : ContentPage, IQueryAttributable
{
	public BoursoramaPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("item", out var obj) && obj is Business.Share item)
        {
            Browser.Source = item.Url;
        }
     }
}