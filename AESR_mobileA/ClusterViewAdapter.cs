using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

using SQLite;

namespace AESR_mobileA
{
    public class ClusterViewAdapter : RecyclerView.Adapter
    {
        Context context;

        public event EventHandler<int> ItemClick;

        public ClusterViewAdapter(Context _context)
        {
            context = _context;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.layout_frame, parent, false);
            ClusterViewHolder holder = new ClusterViewHolder(view, OnClick);

            return holder;
        }

        /// <summary>
        /// // System.NotSupportedException
        //  Сообщение = Cannot get SQL for: Add
        /// </summary>
        /// <param name="_holder"></param>
        /// <param name="position"></param>

        public override void OnBindViewHolder(RecyclerView.ViewHolder _holder, int position)
        {
            ClusterViewHolder holder = _holder as ClusterViewHolder;

            var cls = new SQLiteConnection(System.IO.Path.Combine(context.GetExternalFilesDir(null).AbsolutePath, "aesrclusters.db3"));

            byte[] byteimg = File.ReadAllBytes(cls.Get<Cluster>(position + 1).pathToIcon);

            holder.Image.SetImageBitmap(BitmapFactory.DecodeByteArray(byteimg, 0, byteimg.Length));

            holder.Caption.Text = cls.Get<Cluster>(position + 1).clusterName;

            cls.Close();
        }

        public override int ItemCount
        {
            get 
            {

                var db = new SQLiteConnection(System.IO.Path.Combine(context.GetExternalFilesDir(null).AbsolutePath, "aesrclusters.db3"));

                int cnt = db.Table<Cluster>().Count();

                db.Close();

                return cnt; 
            
            }
        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }
}