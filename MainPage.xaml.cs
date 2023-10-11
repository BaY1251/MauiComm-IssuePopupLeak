using CommunityToolkit.Maui.Views;

namespace MauiComm_IssuePopupLeak
{
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
            PopupPage1 popup = null;
            timer.Tick += async (s, e) =>
            {
                if (showCnt == maxShowCnt)
                {
                    timer.Stop();
                }

                if (!isShow)
                {
                    popup = new PopupPage1();
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
}