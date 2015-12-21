using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

        private ProjectClasses.Entry _ent;

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

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {

            ProjectClasses.AllUsers = ((List<ProjectClasses.User>)await
               SerwerFunction.Getfromserver<List<ProjectClasses.User>>(
                   "Users", "GET", null)).ToList();

            categoryBox.ItemsSource = ProjectClasses.AllCategories = ((List<ProjectClasses.Category>)await
                SerwerFunction.Getfromserver<List<ProjectClasses.Category>>(
                    "Categories/", "GET", null)).ToList();


             currencyBox.ItemsSource = ProjectClasses.AllCurrencies = ((List<ProjectClasses.Currency>)await
            SerwerFunction.Getfromserver<List<ProjectClasses.Currency>>(
                "Currencies/", "GET", null)).ToList();




            reminderPicker.Date = reminderPicker.Date.DateTime.AddDays(1);




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
            else if (e.Parameter is ProjectClasses.Entry)
            {
                _ent = (ProjectClasses.Entry)e.Parameter;

                _isObject = (_ent.Object != null)? true:false;

                if (_ent.Type.tid == 1 && _ent.Object != null)
                    welcomeBlock.Text = "You borrowed an Object.";
                else if (_ent.Type.tid == 1 && _ent.Object == null)
                    welcomeBlock.Text = "You borrowed some Money.";
                else if (_ent.Type.tid == 2 && _ent.Object != null)
                    welcomeBlock.Text = "You lend an Object.";
                else if (_ent.Type.tid == 2 && _ent.Object == null)
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

                titleBox.Text = _ent.title;
                nameBox.Text = _ent.who;
                if (_ent.Object != null)
                {

                    amountBox.Text = _ent.Object.quantity.ToString();
                    categoryBox.PlaceholderText = _ent.Object.Category.cname;
                    objectnameBox.Text = _ent.Object.name;
                    // image.Source = await Base64Converter.FromBase64(_ent.Object.image);
                }
                else
                {
                    amountBox.Text = _ent.amount.ToString(CultureInfo.InvariantCulture.NumberFormat);

                    currencyBox.PlaceholderText = _ent.Currency.cursign;
                }
                initialPicker.Date = new DateTimeOffset(_ent.date);
                reminderPicker.Date = new DateTimeOffset(_ent.deadline);

                descriptionBox.Text = _ent.descr;

                prioritySwitch.IsOn = (_ent.priority != 0);

                addButton.Content = "Edit";
                photoButton.Content = "Change photo ?";
                textBlock.Text = "COLLECTOR -    edit entry";

            }
            base.OnNavigatedTo(e);
        }



        private async void Add_debt_click(object sender, RoutedEventArgs e)
        {



            var objectQuan = 0;
            decimal moneyAmount = 0;
            ProjectClasses.Category cat = null;
            ProjectClasses.Currency cur = null;
            try
            {



                if (titleBox.Text == "" || nameBox.Text == "")
                    throw new Exception("title and name are NOT optional fields");

                if (!_isObject)
                {
                    moneyAmount = decimal.Parse(amountBox.Text, CultureInfo.InvariantCulture.NumberFormat);
                }
                else
                {
                    objectQuan = int.Parse(amountBox.Text);
                }
            }
            catch (Exception ex)
            {
                ErrorDialog(ex.Message);
                return;
            }

            if ((string)addButton.Content == "Add")
            {

                try
                {

                    if (!_isObject)
                    {

                        if (currencyBox.SelectedItem != null)
                            cur = (ProjectClasses.Currency)currencyBox.SelectedItem;
                        else throw new Exception("You forget to choose Currency");

                    }
                    else
                    {

                        if (categoryBox.SelectedItem != null) cat = (ProjectClasses.Category)categoryBox.SelectedItem;
                        else throw new Exception("You forget to choose Category ");

                    }
                }
                catch (Exception ex)
                {
                    ErrorDialog(ex.Message);
                    return;
                }

              
                    ProjectClasses.Entry debt;
                    var type = (_isBorrowed) ? 1 : 2;


                    if (_isObject)
                    {

                    var cat_final = ProjectClasses.AllCategories.First(o => o.cname == cat.cname);

                    var  obj = new ProjectClasses.Object
                        {
                            name = objectnameBox.Text,
                            image = null,
                            quantity = objectQuan,
                            catid = cat_final.cid,
                        };

                    
                    var received_object = (ProjectClasses.Object)await SerwerFunction.Getfromserver<ProjectClasses.Object>("Objects", "POST", obj);
                    

                    debt = new ProjectClasses.Entry
                        {
                            typeid = type,
                            title = titleBox.Text,
                            objectid = received_object.oid,
                            who = nameBox.Text,
                            descr = descriptionBox.Text,
                            priority = (byte) (prioritySwitch.IsOn ? 1 : 0),
                            date = initialPicker.Date.DateTime,
                            deadline = reminderPicker.Date.DateTime.AddMinutes(hourPicker.Time.TotalMinutes),
                            archived = 0,
                            userid = SerwerFunction.Uid,
                            amount = received_object.quantity
                    };
                        

                    }
                    else
                    {


                       var cur_final =  ProjectClasses.AllCurrencies.First(o => o.cursign == cur.cursign);

                         debt = new ProjectClasses.Entry
                        {
                            typeid = type,
                            title = titleBox.Text,
                            date = initialPicker.Date.DateTime,
                            who = nameBox.Text,
                            amount = moneyAmount,
                            descr = descriptionBox.Text,
                            priority = (byte) (prioritySwitch.IsOn ? 1 : 0),
                            deadline = reminderPicker.Date.DateTime.AddMinutes(hourPicker.Time.TotalMinutes),
                            currencyid = cur_final.crid,
                            archived = 0,
                            userid = SerwerFunction.Uid,
                        
                            
                        };

                    
                }


                


                var received_debt = (ProjectClasses.Entry)await SerwerFunction.Getfromserver<ProjectClasses.Entry>("Entries" , "POST", debt);


                   
                


                // var remeinder_task = new Notifify(reminderPicker.date.DateTime, hourPicker.Time);

                //   remeinder_task.set_Notification();

                Frame.Navigate(typeof(MainPage));
            }
            else
            {
               



                    if (!_isObject)
                    {

                    if (currencyBox.SelectedItem == null)
                        cur = ProjectClasses.AllCurrencies.First((o => o.cursign == currencyBox.PlaceholderText));
                    else
                        cur = (ProjectClasses.Currency)currencyBox.SelectedItem;


                }
                else
                    {

                    if (categoryBox.SelectedItem == null)
                        cat = ProjectClasses.AllCategories.First((o => o.cname == categoryBox.PlaceholderText));
                    else cat = (ProjectClasses.Category)categoryBox.SelectedItem;
                }

                _ent.title = titleBox.Text;
                    _ent.descr = descriptionBox.Text;
                    _ent.who = nameBox.Text;
                    _ent.date = initialPicker.Date.DateTime;
                    _ent.deadline = reminderPicker.Date.DateTime;
                    _ent.priority = (byte) (prioritySwitch.IsOn ? 1 : 0);

                if (_ent.Object != null)
                {
                    _ent.Object.Category = cat;
                    _ent.Object.name = objectnameBox.Text;
                    _ent.Object.quantity = int.Parse(amountBox.Text);
                   //_ent.Object.image = _base64;

                    
                }
                else
                {
                    _ent.amount = (decimal) float.Parse(amountBox.Text, CultureInfo.InvariantCulture.NumberFormat);
                    _ent.Currency = cur;
                   
                }

                //  var remeinder_task = new Notifify(reminderPicker.date.DateTime, hourPicker.Time);

                //remeinder_task.set_Notification();
                _ent.Type = null;
                _ent.Object = null;
                _ent.Currency = null;
                _ent.User = null;
               //TODO: wez id z obiektu i dodaj do puta;

                var received_debt = (ProjectClasses.Entry)await SerwerFunction.Getfromserver<ProjectClasses.Entry>("Entries/" +, "PUT", _ent);

                Frame.Navigate(typeof(MainPage));
                
            }
        }

        private static void ErrorDialog(string message)
        {

            var problem = message == "Input string was not in a correct format." ? "There is something wrong with numbers" : message;

            var msgbox = new MessageDialog(problem);

            msgbox.Commands.Clear();
            msgbox.Commands.Add(new UICommand { Label = "Cancel", Id = 0 });

            var res =  msgbox.ShowAsync();

            if ((int)res.Id == 0)
            {
              
            }
            return;

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


        private static async Task<BitmapImage> LoadImage(IStorageFile file)
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


    

        private void namebox_TextChanged(object sender, TextChangedEventArgs e)
        {   var but = (TextBox)sender;
            but.Background = (titleBox.Text == "") ? new SolidColorBrush(Colors.PaleVioletRed) : new SolidColorBrush(Colors.White);
        }


        private void fill_users(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (sender.Text.Length > 0)
                    sender.ItemsSource = GetSuggestions(sender.Text);
                else
                {
                    sender.ItemsSource = new string[] {"No suggestions..."};

                    sender.Background = (sender.Text == "")
                        ? new SolidColorBrush(Colors.PaleVioletRed)
                        : new SolidColorBrush(Colors.White);
                    sender.UpdateLayout();

                }

            }
        }


        private static string[] GetSuggestions(string text)
        {
            return (ProjectClasses.AllUsers.Where(x => x.login.Contains(text) && x.login != SerwerFunction.login)).Select(s => s.login).ToArray().Distinct().ToArray();
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