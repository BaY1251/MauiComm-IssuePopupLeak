using System.Diagnostics;
using CommunityToolkit.Maui.Views;

namespace MauiComm_IssuePopupLeak;

public partial class MainPage : ContentPage
{
	volatile int showCnt = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	void btnShow_Clicked(object sender, EventArgs e)
	{
#if false
		var timer = Dispatcher.CreateTimer();
		timer.Interval = TimeSpan.FromMilliseconds(250);
		showCnt = 0;
		Popup popup = null;
		timer.Tick += async (s, e) =>
		{
			if(showCnt == maxShowCnt)
			{
				timer.Stop();
			}

			if(!isShow)
			{
				var txt = new Label { Text = $"{showCnt}", FontSize = 72, TextColor = Colors.Gray, HorizontalOptions = LayoutOptions.Center };
				popup = new Popup { Content = txt, Size = new Size(480, 240), Color = Colors.Transparent, CanBeDismissedByTappingOutsideOfPopup = false };

				//popup = new PopupPage1();
				this.ShowPopup(popup);
				isShow = true;
			}
			else
			{
				await popup.CloseAsync();
				isShow = false;
				showCnt++;
				GC.Collect();
			}
		};
		timer.Start();
#else
		showCnt = 0;
		(sender as View).IsVisible = false;
		AutoStart();
#endif
	}

	private void AutoStart()
	{
		var escapedTime = 1;
		PopupPage1 popup = new();
		popup?.Update(showCnt.ToString());
		popup.Opened += delegate
		{
			ThreadPool.QueueUserWorkItem(async delegate
			{
				Debug.WriteLine(3);
				do
				{
					await MainThread.InvokeOnMainThreadAsync(() => popup?.Update(showCnt.ToString()));
					Thread.Sleep(100);
				} while(--escapedTime > 0);

				Debug.WriteLine(4);
				MainThread.BeginInvokeOnMainThread(async delegate
				{
					Debug.WriteLine(5);
					await popup.CloseAsync();
					Debug.WriteLine($"-{showCnt++}-");
					Shell.Current.Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(10), () => AutoStart());
					//GC.Collect();
				});
			});
		};

		Debug.WriteLine(1);
		this.ShowPopup(popup);
		Debug.WriteLine(2);
	}
}
