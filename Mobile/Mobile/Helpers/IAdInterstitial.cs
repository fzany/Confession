using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mobile.Helpers
{
    public interface IAdmobInterstitialAds
    {
        Task Display(string adId);
    }
}
