using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Collector_local_db
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public sealed partial class AddDebt
    {
        private bool _isObject;
        private bool _isBorrowed;
        private string _base64 = string.Empty;

        private Entry _ent;

        public AddDebt(Choice choice)
        {
            InitializeComponent();
            _isObject = choice.Object;
            _isBorrowed = choice.Borrowed;
        }

        public AddDebt()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (var db = new CollectorContext())
            {
                categoryBox.ItemsSource = db.Categories.ToList();
                currencyBox.ItemsSource = db.Currencies.ToList();
                reminderPicker.Date =  reminderPicker.Date.DateTime.AddDays(1);
            }


            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var choice = e.Parameter as Choice;
            if (choice != null)
            {
               
                _isObject = choice.Object;
                _isBorrowed = choice.Borrowed;

                if (_isBorrowed && _isObject)
                    welcomeBlock.Text = "You borrowed an Object.";
                else if (_isBorrowed && !_isObject)
                    welcomeBlock.Text = "You borrowed some Money.";
                else if (!_isBorrowed && _isObject)
                    welcomeBlock.Text = "You lend an Object.";
                else if (!_isBorrowed && !_isObject)
                    welcomeBlock.Text = "You lend some Money.";

                if (_isObject)
                {
                    currencyBox.Visibility = Visibility.Collapsed;
                    amountBox.Width = 362;
                }
                else
                {
                    categoryBox.Visibility = Visibility.Collapsed;
                    photoButton.Visibility = Visibility.Collapsed;
                    objectnameBox.Visibility = Visibility.Collapsed;
                }

                addButton.Content = "Add";
                photoButton.Content = "Add photo ?";
                textBlock.Text = "COLLECTOR -    add entry";
            }
            else if (e.Parameter is Entry)
            {
                _ent = (Entry)e.Parameter;

                if (_ent.Type.TypeId == 1 && _ent.Object != null)
                    welcomeBlock.Text = "You borrowed an Object.";
                else if (_ent.Type.TypeId == 1 && _ent.Object == null)
                    welcomeBlock.Text = "You borrowed some Money.";
                else if (_ent.Type.TypeId == 2 && _ent.Object != null)
                    welcomeBlock.Text = "You lend an Object.";
                else if (_ent.Type.TypeId == 2 && _ent.Object == null)
                    welcomeBlock.Text = "You lend some Money.";

                if (_ent.Object != null)
                {
                    currencyBox.Visibility = Visibility.Collapsed;
                    amountBox.Width = 362;
                }
                else
                {
                    categoryBox.Visibility = Visibility.Collapsed;
                    photoButton.Visibility = Visibility.Collapsed;
                    objectnameBox.Visibility = Visibility.Collapsed;
                }

                titleBox.Text = _ent.Title;
                nameBox.Text = _ent.Who;
                if (_ent.Object != null)
                {

                    amountBox.Text = _ent.Object.Quantity.ToString();
                    categoryBox.PlaceholderText = _ent.Object.Category.Cname;
                    objectnameBox.Text = _ent.Object.Name;
                    // image.Source = await Base64Converter.FromBase64(_ent.Object.Image);
                }
                else
                {
                    amountBox.Text = _ent.Amount.ToString(CultureInfo.InvariantCulture.NumberFormat);

                    currencyBox.PlaceholderText = _ent.Currency.Cursi;
                }
                initialPicker.Date = new DateTimeOffset(_ent.Date);
                reminderPicker.Date = new DateTimeOffset(_ent.Deadline);

                descriptionBox.Text = _ent.Desc;

                prioritySwitch.IsOn = (_ent.Priority != 0);

                addButton.Content = "Edit";
                photoButton.Content = "Change photo ?";
                textBlock.Text = "COLLECTOR -    edit entry";

            }
            base.OnNavigatedTo(e);
        }



        private async void Add_debt_click(object sender, RoutedEventArgs e)
        {
            if ((string)addButton.Content == "Add")
            {



                var fail = false;
                var objectQuan = 0;
                float moneyAmount = 0;
                Category cat = null;
                Currency cur = null;
                int type;
                try
                {
                    if (!_isObject)
                    {
                        moneyAmount = float.Parse(amountBox.Text, CultureInfo.InvariantCulture.NumberFormat);
                        cur = (Currency)currencyBox.SelectedItem;
                    }
                    else
                    {
                        objectQuan = int.Parse(amountBox.Text);
                        cat = (Category)categoryBox.SelectedItem;
                    }
                }
                catch
                {
                    fail = true;
                    var msgbox = new MessageDialog("Amount should be a number and not empty");

                    msgbox.Commands.Clear();
                    msgbox.Commands.Add(new UICommand { Label = "Cancel", Id = 0 });

                    var res = await msgbox.ShowAsync();

                    if ((int)res.Id == 0)
                    {

                    }

                }


                if (fail) return;

                using (var db = new CollectorContext())
                {
                       
                    type = (_isBorrowed) ? 1 : 2;


                    if (_isObject)
                    {


                        var obj = new Object
                        {
                            Category = db.Categories.First(o => o.Cname == cat.Cname),
                            Name = objectnameBox.Text,
                            Image = _base64,
                            Quantity = objectQuan
                        };
                        var debt = new Entry
                        {
                            Type = db.Types.First(o => o.TypeId == type),
                            Title = titleBox.Text,
                            Who = nameBox.Text,
                            Desc = descriptionBox.Text,
                            Priority = prioritySwitch.IsOn ? 1 : 0,
                            Object = obj,
                            Date = initialPicker.Date.DateTime,
                            Deadline = reminderPicker.Date.DateTime.AddMinutes(hourPicker.Time.TotalMinutes),
                            Is_active = true
                        };
                        db.Objects.Add(obj);
                        db.Entries.Add(debt);


                    }
                    else
                    {

                        var debt = new Entry
                        {
                            Type = db.Types.First(o => o.TypeId == type),
                            Title = titleBox.Text,
                            Who = nameBox.Text,
                            Desc = descriptionBox.Text,
                            Priority = prioritySwitch.IsOn ? 1 : 0,
                            Amount = moneyAmount,
                            Currency = db.Currencies.First(o => o.Cursn == cur.Cursn),
                            Date = initialPicker.Date.DateTime,
                            Deadline = reminderPicker.Date.DateTime.AddMinutes(hourPicker.Time.TotalMinutes),
                            Is_active = true
                        };
                        db.Entries.Add(debt);

                    }

                    db.SaveChanges();
                }


                // var remeinder_task = new Notifify(reminderPicker.Date.DateTime, hourPicker.Time);

                //   remeinder_task.set_Notification();

                Frame.Navigate(typeof(MainPage));
            }
            else
            {
                using (var db = new CollectorContext())
                {
                    _ent.Title = titleBox.Text;
                    _ent.Desc = descriptionBox.Text;
                    _ent.Who = nameBox.Text;
                    _ent.Date = initialPicker.Date.DateTime;
                    _ent.Deadline = reminderPicker.Date.DateTime;
                    _ent.Priority = prioritySwitch.IsOn ? 1 : 0;

                    if (_ent.Object != null)
                    {
                        _ent.Object.Category = (Category)categoryBox.SelectedItem;
                        _ent.Object.Name = objectnameBox.Text;
                        _ent.Object.Quantity = int.Parse(amountBox.Text);
                        _ent.Object.Image = _base64;

                        db.Update(_ent);
                        db.Update(_ent.Object);
                        db.SaveChanges();
                    }
                    else
                    {
                        _ent.Amount = float.Parse(amountBox.Text, CultureInfo.InvariantCulture.NumberFormat);
                        _ent.Currency = (Currency)currencyBox.SelectedItem;
                        db.Update(_ent);
                        db.SaveChanges();
                    }

                    //  var remeinder_task = new Notifify(reminderPicker.Date.DateTime, hourPicker.Time);

                    //remeinder_task.set_Notification();

                    Frame.Navigate(typeof(MainPage));
                }
            }
        }


        private void Cancel_click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));

        }

        private async void photoButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker {SuggestedStartLocation = PickerLocationId.PicturesLibrary};
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            try
            {
                var file = await picker.PickSingleFileAsync();
                var img = await LoadImage(file);
                _base64 = await Base64Converter.ToBase64(file);
                image.Source = img;
            }
            catch (Exception ex)
            {
                var msgbox = new MessageDialog(ex.Message + "\n\n\n Stack Trace:\n" + ex.StackTrace);

                msgbox.Commands.Clear();
                msgbox.Commands.Add(new UICommand { Label = "OK" });
                await msgbox.ShowAsync();
            }
        }


        private static async Task<BitmapImage> LoadImage(StorageFile file)
        {
            var bitmapImage = new BitmapImage();
            var stream = (FileRandomAccessStream)await file.OpenAsync(FileAccessMode.Read);

            bitmapImage.SetSource(stream);

            return bitmapImage;
        }




        private void amountBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {
                var amount = (_isObject) ? int.Parse(amountBox.Text) : float.Parse(amountBox.Text, CultureInfo.InvariantCulture.NumberFormat);
                amountBox.Background = new SolidColorBrush(Colors.White);
            }
            catch
            {
                amountBox.Background = new SolidColorBrush(Colors.PaleVioletRed);
            }
        }

        private void title_TextChanged(object sender, TextChangedEventArgs e)
        {
            titleBox.Background = (titleBox.Text == "") ? new SolidColorBrush(Colors.PaleVioletRed) : new SolidColorBrush(Colors.White);
        }

        private void namebox_TextChanged(object sender, TextChangedEventArgs e)
        {
            nameBox.Background = (nameBox.Text == "") ? new SolidColorBrush(Colors.PaleVioletRed) : new SolidColorBrush(Colors.White);
        }

    }


    public class Notifify
    {
        private readonly Random _random = new Random((int)DateTime.Now.Ticks);
        private DateTime _whenRemind;
        private TimeSpan _hours;
        const string Toast = @"
<toast>
  <visual>
    <binding template=""ToastGeneric"">
      <text>Collector</text>
      <text>You set a reminder</text>
       <image placement = ""AppLogoOverride"" src=""Assets/Square44x44Logo.targetsize-24_altform-unplated.png"" />
      
    </binding>
  </visual>
  <actions>
    <action content = ""check"" arguments=""check""  />
    <action content = ""cancel"" arguments=""cancel""/>
  </actions>
  <audio src =""ms-winsoundevent:Notification.Reminder""/>
</toast>";


        public Notifify(DateTime when, TimeSpan hours)
        {
            _whenRemind = when;
            _hours = hours;
        }

        public void set_Notification()
        {

            var when = _whenRemind.Date.AddMinutes(_hours.TotalMinutes);

            var offset = new DateTimeOffset(when);

            var xml = new XmlDocument();

            xml.LoadXml(Toast);

            var toast = new ScheduledToastNotification(xml, offset)
            {
                Id = _random.Next(1, 100000000).ToString()
            };


            ToastNotificationManager.CreateToastNotifier().AddToSchedule(toast);
        }
    }



}