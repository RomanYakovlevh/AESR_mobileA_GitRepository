package crc64166b4de75ce3f61a;


public class ClusterViewHolder
	extends android.support.v7.widget.RecyclerView.ViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("AESR_mobileA.ClusterViewHolder, AESR_mobileA", ClusterViewHolder.class, __md_methods);
	}


	public ClusterViewHolder (android.view.View p0)
	{
		super (p0);
		if (getClass () == ClusterViewHolder.class)
			mono.android.TypeManager.Activate ("AESR_mobileA.ClusterViewHolder, AESR_mobileA", "Android.Views.View, Mono.Android", this, new java.lang.Object[] { p0 });
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
