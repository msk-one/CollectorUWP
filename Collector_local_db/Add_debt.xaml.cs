﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
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
    public sealed partial class Add_debt : Page
    {
        private bool is_object = false;
        private bool is_borrowed = false;
        private string base64 = String.Empty;

        private Entry ent = null;

        public Add_debt(Choice choice)
        {
            this.InitializeComponent();
            is_object = choice.Object;
            is_borrowed = choice.Borrowed;
        }

        public Add_debt()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (var db = new CollectorContext())
            {
                categoryBox.ItemsSource = db.Categories.ToList();
            }


            //bool cgb = Frame.CanGoBack;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Choice)
            {
                Choice chc = (Choice) e.Parameter;
                is_object = chc.Object;
                is_borrowed = chc.Borrowed;

                if (is_borrowed && is_object)
                    welcomeBlock.Text = "You borrowed an Object.";
                else if (is_borrowed && !is_object)
                    welcomeBlock.Text = "You borrowed some Money.";
                else if (!is_borrowed && is_object)
                    welcomeBlock.Text = "You lend an Object.";
                else if (!is_borrowed && !is_object)
                    welcomeBlock.Text = "You lend some Money.";

                if (is_object)
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
                ent = (Entry) e.Parameter;

                if (ent.Type.TypeId == 1 && ent.Object != null)
                        welcomeBlock.Text = "You borrowed an Object.";
                else if (ent.Type.TypeId == 1 && ent.Object == null)
                        welcomeBlock.Text = "You borrowed some Money.";
                else if (ent.Type.TypeId == 2 && ent.Object != null)
                        welcomeBlock.Text = "You lend an Object.";
                else if (ent.Type.TypeId == 2 && ent.Object == null)
                        welcomeBlock.Text = "You lend some Money.";

                if (ent.Object != null)
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

                titleBox.Text = ent.Title;
                nameBox.Text = ent.Who;
                if (ent.Object != null)
                {
                    amountBox.Text = ent.Object.Quantity.ToString();
                    categoryBox.SelectedItem = ent.Object.Category;
                    objectnameBox.Text = ent.Object.Name;
                    image.Source = await Base64Converter.FromBase64(ent.Object.Image);
                }
                else
                {
                    amountBox.Text = ent.Amount.ToString(CultureInfo.InvariantCulture.NumberFormat);
                }
                initialPicker.Date = new DateTimeOffset(ent.Date);
                reminderPicker.Date = new DateTimeOffset(ent.Deadline);
                //hourpicker
                descriptionBox.Text = ent.Desc;
                if (ent.Priority == 0)
                {
                    prioritySwitch.IsOn = false;
                }
                else
                {
                    prioritySwitch.IsOn = true;
                }
                addButton.Content = "Edit";
                photoButton.Content = "Change photo ?";
                textBlock.Text = "COLLECTOR -    edit entry";

            }
            base.OnNavigatedTo(e);
        }

        private async void Add_debt_click(object sender, RoutedEventArgs e)
        {
            if ((string) addButton.Content == "Add")
            {
                bool fail = false;
                int object_quan = 0;
                float money_amount = 0;
                Category cat = null;
                int type = -1;
                try
                {
                    if (!is_object)
                        money_amount = float.Parse(amountBox.Text, CultureInfo.InvariantCulture.NumberFormat);
                    else
                    {
                        object_quan = int.Parse(amountBox.Text);
                        cat = (Category) categoryBox.SelectedItem;
                    }
                }
                catch
                {
                    fail = true;
                    MessageDialog msgbox = new MessageDialog("Amount should be a number and not empty");

                    msgbox.Commands.Clear();
                    msgbox.Commands.Add(new UICommand {Label = "Cancel", Id = 0});

                    var res = await msgbox.ShowAsync();

                    if ((int) res.Id == 0)
                    {

                    }

                }


                if (!fail)
                {
                    using (var db = new CollectorContext())
                    {


                        if (is_borrowed)
                            type = 1;
                        else
                            type = 2;


                        if (is_object)
                        {
                            var obj = new Object()
                            {
                                Category = db.Categories.First(o => o.Cname == cat.Cname),
                                Name = objectnameBox.Text,
                                Image = base64,
                                Quantity = object_quan
                            };
                            var debt = new Entry
                            {
                                Title = titleBox.Text,
                                Who = nameBox.Text,
                                Desc = descriptionBox.Text,
                                Priority = prioritySwitch.IsOn ? 1 : 0,
                                Object = obj,
                                Date = initialPicker.Date.DateTime,
                                Deadline = reminderPicker.Date.DateTime
                            };
                            db.Objects.Add(obj);
                            db.Entries.Add(debt);

                            db.SaveChanges();
                        }
                        else
                        {
                            var debt = new Entry
                            {
                                Title = titleBox.Text,
                                Who = nameBox.Text,
                                Desc = descriptionBox.Text,
                                Priority = prioritySwitch.IsOn ? 1 : 0,
                                Amount = money_amount,
                                Date = initialPicker.Date.DateTime,
                                Deadline = reminderPicker.Date.DateTime
                            };
                            db.Entries.Add(debt);
                            db.SaveChanges();
                        }
                    }


                    var remeinder_task = new Notifify(reminderPicker.Date.DateTime, hourPicker.Time);

                    remeinder_task.set_Notification();

                    Frame.Navigate(typeof (MainPage));
                }

            }
            else
            {
                using (var db = new CollectorContext())
                {
                    ent.Title = titleBox.Text;
                    ent.Desc = descriptionBox.Text;
                    ent.Who = nameBox.Text;
                    ent.Date = initialPicker.Date.DateTime;
                    ent.Deadline = reminderPicker.Date.DateTime;
                    ent.Priority = prioritySwitch.IsOn ? 1 : 0;

                    if (ent.Object != null)
                    {                    
                        ent.Object.Category = (Category) categoryBox.SelectedItem;
                        ent.Object.Name = objectnameBox.Text;
                        ent.Object.Quantity = int.Parse(amountBox.Text);
                        ent.Object.Image = base64;

                        db.Update(ent);
                        db.Update(ent.Object);
                        db.SaveChanges();
                    }
                    else
                    {
                        ent.Amount = float.Parse(amountBox.Text, CultureInfo.InvariantCulture.NumberFormat);

                        db.Update(ent);
                        db.SaveChanges();
                    }

                    var remeinder_task = new Notifify(reminderPicker.Date.DateTime, hourPicker.Time);

                    remeinder_task.set_Notification();

                    Frame.Navigate(typeof(MainPage));
                }
            }
        }


        private void Cancel_click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (MainPage));
            //this.Content = new MainPage();
        }

        private async void photoButton_Click(object sender, RoutedEventArgs e)
        {
            StorageFile file = null;
            FileOpenPicker picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            try
            {
                file = await picker.PickSingleFileAsync();
                BitmapImage img = new BitmapImage();
                img = await LoadImage(file);
                base64 = await Base64Converter.ToBase64(file);
                image.Source = img;
            }
            catch (Exception ex)
            {
                MessageDialog msgbox = new MessageDialog(ex.Message + "\n\n\n Stack Trace:\n" + ex.StackTrace);

                msgbox.Commands.Clear();
                msgbox.Commands.Add(new UICommand {Label = "OK"});
                var res = await msgbox.ShowAsync();
            }
        }


        private static async Task<BitmapImage> LoadImage(StorageFile file)
        {
            BitmapImage bitmapImage = new BitmapImage();
            FileRandomAccessStream stream = (FileRandomAccessStream) await file.OpenAsync(FileAccessMode.Read);

            bitmapImage.SetSource(stream);

            return bitmapImage;
        }
    }



    public class Notifify
    {
        private Random random = new Random((int)DateTime.Now.Ticks);
        private DateTime when_remind;
        TimeSpan hours;
        const string TOAST = @"
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


        public Notifify(DateTime when,TimeSpan hours)
        {
            this.when_remind = when;
            this.hours = hours;
        }

        public void set_Notification()
        {
            
            var when = when_remind.Date.AddMinutes(hours.TotalMinutes);

            var offset = new DateTimeOffset(when);

            Windows.Data.Xml.Dom.XmlDocument xml = new Windows.Data.Xml.Dom.XmlDocument();

            xml.LoadXml(TOAST);

            ScheduledToastNotification toast = new ScheduledToastNotification(xml, offset);

            toast.Id = random.Next(1, 100000000).ToString();

            ToastNotificationManager.CreateToastNotifier().AddToSchedule(toast);
        }
    }



}