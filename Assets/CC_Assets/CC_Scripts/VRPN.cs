using UnityEngine;
using System.Runtime.InteropServices;

/*
The MIT License(MIT)

Copyright(c) 2014 Scott Redig

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

Code obtained from https://github.com/Laremere/unityVRPN
*/

public static class VRPN {
    [DllImport("unityVrpn")]
    private static extern double vrpnAnalogExtern(string address, int channel, int frameCount);

    [DllImport("unityVrpn")]
    private static extern bool vrpnButtonExtern(string address, int channel, int frameCount);

    [DllImport("unityVrpn")]
    private static extern double vrpnTrackerExtern(string address, int channel, int component, int frameCount);

    public static double vrpnAnalog(string address, int channel) {
        return vrpnAnalogExtern(address, channel, Time.frameCount);
    }

    public static bool vrpnButton(string address, int channel) {
        return vrpnButtonExtern(address, channel, Time.frameCount);
    }

    public static Vector3 vrpnTrackerPos(string address, int channel) {
        return new Vector3(
            (float)vrpnTrackerExtern(address, channel, 0, Time.frameCount),
            (float)vrpnTrackerExtern(address, channel, 1, Time.frameCount),
            (float)vrpnTrackerExtern(address, channel, 2, Time.frameCount));
    }

    public static Quaternion vrpnTrackerQuat(string address, int channel) {
        return new Quaternion(
            (float)vrpnTrackerExtern(address, channel, 3, Time.frameCount),
            (float)vrpnTrackerExtern(address, channel, 4, Time.frameCount),
            (float)vrpnTrackerExtern(address, channel, 5, Time.frameCount),
            (float)vrpnTrackerExtern(address, channel, 6, Time.frameCount));
    }
}