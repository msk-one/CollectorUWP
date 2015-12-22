using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Collector_local_db
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Charts : Page
    {
        public Charts()
        {
            this.InitializeComponent();
        }


        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {


            ProgressUpload.IsIndeterminate = true; 


            var test1 = ((List<ProjectClasses.Entry>) await
                SerwerFunction.Getfromserver<List<ProjectClasses.Entry>>(
                    "GetEntriesMoneyForUser/" + SerwerFunction.Uid + "/AndType/1", "GET", null));

            var test2 = ((List<ProjectClasses.Entry>) await
                SerwerFunction.Getfromserver<List<ProjectClasses.Entry>>(
                    "GetEntriesMoneyForUser/" + SerwerFunction.Uid + "/AndType/2", "GET", null));


            int totalEntrys = (int) await
                SerwerFunction.Getfromserver<int>(
                    "GetTotalEntriesCountForUser/" + SerwerFunction.Uid, "GET", null);

            int something =(int) await
                SerwerFunction.Getfromserver<int>(
                    "GetTotalEntriesCountForUser/" + SerwerFunction.Uid + "/AndType/1", "GET", null) ;

            int something2 = (int)await
                SerwerFunction.Getfromserver<int>(
                    "GetTotalEntriesCountForUser/" + SerwerFunction.Uid + "/AndType/2", "GET", null);

            int something3 = (int) await
                SerwerFunction.Getfromserver<int>(
                    "GetTotalEntriesCountForUserArchived/" + SerwerFunction.Uid, "GET", null);

            int countMoneyBorrow = test1?.Count ?? 0;

            int countMoneyLend = test2?.Count ?? 0;

           
            opening.Text = "Dear " + SerwerFunction.login + " u have: ";

            

            decimal moneyborrowSum = (countMoneyBorrow > 0) ? test1.Select(o => o.amount).Sum() : 0;





            decimal moneylendSum = (countMoneyLend > 0) ? test2.Select(o => o.amount).Sum() : 0;

            EntriesCount.Text = totalEntrys + " debts in general";
            EntriestBorrowCoun.Text = something + " entrys borrowed";

            EntriesLendCount.Text = something2 + " entrys lend";


            EntriesBorrowMoney.Text = countMoneyBorrow.ToString() +" money entries borrowed";

            EntriesLendMoney.Text = countMoneyLend.ToString() + " money entries lend";

            EntriesBorrowObject.Text = (totalEntrys - countMoneyBorrow).ToString() + " object entries borrowed";

            EntriesLendObject.Text =  (totalEntrys - countMoneyLend).ToString() + " object entries lend";

            EntriesArchivedCount.Text = something3 + " debts that are in the trash";

            CategoriesCount.Text = ((List<ProjectClasses.Category>)await
                SerwerFunction.Getfromserver<List<ProjectClasses.Category>>(
                    "Categories/", "GET", null)).Count.ToString() + " categories";

            EntryBorrowMoneySum.Text = moneyborrowSum.ToString(CultureInfo.InvariantCulture) + " money borrowed";
            EntryLendMoneySum.Text = moneylendSum.ToString(CultureInfo.InvariantCulture) + " money lend";

            ProgressUpload.IsIndeterminate = false;

        }



    }
}
