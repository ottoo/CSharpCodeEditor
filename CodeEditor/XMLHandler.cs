using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace CodeEditor
{
    /// <summary>
    /// Class for saving the prefereces to a xml-file. Specifically the location of
    /// the csc.exe
    /// </summary>
    class XMLHandler
    {

        /// <summary>
        /// Saves the location of the csc.exe to a xml-file.
        /// </summary>
        /// <param name="preferences"></param>
        public void SerializeToXML(Prefs preferences)
        {
            if (!File.Exists(@"C:\prefs.xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Prefs));
                TextWriter textWriter = new StreamWriter(@"C:\prefs.xml");
                serializer.Serialize(textWriter, preferences);
                textWriter.Close();
            }
        }

        /// <summary>
        /// Loads the location of the csc.exe file from the xml-file.
        /// </summary>
        /// <returns></returns>
        public string DeserializeFromXML()
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(Prefs));
            TextReader textReader = new StreamReader(@"C:\prefs.xml");
            Prefs p = (Prefs)deserializer.Deserialize(textReader);
            textReader.Close();

            return p.filePath;
        }

        
    }
}
