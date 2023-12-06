using CommunityToolkit.Maui.Views;

namespace MauiComm_IssuePopupLeak
{
	public partial class MainPage : ContentPage
	{
		int showCnt = 0;

		public MainPage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
		}

		void btnShow_Clicked(object sender, EventArgs e)
		{
			(sender as View).IsVisible = false;
			var timer = Dispatcher.CreateTimer();
			timer.Interval = TimeSpan.FromMilliseconds(250);
			showCnt = 0;
			PopupPage1 popup = null;
			timer.Tick += delegate
			{
				timer.Stop();
				popup = new PopupPage1();
				popup.Opened += delegate
				{
					popup.Update(showCnt.ToString());
					Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(250), async delegate
					{
						await popup.CloseAsync();
					});
				};
				popup.Closed += delegate { timer.Start(); };
				showCnt++;
				this.ShowPopup(popup);
			};
			timer.Start();
		}
	}
}