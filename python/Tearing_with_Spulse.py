# This code and associated information is provided to guide KEOLABS'
# customers in their use of KEOLABS' testing tools. KEOLABS shall not be
# liable for any direct, indirect or consequential damages with respect
# to claims arising from the content and/or its use by the KEOLABS' customers.
# For more information, refer to KEOLABS Sales Conditions at www.keolabs.com.

"""
This script will execute a tearing test on a type A card after activation.
Put a type A card on the antenna connected to RF out port
Adjust the wanted delay betwenn PCD command and rf_off by adjusting "tearing_fdt" variable (unit: 1/fc) 
"""

import sys
import site
import os
site.addsitedir( os.environ['RGPA_PATH'] + '..\\Quest\\Lib' )

import win32com.client
import pythoncom
import sys

# Create ProxiLAB COM object
ProxiLAB = win32com.client.Dispatch("KEOLABS.ProxiLAB")

# Import constants values
sys.path.append(ProxiLAB.GetToolDirectory() + '\inc')
import ProxiLABUtilities

# Test if ProxiLAB is connected
if (ProxiLAB.IsConnected==0):
    sys.exit("ProxiLAB not found")

###### Reset ProxiLAB's configuration
ProxiLAB.Settings.LoadDefaultConfig()
ProxiLAB.Spy.ShowTraceFile = True

###### POWER and start trace
ProxiLAB.Spy.Start()
ProxiLAB.Reader.PowerLevel_1024 = 700 
ProxiLAB.Reader.PowerOn() 
ProxiLAB.Reader.RfReset() 

###### Command GET CARD A
ISO14443_4 = ProxiLABUtilities.CreateVARIANT() 
CID = ProxiLABUtilities.CreateVARIANT() 
UID = ProxiLABUtilities.CreateVARIANT() 
ATS = ProxiLABUtilities.CreateVARIANT() 
error = ProxiLAB.Reader.ISO14443.TypeA.GetCard(106, 106, ISO14443_4, CID, UID, ATS) 

###### CONFIGURE SPULSE ######
#delay time between event and Spulse
tearing_fdt = 30000
#load spulse file (full of 0)
spulseFile = ProxiLAB.GetToolDirectory() + "Examples\\Python\\Spulse\\Tearing.kwav"
error = ProxiLAB.Spulse.LoadSpulseCsvFile(spulseFile, tearing_fdt, ProxiLABUtilities.Constants.FRAME_TYPE_SPULSE, ProxiLABUtilities.Constants.STAND_ALONE)
#enable spulse to trig on PCD_EOF and send it to RF_POWER
error = ProxiLAB.Spulse.EnableSpulse(ProxiLABUtilities.Constants.SP_PCD_EOF, ProxiLABUtilities.Constants.SP_RF_POWER)


###### Send Command T=CL
RxBuffer = ProxiLABUtilities.CreateVARIANT() 
error = ProxiLAB.Reader.ISO14443.SendTclCommand(0x00, 0x00, [0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA], RxBuffer) 


###### stop trace
ProxiLAB.Delay(500)
ProxiLAB.Reader.PowerOff()
ProxiLAB.Spy.Stop()
ProxiLAB.Spy.Analyzer.Start()
