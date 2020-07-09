using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

using SQLite;
using Android.Graphics;
using Android.Media;

using MagicDataA;

namespace AESR_mobileA
{
    //Do not forgot to incule Support.v7 library for RecycleView ~~already~~
    //how to install supporting package search link: https://docs.microsoft.com/en-us/xamarin/android/user-interface/layouts/recycler-view/

    [Activity(Label = "@string/start_screen_title", Theme = "@style/AppTheme", MainLauncher = true)]
    public class SelectClusterActivity : Activity
    {
        RecyclerView recycler;
        RecyclerView.LayoutManager layoutManager;

        ClusterViewAdapter adapter;

        public List<Cluster> clusters = new List<Cluster>();
        public List<PicAndWord> picAndWords = new List<PicAndWord>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Window.RequestFeature(WindowFeatures.NoTitle);

            // Create your application here
            SetContentView(Resource.Layout.activity_select_cluster);

            var path = GetExternalFilesDir("DirectoryPictures"); //May be error

            string[] files = Directory.GetFiles(path.AbsolutePath);

            var smallpath = GetExternalFilesDir(null).AbsolutePath;

            var combinedpath = System.IO.Path.Combine(smallpath, "aesrclusters.db3");

            var db = new SQLiteConnection(combinedpath);

            string[] find = System.IO.Directory.GetFiles(path.AbsolutePath);

            bool dbexists = false;
            try
            {
                var any = db.Table<Cluster>().ToList();//I dont sure how it works. maybe it doesnt work
                if (any.Count > 0)
                {
                    dbexists = true;
                }
            }
            catch(Exception)
            {
               //throw;
            }

            if (/*find.Length == 0*/!dbexists)//How to download image or class to IO memory?
            {
                using (FileStream fs = File.Create(System.IO.Path.Combine(path.AbsolutePath, "defpic.png")))
                {
                    Bitmap bmp = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.defpic);

                    byte[] mypic = Support.BitmapToByte(bmp);

                    // Add some information to the file.//Impossible to add class to byte[]
                    fs.Write(mypic, 0, mypic.Length);
                }

                db.CreateTable<Cluster>();
                db.CreateTable<PicAndWord>();

                //MagicDataA.Generate.Name()

                Cluster cluster = new Cluster();
                cluster.clusterName = "defaultCluster";
                cluster.picsInCluster = 1;
                cluster.pathToIcon = System.IO.Path.Combine(path.AbsolutePath, "defpic.png");
                cluster.uniqID = MagicDataA.Generate.Name("cluster", ".0");

                PicAndWord pic = new PicAndWord();
                pic.clusterUniqID = cluster.uniqID;
                pic.uniqID = MagicDataA.Generate.Name("picture", ".0");
                pic.pathToPic = System.IO.Path.Combine(path.AbsolutePath, "defpic.png");
                pic.word = "default";

                db.Insert(cluster);
                db.Insert(pic);
            }

            clusters = db.Table<Cluster>().ToList();

            picAndWords = db.Table<PicAndWord>().ToList();

            db.Close();

            recycler = this.FindViewById<RecyclerView>(Resource.Id.clusterRECYCLER);

            layoutManager = new LinearLayoutManager(this);
            recycler.SetLayoutManager(layoutManager);

            adapter = new ClusterViewAdapter(this);
            adapter.ItemClick += OnItemClick;//In future: move it to OnResume or OnStart method && put adapter.[something]Click -= Adapter_[something]Click to OnPause
            //adapter.ItemClick -= OnItemClick;
            recycler.SetAdapter(adapter);

            Button addNewCluster = FindViewById<Button>(Resource.Id.addclusterBUTTON);
            addNewCluster.Click += AddNewCluster_Click;

            //Im not sure that code above working right//I take 95% that it is not working
            {
                //db.UpdateAll(clusters);
                //db.UpdateAll(picAndWords);
            }
        }

        private void AddNewCluster_Click(object sender, EventArgs e)
        {
            int iter = 0;

            while (clusters.Exists(x => x.clusterName == "new cluster " + iter))
            {
                iter++;
            }

            Cluster newcls = new Cluster();
            newcls.clusterName = "new cluster " + iter;
            newcls.picsInCluster = 0;
            newcls.pathToIcon = System.IO.Path.Combine(GetExternalFilesDir("DirectoryPictures").AbsolutePath, "defpic.png");
            newcls.uniqID = Generate.Name("cluster", ".0");

            var db = new SQLiteConnection(System.IO.Path.Combine(GetExternalFilesDir(null).AbsolutePath, "aesrclusters.db3"));

            //I take 95% that it is not working as i expect
            db.Insert(newcls);

            Intent next = new Intent(this, typeof(EditClusterActivity));
            next.PutExtra("positionOfCluster", /*db.Table<Cluster>().ToList().Find(x => x.clusterName == "new cluster " + iter).Id - 1*/clusters.Count);

            db.Close();

            StartActivity(next);

            next.Dispose();//It is really need? Maybe, it can broke code
        }

        void OnItemClick(object sender, int position)
        {
            Intent next = new Intent(this, typeof(EditClusterActivity));
            next.PutExtra("positionOfCluster", position);

            StartActivity(next);
        }
    }
}