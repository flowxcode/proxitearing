using ProxiLABLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxiLAB_Example
{
    public class KeoSend
    {

        public void SendTcl()
        {
            Console.WriteLine("send it");

            IProxiLAB keo = (IProxiLAB)Activator.CreateInstance(Type.GetTypeFromProgID("KEOLABS.ProxiLAB"));
            keo.Reader.Power(150);

            byte[] txBuffer = { 0x00, 0xA4, 0x04, 0x00, 0x08, 0xA0, 0x00, 0x00, 0x01, 0x51, 0x00, 0x00, 0x00, 0x00 };
            byte[] rxBuffer = new byte[266];
            uint err;
            KeoService.SendTcl(keo, txBuffer);

            System.Diagnostics.Debug.WriteLine("send tcl thread done");
        }
    }
}
