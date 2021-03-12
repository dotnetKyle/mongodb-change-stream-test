using Newtonsoft.Json;
using System;
using System.IO;

namespace ChangeStreamTest
{
    public class AppSettings
    {
        public string DbConnectionString { get; set; }
        public string DatabaseName { get; set; }

        /// <summary>
        /// Save new app settings
        /// </summary>
        public void SaveSettings()
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(GetAppDataFilePath(), json);
        }

        /// <summary>
        /// Get the app settings from app data
        /// </summary>
        public static AppSettings GetSettings()
        {
            try
            {
                if (File.Exists(GetAppDataFilePath()) == false)
                    return null;

                var json = File.ReadAllText(GetAppDataFilePath());
                return JsonConvert.DeserializeObject<AppSettings>(json);
            }
            catch
            {
                return null;
            }
        }

        static string GetAppDataDirectoryPath()
        {
            var appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var path = Path.Combine(appDataDirectory, "change-stream-test");

            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);

            return path;
        }

        static string GetAppDataFilePath()
            => Path.Combine(GetAppDataDirectoryPath(), "appsettings.json");
    }
}
