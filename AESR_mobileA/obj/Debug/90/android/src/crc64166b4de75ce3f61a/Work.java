package crc64166b4de75ce3f61a;


public class Work
	extends mono.android.app.IntentService
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onHandleIntent:(Landroid/content/Intent;)V:GetOnHandleIntent_Landroid_content_Intent_Handler\n" +
			"";
		mono.android.Runtime.register ("AESR_mobileA.Work, AESR_mobileA", Work.class, __md_methods);
	}


	public Work (java.lang.String p0)
	{
		super (p0);
		if (getClass () == Work.class)
			mono.android.TypeManager.Activate ("AESR_mobileA.Work, AESR_mobileA", "System.String, mscorlib", this, new java.lang.Object[] { p0 });
	}


	public Work ()
	{
		super ();
		if (getClass () == Work.class)
			mono.android.TypeManager.Activate ("AESR_mobileA.Work, AESR_mobileA", "", this, new java.lang.Object[] {  });
	}


	public void onHandleIntent (android.content.Intent p0)
	{
		n_onHandleIntent (p0);
	}

	private native void n_onHandleIntent (android.content.Intent p0);

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
