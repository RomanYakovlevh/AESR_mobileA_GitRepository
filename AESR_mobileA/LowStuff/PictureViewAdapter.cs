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
using Android.Graphics;

namespace AESR_mobileA
{
    class PictureViewAdapter : RecyclerView.Adapter
    {
        EditClusterActivity context;

        static int clusterposition;

        public event EventHandler<int> imageClick;
        public event EventHandler<int> textClick;
        public event EventHandler<int> deleteClick;

        public PictureViewAdapter(EditClusterActivity _context, int _clusterposition)
        {
            context = _context;

            clusterposition = _clusterposition;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.picture_frame, parent, false);
            PictureViewHolder holder = new PictureViewHolder(view, OnTextClick, OnImageClick, OnDeleteClick);

            return holder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder _holder, int picposition)
        {
            var clusters = context.clusters;
            var picAndWords = context.picAndWords;

            PictureViewHolder holder = _holder as PictureViewHolder;

            var ourclusterUniqID = clusters[clusterposition].uniqID;

            var nesitemlist = picAndWords.FindAll(s => s.clusterUniqID == ourclusterUniqID); //from s in allitemlist where s.clusterName == ourclusternmae select s;//Unknown incomprehensible shit//When new cluster will be created, in this line will be error

            holder.Caption.Text = nesitemlist[picposition].word;

            byte[] byteimg = System.IO.File.ReadAllBytes(nesitemlist[picposition].pathToPic);

            holder.Image.SetImageBitmap(BitmapFactory.DecodeByteArray(byteimg, 0, byteimg.Length));
        }

        public override int ItemCount
        {
            get 
            {
                var ourclusterUniqID = context.clusters[clusterposition].uniqID;

                var nesitemlist = context.picAndWords.FindAll(s => s.clusterUniqID == ourclusterUniqID);

                return nesitemlist.Count; 
            }
        }

        void OnImageClick(int position)
        {
            if (imageClick != null)
                imageClick(this, position);
        }

        void OnTextClick(int position)
        {
            if (textClick != null)
                textClick(this, position);
        }

        void OnDeleteClick(int position)
        {
            if (deleteClick != null)
                deleteClick(this, position);
        }
    }
}