package md51889867b189273cf69fb92acf6749639;


public class Splash3Activity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("GeoTiles.Droid.Splash3Activity, GeoTiles.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", Splash3Activity.class, __md_methods);
	}


	public Splash3Activity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == Splash3Activity.class)
			mono.android.TypeManager.Activate ("GeoTiles.Droid.Splash3Activity, GeoTiles.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

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
