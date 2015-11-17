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
            is_object = choice.Object;
            is_borrowed = choice.Borrowed;
            this.InitializeComponent();
        }

        public Add_debt()
        {
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (is_borrowed && is_object)
                welcomeBlock.Text = "You borrowed an Object.";
            else if (is_borrowed && !is_object)
                welcomeBlock.Text = "You borrowed some Money.";
            else if (!is_borrowed && is_object)
                welcomeBlock.Text = "You lend some Object.";
            else if (!is_borrowed && !is_object)
                welcomeBlock.Text = "You lend some Money.";

            if(is_object)
            {
                currencyBox.Visibility = Visibility.Collapsed;
                amountBox.Width = 362;




            }


        }


        private async void Add_debt_click(object sender, RoutedEventArgs e)
        {
            bool fail = false;
            float temp;
            try
            {
                temp = float.Parse(amountBox.Text, CultureInfo.InvariantCulture.NumberFormat);

            }
            catch
            {
                fail = true;
                MessageDialog msgbox = new MessageDialog("Amount should be a number and not empty");

                msgbox.Commands.Clear();
                msgbox.Commands.Add(new UICommand { Label = "Cancel", Id = 1 });

                var res = await msgbox.ShowAsync();

                if ((int)res.Id == 1)
                {
                   
                    
                }

            }



            if (!fail)
            {
                using (var db = new CollectorContext())
                {
                    var debt = new Entry
                    {
                        Title = titleBox.Text,
                        Who = nameBox.Text,
                        Desc = descriptionBox.Text,
                        Priority = prioritySwitch.IsOn ? 1 : 0,




                    };
                    db.Entries.Add(debt);

                    db.SaveChanges();



                }

                this.Content = new MainPage();
            }
        }

        private void Cancel_click(object sender, RoutedEventArgs e)
        {

            this.Content = new MainPage();
           

        }

        private async void photoButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            StorageFile file = await picker.PickSingleFileAsync();

            BitmapImage img = new BitmapImage();
            img = await LoadImage(file);

            image.Source = img;

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