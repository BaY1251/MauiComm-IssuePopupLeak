using System.Diagnostics;
using CommunityToolkit.Maui.Views;
namespace MauiComm_IssuePopupLeak;

#if false
public partial class MainPage : ContentPage
{
	int maxShowCnt = int.MaxValue;
	int showCnt = 0;
	bool isShow = false;

	public MainPage()
	{
		InitializeComponent();
	}

	void btnShow_Clicked(object sender, EventArgs e)
	{
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
			}
		};
		timer.Start();
	}
}
#else
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

	private void Show()
	{
		Timer timer = null;
		PopupPage1 popup = new();
		popup.Opened += delegate
		{
			timer = new Timer(s =>
			{
				MainThread.BeginInvokeOnMainThread(delegate
				{
					if(++showCnt % 2 == 0)
					{
						try
						{
							if(popup is null)
							{
								Shell.Current?.DisplayAlert("popup", "null", "OK");
							}
							popup?.Close(null);
							popup = null;
						}
						catch(Exception x)
						{
							Shell.Current?.DisplayAlert("popup", x.Message, "OK");
						}
					}
					else
					{
						popup?.Update(showCnt.ToString());
					}
				});
			}, null, 0, 250); //定时更新
		};

		popup.Closed += (s, e) =>
		{
			timer.Dispose();
			Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(250), Show);
		};

		Shell.Current.ShowPopup(popup);
	}

	private void AutoStart()
	{
		var escapedTime = 1;
		PopupPage1 popup = new();
		ThreadPool.QueueUserWorkItem(async delegate
		{
			Debug.WriteLine(2);
			do
			{
				MainThread.BeginInvokeOnMainThread(() => popup?.Update(showCnt.ToString()));
				Thread.Sleep(300);
			} while(--escapedTime > 0); //到达时间或点停止时,退出倒计时

			Debug.WriteLine(4);
			await popup.CloseAsync();
			if(escapedTime == 0)
			{
				Debug.WriteLine(5);
				Debug.WriteLine($"-{showCnt++}-");
				Shell.Current.Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(300), () => AutoStart());
			}
		});

		Debug.WriteLine(1);
		this.ShowPopup(popup);
		Debug.WriteLine(3);
	}

	void btnShow_Clicked(object sender, EventArgs e)
	{
		showCnt = 0;
		(sender as View).IsVisible = false;
		AutoStart();
	}
}
#endif