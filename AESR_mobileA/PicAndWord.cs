using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using SQLite;

namespace AESR_mobileA
{
    [Table("All_pictures")]
    public class PicAndWord
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        public string pathToPic { get; set; }

        [MaxLength(20)]
        public string word { get; set; }

        public string clusterName { get; set; }

        public int Id_in_curr_cluster { get; set; }
    }
}