using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Collector_local_db
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Add_debt : Page
    {
        public Add_debt()
        {
            this.InitializeComponent();
        }


       


        private void Add_debt_click(object sender, RoutedEventArgs e)
        {




            using (var db = new CollectorContext())
            {
                var debt = new Entry
                {
                    Title = titleBox.Text,
                    Who = nameBox.Text,
                    Desc = descriptionBox.Text,
                    Priority = prioritySwitch.IsOn ? 1 : 0,
                    Amount =  float.Parse(amountBox.Text, CultureInfo.InvariantCulture.NumberFormat)
                   

            };
                db.Entries.Add(debt);

                db.SaveChanges();

             

            }
        }

        private void Cancel_click(object sender, RoutedEventArgs e)
        {

            this.Content = new MainPage(); 



        }


    }

}