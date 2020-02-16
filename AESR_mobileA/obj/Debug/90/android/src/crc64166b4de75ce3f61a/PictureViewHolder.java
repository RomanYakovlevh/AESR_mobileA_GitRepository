package crc64166b4de75ce3f61a;


public class PictureViewHolder
	extends android.support.v7.widget.RecyclerView.ViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("AESR_mobileA.PictureViewHolder, AESR_mobileA", PictureViewHolder.class, __md_methods);
	}


	public PictureViewHolder (android.view.View p0)
	{
		super (p0);
		if (getClass () == PictureViewHolder.class)
			mono.android.TypeManager.Activate ("AESR_mobileA.PictureViewHolder, AESR_mobileA", "Android.Views.View, Mono.Android", this, new java.lang.Object[] { p0 });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
