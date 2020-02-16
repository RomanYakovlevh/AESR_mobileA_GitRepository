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
using Android.Views;
using Android.Widget;
using Java.Nio;

namespace AESR_mobileA
{
    class Support
    {
        public static byte[] BitmapToByte(Bitmap bitmap)
        {
            MemoryStream stream = new MemoryStream();
            bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
            byte[] bitmapData = stream.ToArray();
            stream.Close();
            return bitmapData;
        }

        public static string GetPathToImage(Android.Net.Uri uri, Context context)
        {
            string doc_id = "";
            using (var c1 = context.ContentResolver.Query(uri, null, null, null, null))
            {
                c1.MoveToFirst();
                string document_id = c1.GetString(0);
                doc_id = document_id.Substring(document_id.LastIndexOf(":") + 1);
            }

            string path = null;

            // The projection contains the columns we want to return in our query.
            string selection = Android.Provider.MediaStore.Images.Media.InterfaceConsts.Id + " =? ";
            using (var cursor = context.ContentResolver.Query(Android.Provider.MediaStore.Images.Media.ExternalContentUri, null, selection, new string[] { doc_id }, null))
            {
                if (cursor == null) return path;
                var columnIndex = cursor.GetColumnIndexOrThrow(Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data);
                cursor.MoveToFirst();
                path = cursor.GetString(columnIndex);
            }
            return path;

            ///
            /// Most important, when you use this method, maybe you will come across this problem :
            /// Java.Lang.SecurityException: Permission Denial: reading com.android.providers.media.MediaProvider uri content://media/external/images/media from pid=21975, uid=10417 requires android.permission.READ_EXTERNAL_STORAGE, or grantUriPermission()
            /// When API >= 23, Requesting Permissions at Run Time, users grant permissions to apps while the app is running, not when they install the app. You should check if you have android.permission.READ_EXTERNAL_STORAGE permission, if not, you need to request the permissions.
            ///
        }
    }
}