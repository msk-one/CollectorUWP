using System;
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
            if (is_borrowed && is_object)
                welcomeBlock.Text = "You borrowed an Object.";
            else if (is_borrowed && !is_object)
                welcomeBlock.Text = "You borrowed some Money.";
            else if (!is_borrowed && is_object)
                welcomeBlock.Text = "You lend an Object.";
            else if (!is_borrowed && !is_object)
                welcomeBlock.Text = "You lend some Money.";

            if(is_object)
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

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Choice)
            {
                Choice chc = (Choice) e.Parameter;
                is_object = chc.Object;
                is_borrowed = chc.Borrowed;
            }
            base.OnNavigatedTo(e);
        }


        private  void Add_debt_click(object sender, RoutedEventArgs e)
        {
            bool fail = false;
            int object_quan = 0;
            float money_amount =0;
            try
            {

                if (!is_object)
                    money_amount = float.Parse(amountBox.Text, CultureInfo.InvariantCulture.NumberFormat);
                else
                    object_quan = int.Parse(amountBox.Text);
                    
                    
                    
            }
            catch
            {
                fail = true;
                MessageDialog msgbox = new MessageDialog("Amount should be a number and not empty");

                msgbox.Commands.Clear();
                msgbox.Commands.Add(new UICommand { Label = "Cancel"});

                

            }



            if (!fail)
            {

                using (var db = new CollectorContext())
                {
<<<<<<< HEAD
                    if (is_object == true)
                    {
                        var debt = new Entry
                        {
                            Title = titleBox.Text,
                            Who = nameBox.Text,
                            Desc = descriptionBox.Text,
                            Priority = prioritySwitch.IsOn ? 1 : 0,
                            Object = new Object() { Category = db.Categories.First(o => o.Cname == (string) categoryBox.SelectedItem ), Image = " "},
                            Date = initialPicker.Date.DateTime,
                            Deadline = reminderPicker.Date.DateTime
                        };
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
                            Amount = temp,
                            Date = initialPicker.Date.DateTime,
                            Deadline = reminderPicker.Date.DateTime
                        };
                        db.Entries.Add(debt);
                        db.SaveChanges();
                    }
=======

                    

                    var debt = new Entry
                    {
                        Title = titleBox.Text,
                        Who = nameBox.Text,
                        Desc = descriptionBox.Text,
                        Priority = prioritySwitch.IsOn ? 1 : 0,
                        Amount = money_amount


>>>>>>> c33ea3d633d9fdaf653b1dfb63a4cbe1c8116aea

                }

               
                Frame.Navigate(typeof(MainPage));
            }
        }

        private void Cancel_click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
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
            try {
                 file = await picker.PickSingleFileAsync();
                BitmapImage img = new BitmapImage();
                img = await LoadImage(file);

                image.Source = img;
            }
            catch { }
            

        }



        private static async Task<BitmapImage> LoadImage(StorageFile file)
        {
            BitmapImage bitmapImage = new BitmapImage();
            FileRandomAccessStream stream = (FileRandomAccessStream)await file.OpenAsync(FileAccessMode.Read);

            bitmapImage.SetSource(stream);

            return bitmapImage;

        }


    }



}