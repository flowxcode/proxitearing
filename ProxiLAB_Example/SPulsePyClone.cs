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
                //File.Delete(f1);
            if (File.Exists(f2))
                File.Delete(f2);

            for (int j = 0; j < 20; j++)
            {
                IProxiLAB keo = (IProxiLAB)Activator.CreateInstance(Type.GetTypeFromProgID("KEOLABS.ProxiLAB"));

                keo.Settings.LoadDefaultConfig();
                keo.Spy.ShowTraceFile = 1;

                keo.Spy.OutputFile = path + "\\spyfile.trc";
                keo.Spy.Analyzer.ISO14443Enable = 1;
                //keo.Spy.Analyzer.ISO15693Enable = 0;
                //keo.Spy.Analyzer.ISO18092Enable = 0;
                //keo.Spy.Analyzer.JISX6319Enable = 0;
                keo.Spy.Analyzer.DisplaySMA1 = 1;
                keo.Spy.Analyzer.OutputFile = path + "\\spyfile.xgpa";

                keo.Spy.Analyzer.Start();
                keo.Spy.Start();

                keo.Delay(100);

                uint error = 0;
                //uint pcdBitRate = 106;
                //uint piccBitRate = 106;
                byte[] ISO14443_compliant = new byte[1];
                byte[] CID = new byte[1];
                byte[] UID = new byte[7];
                uint UIDLength = 0;
                byte[] ATS = new byte[128];
                uint ATSLength = 0;
                byte[] SAK = new byte[1];

                byte[] txBuffer = { 0x00, 0xA4, 0x04, 0x00, 0x08, 0xA0, 0x00, 0x00, 0x01, 0x51, 0x00, 0x00, 0x00, 0x00 };
                byte[] rxBuffer = new byte[266];
                uint rxBufferLength;

                keo.Spy.Start();
                keo.Reader.PowerLevel_1024 = 700;
                keo.Reader.PowerOn();
                keo.Reader.RfReset();

                //set ProxiLAB in Reader mode
                //keo.Settings.Mode = (byte)eSettingsMode.MODE_READER_15693;

                error = keo.Reader.ISO14443.TypeA.GetCard(106, 106, out ISO14443_compliant[0], out CID[0], out UID[0], (uint)UID.Length, out UIDLength, out ATS[0], (uint)ATS.Length, out ATSLength);

                if (error != (uint)ProxiLABErrorCode.ERR_SUCCESSFUL)
                {
                    System.Diagnostics.Debug.WriteLine("GetCard: " + keo.GetErrorInfo(error));
                }
                else
                {
                    String str = "UID: ";
                    for (uint i = 0; i < UIDLength; i++)
                        str += "0x" + UID[i].ToString("X2") + " ";
                    System.Diagnostics.Debug.WriteLine(str);
                }

                var dir = Directory.GetCurrentDirectory();
                var filepath = dir + "\\Tearing.kwav";

                Random rnd = new Random();
                uint fdt = (uint)rnd.Next(1, 6)*10000;

                error = keo.Spulse.LoadSpulseCsvFile(filepath, fdt, (uint)eFrameTypeFormat.FRAME_TYPE_SPULSE, (uint)eEmulatorLoadSpulseMode.STAND_ALONE);
                error = keo.Spulse.EnableSpulse((uint)eEmulatorSpulseEvent.SP_PCD_EOF, (uint)eEmulatorSpulseOutput.SP_RF_POWER);
                error = keo.Reader.ISO14443.SendTclCommand(0x00, 0x00, ref txBuffer[0], (uint)txBuffer.Length, out rxBuffer[0], (uint)rxBuffer.Length, out rxBufferLength);

                keo.Delay(100);
                keo.Reader.PowerOff();

                if (error != (uint)ProxiLABErrorCode.ERR_SUCCESSFUL)
                {
                    var errorString = keo.GetErrorInfo(error);
                    System.Diagnostics.Debug.WriteLine("Tcl: " + keo.GetErrorInfo(error));
                }
                else
                {
                    String str = "Tcl response: ";
                    for (uint i = 0; i < rxBufferLength; i++)
                        str += "0x" + rxBuffer[i].ToString("X2") + " ";
                    System.Diagnostics.Debug.WriteLine(str);

                    byte[] returnBuffer = new byte[rxBufferLength];
                    for (uint i = 0; i < rxBufferLength; i++)
                        returnBuffer[i] = rxBuffer[i];
                }

                keo.Spy.Analyzer.Stop();
                keo.Spy.Stop();
            }

            Console.WriteLine("press key to end");
            System.Diagnostics.Debug.WriteLine("##### end");
            //Console.ReadKey();
        }
    }
}
