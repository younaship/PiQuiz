using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MyAdCenter
{
    static MyAdCenter myAdCenter;
    BannerView bannerView;
    InterstitialAd id;

    public static void Start()
    {
        if (myAdCenter != null)
            return;
        myAdCenter = new MyAdCenter();
        myAdCenter.AdStart();
        myAdCenter.IAdLoad();
    }

    public static void IAdShow()
    {
        if (myAdCenter == null)
            return;

        if (!myAdCenter.id.IsLoaded()) return;
        myAdCenter.id.Show();
        myAdCenter.IAdLoad();
    }

    private void AdStart()
    {
#if UNITY_ANDROID
        string appId = "AdID"; // "ca-app-pub-5352945085001350~5895921481";

#elif UNITY_IPHONE
        string appId =  "" // "ca-app-pub-5352945085001350~5895921481";

#else
        string appId = "unexpected_platform";
#endif

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);
    }

    private void IAdLoad()
    {
        string adId = "AdId"; // "ca-app-pub-5352945085001350/1638209754";
        id = new InterstitialAd(adId);
        id.LoadAd(new AdRequest.Builder().Build());
    }

}
