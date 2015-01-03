using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvserver
{
    //public class mvSzin
    //{
    //    private Szin? _kod;
    //    public Szin? kod
    //    {
    //        get
    //        {
    //            return _kod;
    //        }
    //        set
    //        {
    //            _kod = value;
    //            this.cssClass = getCssClassName(value);
    //        }
    //    }

    //    private string getCssClassName(Szin? kod)
    //    {
    //        switch (kod)
    //        {
    //            case Szin.Kek: return "szin_k";
    //            case Szin.Piros: return "szin_p";
    //            case Szin.Sarga: return "szin_s";
    //            case Szin.Zold: return "szin_z";
    //            default: return String.Empty;
    //        }
    //    }
    //    public string cssClass { get; private set; }

    //    public static Szin? GetSzin(string kod)
    //    {
    //        byte b;
    //        if (Byte.TryParse(kod, out b))
    //        {
    //            return (Szin)b;
    //        }
    //        return null;
    //    }

    //    public static Szin? GetSzin(byte kod)
    //    {
    //        if (kod > 0)
    //            return (Szin)kod;

    //        return null;
    //    }
    //}
}
