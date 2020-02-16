using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace AESR_mobileA
{
    public class ClusterViewHolder : RecyclerView.ViewHolder
    {
        public ImageView Image { get; private set; }
        public TextView Caption { get; private set; }

        public ClusterViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            // Locate and cache view references:
            Image = itemView.FindViewById<ImageView>(Resource.Id.frameImage);
            Caption = itemView.FindViewById<TextView>(Resource.Id.frameText);

            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}