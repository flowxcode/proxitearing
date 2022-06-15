using ProxiLABLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxiLAB_Example
{
    public static class KeoService
    {
        public static uint RfPower(IProxiLAB keo, uint level)
        {
            return keo.Reader.Power(level);
        }

        public static byte[] InitProtocol(IProxiLAB keo)
        {
            byte[] answer = null;
            uint error = 0;

            uint pcdBitRate = 106;
            uint piccBitRate = 106;
            byte[] ISO14443_compliant = new byte[1];
            byte[] CID = new byte[1];
            byte[] UID = new byte[4];
            byte[] ATS = new byte[128];
            uint UIDLength = 0;
            uint ATSLength = 0;
            answer = new byte[128];
            uint answerLength = 0;
            byte[] SAK = new byte[1];

            keo.Reader.Power(0);
            keo.Reader.Power(200);

            //Command REQUEST A
            error = keo.Reader.ISO14443.TypeA.Request(out answer[0], (uint)answer.Length, out answerLength);
            if (error != (uint)ProxiLABErrorCode.ERR_SUCCESSFUL)
            {
                //Display the error
                System.Diagnostics.Debug.WriteLine("ATQA: " + keo.GetErrorInfo(error));
            }
            else
            {
                //Display the answer
                String str = "ATQA: ";
                for (uint i = 0; i < answerLength; i++)
                    str += "0x" + answer[i].ToString("X2") + " ";
                System.Diagnostics.Debug.WriteLine(str);
            }

            for (uint cascadeLevel = 1; cascadeLevel <= 3; cascadeLevel++)
            {
                keo.Delay(100);

                //Command ANTICOLLISION A
                keo.Reader.ISO14443.TypeA.Anticollision(cascadeLevel, out UID[0], (uint)UID.Length, out UIDLength);

                keo.Delay(100);

                //Command SELECT A
                String str = "UID: ";
                for (uint i = 0; i < UIDLength; i++)
                    str += "0x" + UID[i].ToString("X2") + " ";
                System.Diagnostics.Debug.WriteLine(str);
                error = keo.Reader.ISO14443.TypeA.Select(cascadeLevel, ref UID[0], (uint)UID.Length, out SAK[0]);
                if (error != (uint)ProxiLABErrorCode.ERR_SUCCESSFUL)
                {
                    //Display the error
                    System.Diagnostics.Debug.WriteLine("SAK: " + keo.GetErrorInfo(error));
                }
                else
                {
                    //Display the answer
                    str = "SAK: ";
                    for (uint i = 0; i < SAK.Length; i++)
                        str += "0x" + SAK[i].ToString("X2") + " ";
                    System.Diagnostics.Debug.WriteLine(str);
                }

                //Is UID complete ?
                bool UIDComplete;
                if ((SAK[0] & 0x04) != 0)
                    UIDComplete = false;
                else
                    UIDComplete = true;
                if (UIDComplete)
                    break;
            }

            keo.Delay(100);

            //Command RATS
            System.Diagnostics.Debug.WriteLine("RATS command...");
            error = keo.Reader.ISO14443.TypeA.Rats(CID[0], out ATS[0], (uint)ATS.Length, out ATSLength);
            if (error != (uint)ProxiLABErrorCode.ERR_SUCCESSFUL)
            {
                //Display the error
                System.Diagnostics.Debug.WriteLine("ATS: " + keo.GetErrorInfo(error));
            }
            else
            {
                //Display the answer
                String str = "ATS: ";
                for (uint i = 0; i < ATSLength; i++)
                    str += "0x" + ATS[i].ToString("X2") + " ";
                System.Diagnostics.Debug.WriteLine(str);
            }

            return UID;
        }

        public static byte[] SendTcl(IProxiLAB keo, byte[] apdu)
        {
            uint error = 0;

            byte[] UID = new byte[4];

            byte[] rxBuffer = new byte[266];
            uint rxBufferLength;
            System.Diagnostics.Debug.WriteLine("Tcl command...");
            error = keo.Reader.ISO14443.SendTclCommand(0x00, 0x00, ref apdu[0], (uint)apdu.Length, out rxBuffer[0], (uint)rxBuffer.Length, out rxBufferLength);
            if (error != (uint)ProxiLABErrorCode.ERR_SUCCESSFUL)
            {
                //Display the error
                var errorString = keo.GetErrorInfo(error);
                System.Diagnostics.Debug.WriteLine("Tcl: " + keo.GetErrorInfo(error));
                
                throw new Exception(errorString);
            }
            else
            {
                //Display the answer
                String str = "Tcl response: ";
                for (uint i = 0; i < rxBufferLength; i++)
                    str += "0x" + rxBuffer[i].ToString("X2") + " ";
                System.Diagnostics.Debug.WriteLine(str);

                byte[] returnBuffer = new byte[rxBufferLength];
                for (uint i = 0; i < rxBufferLength; i++)
                    returnBuffer[i] = rxBuffer[i];

                return returnBuffer;
            }

            return UID;
        }

        public static byte[] SendTcl(IProxiLAB keo, byte[] apdu, uint cycles)
        {
            byte[] answer = null;
            uint answerLength = 0;



            uint error = 0;

            byte[] UID = new byte[4];

            byte[] rxBuffer = new byte[266];
            uint rxBufferLength;

            keo.Settings.Mode = (byte)eSettingsMode.MODE_READER_AB;
            error = keo.Sequencer.StartChaining();

            System.Diagnostics.Debug.WriteLine("Tcl command...");
            error = keo.Reader.ISO14443.TypeA.Request(out answer[0], (uint)answer.Length, out answerLength);
            //error = keo.Reader.ISO14443.SendTclCommand(0x00, 0x00, ref apdu[0], (uint)apdu.Length, out rxBuffer[0], (uint)rxBuffer.Length, out rxBufferLength);
            keo.Sequencer.DelayCycle((byte)eClockDomain.CLOCK_DOMAIN_1356, cycles);
            keo.Reader.RfReset();
            if (error != (uint)ProxiLABErrorCode.ERR_SUCCESSFUL)
            {
                //Display the error
                var errorString = keo.GetErrorInfo(error);
                System.Diagnostics.Debug.WriteLine("Tcl: " + keo.GetErrorInfo(error));

                throw new Exception(errorString);
            }
            else
            {
                /*
                //Display the answer
                String str = "Tcl response: ";
                for (uint i = 0; i < rxBufferLength; i++)
                    str += "0x" + rxBuffer[i].ToString("X2") + " ";
                System.Diagnostics.Debug.WriteLine(str);

                byte[] returnBuffer = new byte[rxBufferLength];
                for (uint i = 0; i < rxBufferLength; i++)
                    returnBuffer[i] = rxBuffer[i];

                return returnBuffer;
                */
            }

            return UID;
        }

        public static byte[] RequestTearing(IProxiLAB keo, uint cycles)
        {
            byte[] answer = new byte[128];
            uint answerLength = 0;

            uint error = 0;

            keo.Settings.Mode = (byte)eSettingsMode.MODE_READER_AB;
            keo.Sequencer.StartChaining();

            keo.Sequencer.DelayCycle((byte)eClockDomain.CLOCK_DOMAIN_1356, cycles);

            keo.Reader.ISO14443.TypeA.Request(out answer[0], (uint)answer.Length, out answerLength);

            keo.Sequencer.DelayCycle((byte)eClockDomain.CLOCK_DOMAIN_1356, 3000);

            keo.Reader.RfReset();

            keo.Sequencer.Run();

            return answer;
        }


    }
}
