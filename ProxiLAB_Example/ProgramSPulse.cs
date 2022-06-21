using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    public static class ProgramSPulse
    {
        public static void StartSPulse()
        {
            var path = Directory.GetCurrentDirectory();
            var f1 = path + "\\spyfile.trc";
            var f2 = path + "\\spyfile.xgpa";
            if (File.Exists(f1))
                File.Delete(f1);
            if (File.Exists(f2))
                File.Delete(f2);

            IProxiLAB keo = (IProxiLAB)Activator.CreateInstance(Type.GetTypeFromProgID("KEOLABS.ProxiLAB"));


            byte[] txBuffer = { 0x00, 0xA4, 0x04, 0x00, 0x08, 0xA0, 0x00, 0x00, 0x01, 0x51, 0x00, 0x00, 0x00, 0x00 };
            byte[] rxBuffer = new byte[266];
            uint err;


            keo.Spy.OutputFile = path + "\\spyfile.trc";

            keo.Spy.Analyzer.ISO14443Enable = 1;
            //keo.Spy.Analyzer.ISO15693Enable = 0;
            //keo.Spy.Analyzer.ISO18092Enable = 0;
            //keo.Spy.Analyzer.JISX6319Enable = 0;
            keo.Spy.Analyzer.DisplaySMA1 = 1;
            keo.Spy.Analyzer.OutputFile = path + "\\spyfile.xgpa";
            keo.Spy.Analyzer.Start();

            keo.Spy.Start();

            KeoService.InitProtocol(keo);
            KeoService.SPulseTcl(keo, txBuffer, 0);

            DateTime _starttime = DateTime.UtcNow;
            Stopwatch _stopwatch = Stopwatch.StartNew();
            DateTime highresDT = _starttime.AddTicks(_stopwatch.Elapsed.Ticks);

            keo.Delay(200);

            keo.Spy.Analyzer.Stop();
            keo.Spy.Stop();

            Console.WriteLine(highresDT);

//            Console.ReadKey();
        }
    }
}
