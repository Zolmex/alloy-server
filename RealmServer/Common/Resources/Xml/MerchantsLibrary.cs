#region

using Common.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

#endregion

namespace Common.Resources.Xml
{
    public class MerchantsLibrary
    {
        private static readonly Logger _log = new(typeof(MerchantsLibrary));

        public static readonly Dictionary<string, MerchantDesc> Merchants = new();

        public static void Load(string dir)
        {
            var files = Directory.EnumerateFiles(dir, "*xml", SearchOption.AllDirectories);
            _log.Debug($"Loading merchants...");
            foreach (var file in files)
                MakeDictionaries(XElement.Parse(File.ReadAllText(file)));
            _log.Info("Merchants loaded successfully.");
        }

        private static void MakeDictionaries(XElement root)
        {
            foreach (var xml in root.Elements())
            {
                var region = xml.GetAttribute<string>("region");
                Merchants.Add(region, new MerchantDesc(xml, region));
            }
        }
    }
}