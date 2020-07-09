using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace AESR_mobileA
{

    /*
     * (Решено)Ничего не работает. Я не доделал этот класс, все изменения кластеров и картинок сохраняются в листы (clusters и picAndWords соответственно). В методе
     * OnStop() должна чистится база данных и создаваться заново из этих листов(ну или как-нибудь по-другому это может происходить, более оптимально).
     * 
     * (Решено)С картинками хрень, там загружка по-прежнему прописана напрямую в базу(но из-за конфликта в коде вылетает ошибка), но менять это бессмысленно пока не будет решена проблема с тем, почему в PutExtra не 
     * загружаются данные
     * 
     * (Решено)Исчо одна ошибка: adapter.NotifyDataSetChanged работает от базы данных, поэтому никакие изменения в списках тут на него не отражаются
     * 
     * Почему-то при переходе в ЭдитКластерАктивити происходит фокус на элемент списка
     * 
     * Странно работает сохранение слов, фокус будто-бы не всегда меняется (Дополнено) При создании нового кластера слетают и имена кластеров
     * 
     * По какой-то причине, иногда при устаноке фотографии вылетает ошибка о несовместимосте действий прокрутки списка и установки. Думаю, это происходит изза 
     * того что список начинает работать раньше чем заканчиваеться работа опасных методов, и случайное взаимодействие с ним приводит к вылету
     * (Дополнено) Конкретно, ошибка вылетает на ..NotifyDataSetChanged(), возможно, перед использованием этого метода стоит как-то блокировать RecycleView
     *  
     * (Решено)При переходе в прошлую активность вылетает ошибка об отсутствии элементов в таблице. Я думаю, что таблица запоминает номера удаленных элементов и присваивает
     * новым номера не начиная с единицы
     * 
     * Если удалить из кластера картинку, которая на обложке кластера, то обложка не поменяется.
     * (Дополнено) Считаю, что стоит поменять систему усатановки обложки: пускай каждый раз при прогрузке страницы кластеров обложка каждый раз берется как картинка 
     * первого элемента из списка в кластере, а если в кластере ничего нет то устанавливается дефолтная
      */
    [Activity(Label = "@string/start_screen_title", Theme = "@style/AppTheme")]
    class EditClusterActivity : Activity
    {
        RecyclerView recycler;
        RecyclerView.LayoutManager layoutManager;

        PictureViewAdapter adapter;

        public List<Cluster> clusters = new List<Cluster>();
        public List<PicAndWord> picAndWords = new List<PicAndWord>();

        int position;
        int lastpicpos;

        static EventHandler<View.FocusChangeEventArgs> EditViewText;
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

            var db = new SQLiteConnection(System.IO.Path.Combine(GetExternalFilesDir(null).AbsolutePath, "aesrclusters.db3")); // написать такой же реактивный код
            //для SelectClusterActivity

            clusters = db.Table<Cluster>().ToList();
            picAndWords = db.Table<PicAndWord>().ToList();

            db.Close();

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

            title.Text = clusters[position].clusterName;//Text editing is not just by touch. It will get a some code

            title.Click += (s, e) =>
            {
                title.FocusableInTouchMode = true;
                bool focres = title.RequestFocus(); //Java.Lang.NoSuchMethodException??
                //if (focres == true)
                //{
                //    title.KeyPress += Title_TextChanged;
                //}
                title.FocusChange += Title_TextChanged;
            };

            backButton.Click += BackButton_Click;
            addwordButton.Click += AddwordButton_Click;
            playButton.Click += PlayButton_Click;
        }

        //4 июл 2020
        //Пошел присваивать ИД всем кластерам и картинкам, скоро вернусь. Если что, сверху я создал лист со всеми картинками, осталось сделать так чтобы 
        //все изменения шли туда, а сохранение происходило в OnStop()

        private void Title_TextChanged(object sender, View.FocusChangeEventArgs e)
        {
            //var db = new SQLiteConnection(System.IO.Path.Combine(GetExternalFilesDir(null).AbsolutePath, "aesrclusters.db3")); //This code calling twice?! Or not?

            //string lastclustername = clusters[position].clusterName;

            clusters[position].clusterName = title.Text;

            //var picwords = db.Table<PicAndWord>().ToList();

            //foreach (var item in picwords)
            //{
            //    if (item.clusterUniqID == lastclustername)
            //    {
            //        item.clusterUniqID = clusters[position].clusterName;
            //    }
            //}

            //db.Update(clusters[position]);

            //db.UpdateAll(picwords);

            //db.Close();

            EditText objectWhoSentEvent = sender as EditText; //я не знаю какой у тебя базовый класс такчто поменяй Android.Text если нужно
            objectWhoSentEvent.FocusChange -= Title_TextChanged;
            objectWhoSentEvent.FocusableInTouchMode = false;
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            Intent next = new Intent(this, typeof(SelectClusterActivity));

            StartActivity(next);
        }

        private void AddwordButton_Click(object sender, EventArgs e)
        {
            PicAndWord pic = new PicAndWord();
            pic.clusterUniqID = clusters[position].uniqID;
            pic.uniqID = MagicDataA.Generate.Name("picture", ".0");
            pic.pathToPic = System.IO.Path.Combine(GetExternalFilesDir("DirectoryPictures").AbsolutePath, "defpic.png");
            pic.word = "new word";

            //var db = new SQLiteConnection(System.IO.Path.Combine(GetExternalFilesDir(null).AbsolutePath, "aesrclusters.db3"));
            //db.Insert(pic);//Rewrite it to update only picpos element
            //db.Close();

            picAndWords.Add(pic);

            adapter.NotifyDataSetChanged();//Is more effective version of recycler updateing: ..NotifyItemChanged(int position);
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            var db = new SQLiteConnection(System.IO.Path.Combine(GetExternalFilesDir(null).AbsolutePath, "aesrclusters.db3"));

            db.DeleteAll<Cluster>(); //Заменить этот код на более эффективный
            db.DeleteAll<PicAndWord>();

            db.InsertAll(clusters);
            db.InsertAll(picAndWords);

            db.Close();

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

            EditViewText = (object sender, View.FocusChangeEventArgs e) => EditText_Changed(sender, e, pos, editText);

            //editText.TextChanged += EditViewText;//I not exctly know how it works - this event is call by each letter changed, or when user write word fully, and press enter and exit from keybrd.?

            editText.FocusableInTouchMode = true;
            bool focres = editText.RequestFocus(); //Java.Lang.NoSuchMethodException??
            //if (focres == true)
            //{
            //    editText.KeyPress += EditViewText;
            //}

            editText.FocusChange += EditViewText;
        }

        void EditText_Changed(object sender, Android.Views.View.FocusChangeEventArgs e, int pos, EditText editText)
        {
            //var db = new SQLiteConnection(System.IO.Path.Combine(GetExternalFilesDir(null).AbsolutePath, "aesrclusters.db3"));

            //List<PicAndWord> picAndWords = db.Table<PicAndWord>().ToList().FindAll(x => x.clusterUniqID == clusters[position].clusterName);

            try
            {
                int c = 0;

                for (int i = 0; i < picAndWords.Count; i++)
                {
                    if (picAndWords[i].clusterUniqID == clusters[position].uniqID)
                    {
                        if (c == pos)
                        {
                            picAndWords[i].word = editText.Text;
                            c++;
                        }
                        else
                        {
                            c++;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new IndexOutOfRangeException();
            }



            //db.Update(picAndWords[pos]);//Rewrite it to update only picpos element
            //db.Close();

            adapter.NotifyDataSetChanged();//Is more effective version of recycler updateing: ..NotifyItemChanged(int position);

            EditText objectWhoSentEvent = sender as EditText; //я не знаю какой у тебя базовый класс такчто поменяй Android.Text если нужно
            objectWhoSentEvent.FocusChange -= EditViewText;//objectWhoSentEvent is the same as editText? Might be error of it?
            editText.FocusableInTouchMode = false;
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
                //pickPhoto.Dispose();//It is haveing a goal?

                lastpicpos = e;

                pickPhoto.PutExtra("picpos", e);

                StartActivityForResult(pickPhoto, 0);//one can be replaced with any action code
            }
        }

        protected override void OnStop()
        {
            base.OnStop(); // Always call the superclass first

            var db = new SQLiteConnection(System.IO.Path.Combine(GetExternalFilesDir(null).AbsolutePath, "aesrclusters.db3"));

            db.DeleteAll<Cluster>(); //Заменить этот код на более эффективный
            db.DeleteAll<PicAndWord>();

            db.InsertAll(clusters);
            db.InsertAll(picAndWords);

            db.Close();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent imageReturnedIntent)
        {
            switch (requestCode)
            {
                case 0:
                    if (resultCode == Result.Ok)
                    {
                        //List<PicAndWord> picAndWordsD = picAndWords.FindAll(x => x.clusterUniqID == clusters[position].uniqID);

                        //In current version picked image holded only in users gallery. If user delete image, image will be unaccessible in app.
                        Android.Net.Uri selectedImage = imageReturnedIntent.Data;
                        bool hsextra = imageReturnedIntent.HasExtra("picpos");

                        //if (hsextra == false)
                        //{
                        //    throw new ArgumentException();
                        //}

                        int picpos =/* imageReturnedIntent.GetIntExtra("picpos", 0);*/lastpicpos;

                        string selectedpath = Support.GetPathToImage(selectedImage, this);

                        //picAndWords[picpos].pathToPic = selectedpath; //i dont know what path this command return

                        try
                        {
                            int c = 0;

                            for (int i = 0; i < picAndWords.Count; i++)
                            {
                                if (picAndWords[i].clusterUniqID == clusters[position].uniqID)
                                {
                                    if (c == picpos)
                                    {
                                        picAndWords[i].pathToPic = selectedpath;
                                        c++;
                                    }
                                    else
                                    {
                                        c++;
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                            throw new IndexOutOfRangeException();
                        }

                        //if (db.Get<Cluster>(position + 1).pathToIcon == System.IO.Path.Combine(GetExternalFilesDir("DirectoryPictures").AbsolutePath, "defpic.png"))
                        //{
                        //    Cluster cluster = db.Get<Cluster>(position + 1);

                        //    cluster.pathToIcon = selectedpath;

                        //    db.Update(cluster);
                        //}

                        if (clusters[position].pathToIcon == System.IO.Path.Combine(GetExternalFilesDir("DirectoryPictures").AbsolutePath, "defpic.png"))
                        {
                            clusters[position].pathToIcon = selectedpath;
                        }

                        //db.Update(picAndWords[picpos]);//Rewrite it to update only picpos element
                        //db.Close();

                        adapter.NotifyDataSetChanged();//Is more effective version of recycler updateing: ..NotifyItemChanged(int position);
                    }

                    break;
            }

            base.OnActivityResult(requestCode, resultCode, imageReturnedIntent);
        }
    }
}