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
    [Table("All_Clusters")]
    public class Cluster
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        public string clusterName { get; set; }

        public int picsInCluster { get; set; }

        public string pathToIcon { get; set; }

        public long clsIndex { get; set; }
    }
}