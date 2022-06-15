using ProxiLABLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxiLAB_Example
{
    public class KeoTear
    {

        public void Tear()
        {
            Console.WriteLine("tera");

            IProxiLAB keo = (IProxiLAB)Activator.CreateInstance(Type.GetTypeFromProgID("KEOLABS.ProxiLAB"));
            keo.Delay(35);
            keo.Reader.PowerOff();

            System.Diagnostics.Debug.WriteLine("####### tear power down");
        }
    }
}
