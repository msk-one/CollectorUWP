using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collector_local_db
{
   public class ProjectClasses
    {
        public class Entry
        {
            public int id { get; set; }
            public int typeid { get; set; }
            public string title { get; set; }
            public DateTime date { get; set; }
            public string descr { get; set; }
            public string who { get; set; }
            public decimal amount { get; set; }
            public byte priority { get; set; }
            public DateTime deadline { get; set; }
            public int currencyid { get; set; }
            public byte archived { get; set; }
            public int userid { get; set; }
            public Currency Currency { get; set; }
            public Object Object { get; set; }
            public Type Type { get; set; }
            public User User { get; set; }
        }


        public class User
        {
            
            public int uid { get; set; }
            public string login { get; set; }
            public string password { get; set; }
            public byte active { get; set; }
            public List<Entry> Entries { get; set; }

        }
       public static  List<ProjectClasses.User> AllUsers ;

        public class Object
        {
          
            public string name { get; set; }
            //public string image { get; set; }
            public  int catid { get; set; }
            public int quantity { get; set; }
            public Category Category { get; set; }
        }

        public class Category
        {
            public List<Object> Objects { get; set; }
            public int cid { get; set; }
            public string cname { get; set; }
            
        }


        public static List<ProjectClasses.Category> AllCategories;

        public class Type
        {
          
            public int tid { get; set; }
            public string typename { get; set; }
            public List<Entry> Entries { get; set; }

        }

        public class Currency
        {
            public string cursign { get; set; }
            public string cursname { get; set; }
            public string curlname { get; set; }
            public int crid { get; set; }

        }

        public static List<ProjectClasses.Currency> AllCurrencies;
    }
}
