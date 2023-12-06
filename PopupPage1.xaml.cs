using CommunityToolkit.Maui.Views;

namespace MauiComm_IssuePopupLeak;

public partial class PopupPage1 : Popup
{
	public PopupPage1()
	{
		InitializeComponent();
	}

	public void Update(string msg) => L_Count.Text = msg;
}