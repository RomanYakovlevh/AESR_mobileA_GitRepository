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
using Android.Support.V7.Widget;

using SQLite;
using System.IO;
using Android.Support.V4.Content;
using Android;
using Android.Content.PM;
using Android.Support.V4.App;

namespace AESR_mobileA
{
    [Activity(Label = "@string/start_screen_title", Theme = "@style/AppTheme")]
    class EditClusterActivity : Activity
    {
        RecyclerView recycler;
        RecyclerView.LayoutManager layoutManager;

        PictureViewAdapter adapter;

        List<Cluster> clusters = new List<Cluster>();

        int position;
        int lastpicpos;

        static EventHandler<View.KeyEventArgs> EditViewText;
        static EventHandler<Android.Text.TextChangedEventArgs> TitleViewText;

        EditText title;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Window.RequestFeature(WindowFeatures.NoTitle);

            // Create your application here
            SetContentView(Resource.Layout.edit_cluster);

            recycler = this.FindViewById<RecyclerView>(Resource.Id.picturesRECYCLER);

            position = Intent.GetIntExtra("positionOfCluster", 0);

            // Plug in the linear layout manager:
            layoutManager = new LinearLayoutManager(this);
            recycler.SetLayoutManager(layoutManager);

            // Plug in my adapter:
            adapter = new PictureViewAdapter(this, position);
            adapter.imageClick += Adapter_imageClick;//In future: move it to OnResume or OnStart method && put adapter.[something]Click -= Adapter_[something]Click to OnPause
            adapter.textClick += Adapter_textClick;
            adapter.deleteClick += Adapter_deleteClick;
            recycler.SetAdapter(adapter);

            Button backButton = FindViewById<Button>(Resource.Id.backBUTTON);
            Button addwordButton = FindViewById<Button>(Resource.Id.addwordBUTTON);
            Button playButton = FindViewById<Button>(Resource.Id.playBUTTON);
            title = FindViewById<EditText>(Resource.Id.clusterTitleTEXTVIEW); //reason to check title.DefaultFocusHighlightEnabled one more time, bec. I found that there was TextView in the place for EditText

            var db = new SQLiteConnection(System.IO.Path.Combine(GetExternalFilesDir(null).AbsolutePath, "aesrclusters.db3"));

            clusters = db.Table<Cluster>().ToList();

            db.Close();

            title.Text = clusters[position].clusterName;//Text editing is not just by touch. It will get a some code

            title.Click += (s, e) =>
            {
                title.FocusableInTouchMode = true;
                bool focres = title.RequestFocus(); //Java.Lang.NoSuchMethodException??
                if (focres == true)
                {
                    title.KeyPress += Title_TextChanged;
                }
            };

            backButton.Click += BackButton_Click;
            addwordButton.Click += AddwordButton_Click;
            playButton.Click += PlayButton_Click;
        }

        private void Title_TextChanged(object sender, View.KeyEventArgs e)
        {
            e.Handled = false;
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                var db = new SQLiteConnection(System.IO.Path.Combine(GetExternalFilesDir(null).AbsolutePath, "aesrclusters.db3")); //This code calling twice?! Or not?

                string lastclustername = clusters[position].clusterName;

                clusters[position].clusterName = title.Text;

                var picwords = db.Table<PicAndWord>().ToList();

                foreach (var item in picwords)
                {
                    if (item.clusterName == lastclustername)
                    {
                        item.clusterName = clusters[position].clusterName;
                    }
                }

                db.Update(clusters[position]);

                db.UpdateAll(picwords);

                db.Close();

                EditText objectWhoSentEvent = sender as EditText; //я не знаю какой у тебя базовый класс такчто поменяй Android.Text если нужно
                objectWhoSentEvent.KeyPress -= Title_TextChanged;
                objectWhoSentEvent.FocusableInTouchMode = false;
            }
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            Intent next = new Intent(this, typeof(SelectClusterActivity));

            StartActivity(next);
        }

        private void AddwordButton_Click(object sender, EventArgs e)
        {
            PicAndWord pic = new PicAndWord();
            pic.clusterName = clusters[position].clusterName;
            pic.Id_in_curr_cluster = 0;
            pic.pathToPic = System.IO.Path.Combine(GetExternalFilesDir("DirectoryPictures").AbsolutePath, "defpic.png");
            pic.word = "new word";

            var db = new SQLiteConnection(System.IO.Path.Combine(GetExternalFilesDir(null).AbsolutePath, "aesrclusters.db3"));
            db.Insert(pic);//Rewrite it to update only picpos element
            db.Close();

            adapter.NotifyDataSetChanged();//Is more effective version of recycler updateing: ..NotifyItemChanged(int position);
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            Intent next = new Intent(this, typeof(SelectClusterActivity));

            StartActivity(next);
        }

        private void Adapter_deleteClick(object sender, int e)
        {
            throw new NotImplementedException();
        }

        private void Adapter_textClick(object sender, int pos)
        {
            EditText editText = layoutManager.FindViewByPosition(pos).FindViewById<EditText>(Resource.Id.picframeText);

            EditViewText = (object sender, View.KeyEventArgs e) => EditText_Changed(sender, e, pos, editText);

            //editText.TextChanged += EditViewText;//I not exctly know how it works - this event is call by each letter changed, or when user write word fully, and press enter and exit from keybrd.?

            editText.FocusableInTouchMode = true;
            bool focres = editText.RequestFocus(); //Java.Lang.NoSuchMethodException??
            if (focres == true)
            {
                editText.KeyPress += EditViewText;
            }
        }

        void EditText_Changed(object sender, Android.Views.View.KeyEventArgs e, int pos, EditText editText)
        {
            e.Handled = false;
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                var db = new SQLiteConnection(System.IO.Path.Combine(GetExternalFilesDir(null).AbsolutePath, "aesrclusters.db3"));

                List<PicAndWord> picAndWords = db.Table<PicAndWord>().ToList().FindAll(x => x.clusterName == clusters[position].clusterName);

                picAndWords[pos].word = editText.Text;

                db.Update(picAndWords[pos]);//Rewrite it to update only picpos element
                db.Close();

                adapter.NotifyDataSetChanged();//Is more effective version of recycler updateing: ..NotifyItemChanged(int position);

                EditText objectWhoSentEvent = sender as EditText; //я не знаю какой у тебя базовый класс такчто поменяй Android.Text если нужно
                objectWhoSentEvent.KeyPress -= EditViewText;
                editText.FocusableInTouchMode = false;
            }
        }

        private void Adapter_imageClick(object sender, int e)
        {

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != (int)Permission.Granted)
            {
                // Permission is not granted
                //Follow next link to know how to implement permission request by more comfortable for user way
                //https://developer.android.com/training/permissions/requesting.html?hl=en-us#kotlin
                //To know how to do it on C# use Xamarin Android docs -> Permissions

                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.ReadExternalStorage }, 1);
            }
            else
            {
                Intent pickPhoto = new Intent(Intent.ActionPick, Android.Provider.MediaStore.Images.Media.ExternalContentUri);

                lastpicpos = e;

                pickPhoto.PutExtra("picpos", e);

                StartActivityForResult(pickPhoto, 0);//one can be replaced with any action code
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent imageReturnedIntent)
        {
            switch (requestCode)
            {
                case 0:
                    if (resultCode == Result.Ok)
                    {
                        var db = new SQLiteConnection(System.IO.Path.Combine(GetExternalFilesDir(null).AbsolutePath, "aesrclusters.db3"));

                        List<PicAndWord> picAndWords = db.Table<PicAndWord>().ToList().FindAll(x => x.clusterName == clusters[position].clusterName); ;

                        //In current version picked image holded only in users gallery. If user delete image, image will be unaccessible in app.
                        Android.Net.Uri selectedImage = imageReturnedIntent.Data;
                        bool hsextra = Intent.HasExtra("picpos");
                        int picpos = Intent.GetIntExtra("picpos", 0);/*lastpicpos*/;

                        string selectedpath = Support.GetPathToImage(selectedImage, this);

                        picAndWords[picpos].pathToPic = selectedpath; //i dont know what path this command return

                        if (db.Get<Cluster>(position + 1).pathToIcon == System.IO.Path.Combine(GetExternalFilesDir("DirectoryPictures").AbsolutePath, "defpic.png"))
                        {
                            Cluster cluster = db.Get<Cluster>(position + 1);

                            cluster.pathToIcon = selectedpath;

                            db.Update(cluster);
                        }
                        
                        db.Update(picAndWords[picpos]);//Rewrite it to update only picpos element
                        db.Close();

                        adapter.NotifyDataSetChanged();//Is more effective version of recycler updateing: ..NotifyItemChanged(int position);
                    }

                    break;
            }

            base.OnActivityResult(requestCode, resultCode, imageReturnedIntent);
        }
    }
}