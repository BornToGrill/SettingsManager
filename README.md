# Settings Manager

A reflection based settings manager for .NET applications.

# Background

This project was started to create an extendible and easy to use settings manager.
The main focus was abstraction and customizability.

# Requirements

- .NET Framework 4.5

# Usage

To create an XML settings class simply inherit from the XmlSettings class.

    public class MySettings : XmlSettings<MySettings> {
        
        public string StringSetting = "Hello";
        public int IntSetting = 50;
        
    }

To save a settings class without loading one first.

    MySettings settings = new MySettings();
    settings.IntSetting = 70;
    settings.SaveAs("LocalSettingsFile.xml");

To load a settings file and save to the same file.

    MySettings settings = MySettings.Load("LocalSettingsFile.xml");
    settings.IntSettings = 100;
    settings.Save();

# Licenses

- [Settings Manager](LICENSE.md)
- [JSON.NET](Third party licenses/JSON.NET License.md)

# About

This project was created by Daniel Molenaar
- [Website](http://daniel-molenaar.com/)