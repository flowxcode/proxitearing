# This code and associated information is provided to guide KEOLABS'
# customers in their use of KEOLABS' testing tools. KEOLABS shall not be
# liable for any direct, indirect or consequential damages with respect
# to claims arising from the content and/or its use by the KEOLABS' customers.
# For more information, refer to KEOLABS Sales Conditions at www.keolabs.com.

"""
@file   StartStop.py
@brief  Python example, that uses the ProxiLAB.
        This is an example to see how to configure, launch and stop the trace
        feature.
"""

import sys
import site
import os
site.addsitedir( os.environ['RGPA_PATH'] + '..\\Quest\\Lib' )

import win32com.client
import pythoncom
import sys
import ctypes

OUTPUT_FILE_PATH = "C:\\tmp"
NAME = "StartStop"

#Main function
def StartStop(proxilab):

    #Configure the trace
    ProxiLAB.Spy.OutputFile = OUTPUT_FILE_PATH + "\\" + NAME + ".trc"
    
    #Configure the analyzer
    ProxiLAB.Spy.Analyzer.ISO14443Enable = 1 
    ProxiLAB.Spy.Analyzer.ISO15693Enable = 0 
    ProxiLAB.Spy.Analyzer.ISO18092Enable = 0 
    ProxiLAB.Spy.Analyzer.JISX6319Enable = 0
    ProxiLAB.Spy.Analyzer.DisplaySMA1 = 1
    ProxiLAB.Spy.Analyzer.InputFile = ProxiLAB.Spy.OutputFile
    ProxiLAB.Spy.Analyzer.OutputFile = OUTPUT_FILE_PATH + "\\" + NAME + ".xgpa"
    
    #Start the trace
    ProxiLABUtilities.StartSpy(proxilab)
    
    #Toggle SMA1
    ProxiLAB.Settings.IO1Direction = ProxiLABUtilities.Constants.DIRECTION_OUTPUT
    j = 0
    while(j<20):
        ProxiLAB.Settings.Output1 = ProxiLABUtilities.Constants.OUTPUT_CONSTANT_HIGH
        ProxiLAB.Settings.Output1 = ProxiLABUtilities.Constants.OUTPUT_CONSTANT_LOW
        j += 1
    
    #Stop the trace
    ProxiLABUtilities.StopSpy(proxilab)
    

# Create ProxiLAB COM object
ProxiLAB = win32com.client.Dispatch("KEOLABS.ProxiLAB")

# Test if ProxiLAB is connected
if (ProxiLAB.IsConnected==0):
    Mbox('StartStop', 'ProxiLAB not found', 0)
else:  
    # Import constants values
    sys.path.append(ProxiLAB.GetToolDirectory() + '\inc')
    import ProxiLABUtilities
    
    # Reset ProxiLAB's configuration
    ProxiLAB.Settings.LoadDefaultConfig()

    #Clear RGPA Output view
    ProxiLAB.Display.ClearOutput()
    
    #Call main function
    StartStop(ProxiLAB)
    


