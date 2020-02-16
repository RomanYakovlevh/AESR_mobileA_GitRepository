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

namespace AESR_mobileA
{
    class PictureViewHolder : RecyclerView.ViewHolder
    {
        public ImageView Image { get; private set; }
        public ImageView ImageDelete { get; private set; }
        public TextView Caption { get; private set; }

        public PictureViewHolder(View itemView, Action<int> listenerTextClick, Action<int> listenerImageClick, Action<int> listenerDeleteClick) : base(itemView)
        {
            // Locate and cache view references:
            Image = itemView.FindViewById<ImageView>(Resource.Id.picframeImage);
            Caption = itemView.FindViewById<EditText>(Resource.Id.picframeText);
            ImageDelete = itemView.FindViewById<ImageView>(Resource.Id.picDelete);

            Image.Click += (sender, e) => listenerImageClick(base.LayoutPosition);
            Caption.Click += (sender, e) => listenerTextClick(base.LayoutPosition);
            ImageDelete.Click += (sender, e) => listenerDeleteClick(base.LayoutPosition);
        }
    }
}