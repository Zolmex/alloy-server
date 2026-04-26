using System.Xml.Linq;
using Common.Utilities;

namespace Common.Resources.Xml.Descriptors;

public class TextureDesc {
    public readonly string File;
    public readonly int Index;

    public TextureDesc(XElement e) {
        File = e.GetValue<string>("File");
        Index = e.GetValue<int>("Index");
    }
}