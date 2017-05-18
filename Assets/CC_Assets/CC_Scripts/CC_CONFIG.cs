using UnityEngine;
using System.IO;
using System.Xml;
using System.Runtime.InteropServices;
using System;

/* 
This class loads the configuration XML file for Innovator or Destiny and sets the correct settings for that platform.
If no XML files exists the program launches in a scalable window in Simulator mode.

CyberCANOE Virtual Reality API for Unity3D
(C) 2016 Ryan Theriot, Jason Leigh, Laboratory for Advanced Visualization & Applications, University of Hawaii at Manoa.
Version: 1.13, May 17th, 2017.
 */

/// <summary> Loads the XML config file for Innovator and Destiny. </summary>
public static class CC_CONFIG
{
    [DllImport("user32.dll")]
    static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);
    [DllImport("user32.dll")]
    static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    public static string platform = "";
    private static bool destiny;
    private static bool innovator;

    public static string trackerIP = "128.171.47.10:3883";
    public static string controllerIP = "128.171.47.10:3884";

    public static float interaxial;
    public static bool stereo;
    public static bool panOptic;

    public static bool tracking;

    public static int screenWidth;
    public static int screenHeight;
    public static bool fullScreen;

    private static bool loaded;

    /// <summary>
    /// Checks for the XML config file existence and loads the file if it exists.
    /// </summary>
    /// <returns>Returns true if the config file was succesfully loaded</returns>
    public static bool loadXMLConfig()
    {
    	FileInfo fileCheck = new FileInfo("C:\\CCUnityConfig\\CCUnityConfig.xml");

        if (fileCheck.Exists)
        {
            loadConfig("C:\\CCUnityConfig\\CCUnityConfig.xml");
            loaded = true;
            return true;
        }
        else
        {
            loaded = false;
            return false;
        }

    }
   
    /// <summary>
    /// Check to see if a config file was loaded.
    /// </summary>
    /// <returns>Returns true if a config file was loaded</returns>
    public static bool configLoaded()
    {
        return loaded;
    }

    /// <summary>
    /// Check to see if the current platform is Destiny according to the XML config file.
    /// </summary>
    /// <returns>Returns true if the current platform is Destiny.</returns>
    public static bool isDestiny()
    {
        return destiny;
    }

    /// <summary>
    /// Check to see if the current platform is Innovator according to the XML config file.
    /// </summary>
    /// <returns>Returns true if the current platform is Innovator.</returns>
    public static bool isInnovator()
    {
        return innovator;
    }

    /// <summary>
    /// Check to see if tracking is enabled according to the XML config file.
    /// </summary>
    /// <returns>Returns true if tracking is enabled.</returns>
    public static bool isTracking()
    {
        return tracking;
    }

  
    //Sets the window of innovator as a borderless window and sets to the resolution of Innovator
    private static void setWindowInnovator()
    {
        uint SWP_SHOWWINDOW = 0x0040;
        int GWL_STYLE = -16;
        int WS_BORDER = 1;

        SetWindowLong(GetForegroundWindow(), GWL_STYLE, WS_BORDER);
        SetWindowPos(GetForegroundWindow(), 0, 0, 0, screenWidth, screenHeight, SWP_SHOWWINDOW);
    }

    //Loads the XML file
    private static void loadConfig(string filePath)
    {
        XmlDocument reader = new XmlDocument();
        reader.Load(filePath);

        XmlNode node = reader.DocumentElement.SelectSingleNode("/config/trackingIP");
        trackerIP = node.InnerText;

        node = reader.DocumentElement.SelectSingleNode("/config/platform");
        platform = node.InnerText;
        if (platform.Equals("destiny"))
            destiny = true;
        if (platform.Equals("innovator"))
            innovator = true;

        node = reader.DocumentElement.SelectSingleNode("/config/controllerIP");
        controllerIP = node.InnerText;

        node = reader.DocumentElement.SelectSingleNode("/config/tracking");
        if (node.InnerText.Equals("1"))
            tracking = true;
        else
            tracking = false;

        node = reader.DocumentElement.SelectSingleNode("/config/screenwidth");
        int.TryParse(node.InnerText, out screenWidth);

        node = reader.DocumentElement.SelectSingleNode("/config/screenheight");
        int.TryParse(node.InnerText, out screenHeight);

        node = reader.DocumentElement.SelectSingleNode("/config/fullscreen");
        if (node.InnerText.Equals("1"))
        {
            fullScreen = true;
            Screen.fullScreen = true;
        }
        else
        {
            Screen.fullScreen = false;
            fullScreen = false;
        }

        if (screenWidth > 0 && screenHeight > 0)
            Screen.SetResolution(screenWidth, screenHeight, fullScreen);

        node = reader.DocumentElement.SelectSingleNode("/config/panoptic");
        if (node.InnerText.Equals("1"))
            panOptic = true;
        else
            panOptic = false;

        node = reader.DocumentElement.SelectSingleNode("/config/stereo");
        if (node.InnerText.Equals("1"))
            stereo = true;
        else
            stereo = false;

        node = reader.DocumentElement.SelectSingleNode("/config/interaxial");
        int interax = 0;
        if (int.TryParse(node.InnerText, out interax))
            interaxial = interax;
        else
            interaxial = 55;

        loaded = true;

        if (isInnovator())
        {
            setWindowInnovator();
        }
       
        node = null;
        reader = null;
        GC.Collect();
        GC.WaitForPendingFinalizers();

    }

}
