using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//To use ProxiLABLib namespace, you must add a reference to ProxiLABLib in your project:
//In 'Solution Explorer', right click on 'References', then 'Add Reference...'
//In the 'Add Reference' window, select 'ProxiLAB 1.0 Type Library' in the 'COM' tab and click 'OK'
//After that, you should be able to use ProxiLABLib namespace and access ProxiLAB COM object
using ProxiLABLib;

namespace ProxiLAB_Example
{
    class Program
    {
        static void Main(string[] args)
        {
            IProxiLAB keo = (IProxiLAB)Activator.CreateInstance(Type.GetTypeFromProgID("KEOLABS.ProxiLAB"));

            KeoService.InitProtocol(keo);

            byte[] txBuffer = { 0x00, 0xA4, 0x04, 0x00, 0x08, 0xA0, 0x00, 0x00, 0x01, 0x51, 0x00, 0x00, 0x00, 0x00 };
            byte[] rxBuffer = new byte[266];
            uint err;
            var x = KeoService.SendTcl(keo, txBuffer, 5000);
        }
    }
}
