using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace fill_a_pix
{
    public class FillAPix
    {
        private FillAPix()
        {
        }

        public static FillAPix FromStream(Stream stream)
        {
            using (var reader = new StreamReader(stream))
                return FromString(reader.ReadToEnd());
        }
        public static async Task<FillAPix> FromStreamAsync(Stream stream)
        {
            using (var reader = new StreamReader(stream))
                return FromString(await reader.ReadToEndAsync());
        }
        public static FillAPix FromString(string xml)
        {
            var puzzle = new FillAPix();

            var doc = XDocument.Parse(xml);

            var headerEl = doc.Descendants("header").First();

            var formatVersion = headerEl.Descendants("formatVersion").First();
            var majorFormatVersion = (int)formatVersion.Attribute("major");
            var minorFormatVersion = (int)formatVersion.Attribute("minor");
            if (majorFormatVersion != 2 || minorFormatVersion != 0) throw new NotSupportedException($"Invalid format version. Major: {majorFormatVersion}, Minor: {minorFormatVersion}.");

            puzzle.Guid = Guid.Parse((string)headerEl.Descendants("guid").First());

            var code = headerEl.Descendants("code").First();
            puzzle.CodeFamily = (int)code.Attribute("family");
            puzzle.CodeVariant = (int)code.Attribute("variant");
            puzzle.CodeModel = (string)code.Attribute("model");

            puzzle.SerialNumber = (string)headerEl.Descendants("serialNumber").First();

            puzzle.CreationDate = (DateTime)headerEl.Descendants("creationDate").First();

            var propertiesEl = headerEl.Descendants("properties").First();
            puzzle.Width = (int)propertiesEl.Descendants("numeric").Where(el => (string)el.Attribute("name") == "width").First().Attribute("value");
            puzzle.Height = (int)propertiesEl.Descendants("numeric").Where(el => (string)el.Attribute("name") == "height").First().Attribute("value");
            puzzle.Name = (string)propertiesEl.Descendants("text").Where(el => (string)el.Attribute("name") == "name").First().Attribute("value");
            puzzle.Difficulty = (int)propertiesEl.Descendants("numeric").Where(el => (string)el.Attribute("name") == "difficulty").First().Attribute("value");

            var dataEl = doc.Descendants("data").First();
            puzzle.Palette = dataEl
                .Descendants("palette")
                .Descendants("color")
                .Select(el => parseColor((string)el.Attribute("rgb")))
                .ToArray();

            puzzle.Source = dataEl
                .Descendants("source")
                .Descendants("row")
                .Select(el => ((string)el).Split(' ').Select(int.Parse).ToArray())
                .ToArray();

            puzzle.Solution = dataEl
                .Descendants("solution")
                .Descendants("row")
                .Select(el => ((string)el).Split(' ').Select(int.Parse).ToArray())
                .ToArray();

            return puzzle;
        }

        private static Color parseColor(string rgb)
        {
            return Color.FromArgb(
                Convert.ToInt32(rgb.Substring(0, 2), 16),
                Convert.ToInt32(rgb.Substring(2, 2), 16),
                Convert.ToInt32(rgb.Substring(4, 2), 16)
            );
        }

        public Guid Guid { get; private set; }

        public int CodeFamily { get; private set; }
        public int CodeVariant { get; private set; }
        public string CodeModel { get; private set; }

        public string SerialNumber { get; private set; }

        public DateTime CreationDate { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public string Name { get; private set; }
        public int Difficulty { get; private set; }

        public Color[] Palette { get; private set; }

        public int[][] Source { get; private set; }
        public int[][] Solution { get; private set; }
    }
}
