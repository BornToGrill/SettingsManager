# Settings Manager

A reflection based settings manager for .NET applications.

# Background

This project was started to create an extendible and easy to use settings manager.
The main focus was abstraction and customizability.

# Requirements

- .NET Framework 4.5

# Usage

To create an XML settings class simply inherit from the XmlSettings class.

'''c#
public class MySettings : XmlSettings<MySettings> {
    
    public string StringSetting = "Hello";
    public int IntSetting = 50;
}
'''

To load/save a settings class:

'''c#
public static void Main() {

    // Initial save
    MySettings settings = new MySettings();
    settings.SaveAs("LocalSettingsFile.xml");
    
    // To load and save
    MySettings settings = MySettings.Load("LocalSettingsFile.xml");
    settings.IntSettings = 100;
    settings.Save();
}
'''

# Licenses

- [Settings Manager](LICENSE.md)
- [JSON.NET](