﻿using System;
using System.Collections;
using System.Collections.Generic;
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
        class Categories_list
        {
            private List<ProjectClasses.Category> categoryList;
        }

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
            categoryBox.ItemsSource = ((List<ProjectClasses.Category>)await
                SerwerFunction.Getfromserver<List<ProjectClasses.Category>>(
                    "Categories/", "GET", null)).ToList();


            // currencyBox.ItemsSource = db.Currencies.ToList();
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

                    amountBox.Text = _ent.Object.Quantity.ToString();
                    categoryBox.PlaceholderText = _ent.Object.Category.cname;
                    objectnameBox.Text = _ent.Object.Name;
                    // image.Source = await Base64Converter.FromBase64(_ent.Object.Image);
                }
                else
                {
                    amountBox.Text = _ent.amount.ToString(CultureInfo.InvariantCulture.NumberFormat);

                   // currencyBox.PlaceholderText = _ent.Currency.Cursn;
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
            float moneyAmount = 0;
            ProjectClasses.Category cat = null;
            ProjectClasses.Currency cur = null;
            int type;
            try
            {



                if (titleBox.Text == "" || nameBox.Text == "")
                    throw new Exception("title and name are NOT optional fields");

                if (!_isObject)
                {
                    moneyAmount = float.Parse(amountBox.Text, CultureInfo.InvariantCulture.NumberFormat);
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
                       // else throw new Exception("You forget to choose Currency");

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
                    type = (_isBorrowed) ? 1 : 2;


                    if (_isObject)
                    {



                       var  obj = new ProjectClasses.Object
                        {
                           // ProjectClasses.Category = 
                            Name = objectnameBox.Text,
                            Image = _base64,
                            Quantity = objectQuan
                        };
                         debt = new ProjectClasses.Entry
                        {
                            //Type = new Type {tid = type },
                            title = titleBox.Text,
                            who = nameBox.Text,
                            descr = descriptionBox.Text,
                            priority = (byte) (prioritySwitch.IsOn ? 1 : 0),
                            Object = obj,
                            date = initialPicker.Date.DateTime,
                            deadline = reminderPicker.Date.DateTime.AddMinutes(hourPicker.Time.TotalMinutes),
                            archived = 0,
                            User = new ProjectClasses.User {uid  = SerwerFunction.Uid, login = SerwerFunction.login, password = SerwerFunction.password}
                        };
                        


                        //db.Objects.Add(obj);
                        // db.Entries.Add(debt);


                    }
                    else
                    {




                         debt = new ProjectClasses.Entry
                        {
                            Type = new ProjectClasses.Type { tid = type },
                            title = titleBox.Text,
                            who = nameBox.Text,
                            descr = descriptionBox.Text,
                            priority = (byte) (prioritySwitch.IsOn ? 1 : 0),
                            amount = (decimal) moneyAmount,
                            //Currency = db.Currencies.First(o => o.Cursn == cur.Cursn),
                            date = initialPicker.Date.DateTime,
                            deadline = reminderPicker.Date.DateTime.AddMinutes(hourPicker.Time.TotalMinutes),
                            archived = 0,
                             User = new ProjectClasses.User { uid = SerwerFunction.Uid, login = SerwerFunction.login, password = SerwerFunction.password },
                           userid = SerwerFunction.Uid
                        };
                        //db.Entries.Add(debt);

                    }





                    var received_debt = (ProjectClasses.Entry)await SerwerFunction.Getfromserver<ProjectClasses.Entry>("Entries" , "POST", debt);


                    //db.SaveChanges();
                


                // var remeinder_task = new Notifify(reminderPicker.date.DateTime, hourPicker.Time);

                //   remeinder_task.set_Notification();

                Frame.Navigate(typeof(MainPage));
            }
            else
            {
               



                    if (!_isObject)
                    {

                        //if (currencyBox.SelectedItem == null)
                        //    cur = db.Currencies.First((o => o.Cursn == currencyBox.PlaceholderText));
                        //else
                        //    cur = (ProjectClasses.Currency)currencyBox.SelectedItem;


                    }
                    else
                    {

                        //if (categoryBox.SelectedItem == null)
                        //    cat = db.Categories.First((o => o.cname == categoryBox.PlaceholderText));
                        //else cat = (ProjectClasses.Category)categoryBox.SelectedItem;
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
                    _ent.Object.Name = objectnameBox.Text;
                    _ent.Object.Quantity = int.Parse(amountBox.Text);
                    _ent.Object.Image = _base64;

                    //db.Update(_ent);
                    //db.Update(_ent.Object);
                    //db.SaveChanges();
                }
                else
                {
                    _ent.amount = (decimal) float.Parse(amountBox.Text, CultureInfo.InvariantCulture.NumberFormat);
                    _ent.Currency = cur;
                    //    db.Update(_ent);
                    //    db.SaveChanges();
                    //
                }

                //  var remeinder_task = new Notifify(reminderPicker.date.DateTime, hourPicker.Time);

                    //remeinder_task.set_Notification();

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