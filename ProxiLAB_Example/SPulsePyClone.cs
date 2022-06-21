using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using ProxiLABLib;

namespace ProxiLAB_Example
{
    public static class SPulsePyClone
    {
        public static void Run()
        {
            var path = Directory.GetCurrentDirectory();
            var f1 = path + "\\spyfile.trc";
            var f2 = path + "\\spyfile.xgpa";
            if (File.Exists(f1))
                File.Delete(f1);
            if (File.Exists(f2))
                File.Delete(f2);

            IProxiLAB keo = (IProxiLAB)Activator.CreateInstance(Type.GetTypeFromProgID("KEOLABS.ProxiLAB"));

            keo.Settings.LoadDefaultConfig();
            keo.Spy.ShowTraceFile = 1;

            keo.Spy.OutputFile = path + "\\spyfile.trc";
            keo.Spy.Analyzer.ISO14443Enable = 1;
            keo.Spy.Analyzer.ISO15693Enable = 0;
            keo.Spy.Analyzer.ISO18092Enable = 0;
            keo.Spy.Analyzer.JISX6319Enable = 0;
            keo.Spy.Analyzer.DisplaySMA1 = 1;
            keo.Spy.Analyzer.OutputFile = path + "\\spyfile.xgpa";
            keo.Spy.Analyzer.Start();
            keo.Spy.Start();


            uint error = 0;
            uint pcdBitRate = 106;
            uint piccBitRate = 106;
            byte[] ISO14443_compliant = new byte[1];
            byte[] CID = new byte[1];
            byte[] UID = new byte[7];
            uint UIDLength = 0;
            byte[] ATS = new byte[128];
            uint ATSLength = 0;
            byte[] answer = new byte[128];
            uint answerLength = 0;
            byte[] SAK = new byte[1];

            //set ProxiLAB in Reader mode
            keo.Settings.Mode = (byte)eSettingsMode.MODE_READER_15693;

            //Start the trace

            //Switch off RF field
            keo.Reader.Power(0);

            //Command RF ON
            keo.Reader.Power(150);

            keo.Delay(500);

            
            //Command GET CARD A
            error = keo.Reader.ISO14443.TypeA.GetCard(pcdBitRate, piccBitRate, out ISO14443_compliant[0], out CID[0], out UID[0], (uint)UID.Length, out UIDLength, out ATS[0], (uint)ATS.Length, out ATSLength);
            if (error != (uint)ProxiLABErrorCode.ERR_SUCCESSFUL)
            {
                //Display the error
                System.Diagnostics.Debug.WriteLine("GetCard: " + keo.GetErrorInfo(error));
            }
            else
            {
                //Display the answer
                String str = "UID: ";
                for (uint i = 0; i < UIDLength; i++)
                    str += "0x" + UID[i].ToString("X2") + " ";
                System.Diagnostics.Debug.WriteLine(str);
            }

            keo.Delay(500);
            


            keo.Spy.Analyzer.Stop();
            keo.Spy.Stop();

            Console.WriteLine("press key to end");
            System.Diagnostics.Debug.WriteLine("##### end");
            //Console.ReadKey();
        }
    }
}
