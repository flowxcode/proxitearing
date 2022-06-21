using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

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
            ProgramSPulse.StartSPulse();
            //SPulsePyClone.Run();
        }

        public void threads()
        {
            IProxiLAB keo = (IProxiLAB)Activator.CreateInstance(Type.GetTypeFromProgID("KEOLABS.ProxiLAB"));

            //KeoService.InitProtocol(keo);

            byte[] txBuffer = { 0x00, 0xA4, 0x04, 0x00, 0x08, 0xA0, 0x00, 0x00, 0x01, 0x51, 0x00, 0x00, 0x00, 0x00 };
            byte[] rxBuffer = new byte[266];
            uint err;

            //var x = KeoService.SendTcl(keo, txBuffer, 5000);
            //var x = KeoService.RequestTearing(keo, 5000);

            KeoService.InitProtocol(keo);

            DateTime _starttime = DateTime.UtcNow;
            Stopwatch _stopwatch = Stopwatch.StartNew();
            DateTime highresDT = _starttime.AddTicks(_stopwatch.Elapsed.Ticks);

            KeoSend ks = new KeoSend();
            KeoTear kt = new KeoTear();

            Thread t1 = new Thread(new ThreadStart(ks.SendTcl));
            t1.Start();
            Thread t2 = new Thread(new ThreadStart(kt.Tear));
            t2.Start();

            DateTime highresDT2 = _starttime.AddTicks(_stopwatch.Elapsed.Ticks);



            Console.WriteLine(highresDT);
            Console.WriteLine(highresDT2);

            //Console.ReadKey();

        }
    }
}
