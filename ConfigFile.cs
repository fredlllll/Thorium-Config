using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Thorium.IO;
using Thorium.Reflection;

namespace Thorium.Config
{
    public class ConfigFile : Config
    {
        public string FilePath { get; set; }

        public ConfigFile(string file)
        {
            FilePath = file;
            Reload();
        }

        /// <summary>
        /// clears the cache and reloads the contents from a file
        /// </summary>
        public void Reload()
        {
            ClearCache();
            Values = JObject.Parse(File.ReadAllText(FilePath));
        }

        /// <summary>
        /// searches for a file name+"_config.json"
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ConfigFile GetConfig(string name)
        {
            name += "_config.json";

            string file = Files.ResolveFileOrDefault(name);

            return new ConfigFile(file);
        }

        /// <summary>
        /// will return the config using the class name. every uppercase letter will be replaced by its lowercase variant precedented by an _ (except the first letter) and a _config.json will be appended.
        /// this also looks for the default file variant.
        /// 
        /// example: ThoriumClient => thorium_client_config.json => thorium_client_config.json.default
        /// </summary>
        /// <returns></returns>
        public static ConfigFile GetClassConfig()
        {
            Type t = ReflectionHelper.GetCallingType();
            string name = Char.ToLowerInvariant(t.Name[0]) + String.Join("", t.Name.Skip(1).Select(x => char.IsUpper(x) ? ("_" + x) : (char.ToLowerInvariant(x).ToString())));
            return GetConfig(name);
        }
    }
}
