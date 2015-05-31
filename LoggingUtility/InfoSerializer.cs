namespace LoggingUtility
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Base class for serializing and deserializing objects
    /// </summary>
    public class InfoSerializer<T> where T : class
    {
        private Encoding _encodeType;
        private XmlWriterSettings _settings;

        /// <summary>
        /// Initializes a new instance of the InfoSerializer class.
        /// </summary>
        protected InfoSerializer()
            : this(Encoding.UTF8)
        {
        }

        /// <summary>
        /// Initializes a new instance of the InfoSerializer class.
        /// </summary>
        /// <param name="encodeType">Type of the encoding to use</param>
        protected InfoSerializer(Encoding encodeType)
        {
            _encodeType = encodeType;
        }

        /// <summary>
        /// Serializes the object to a file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void SerializeToFile(string filePath)
        {
            string xml = SerializeToXml();

            try
            {
                using (MemoryStream memStream = new MemoryStream(_encodeType.GetBytes(xml)))
                {
                    //calling doc.LoadXml with the xml variable will not work
                    //as the string value is not valid xml
                    XmlDocument doc = new XmlDocument();
                    doc.Load(memStream);

                    using (XmlWriter writer = XmlWriter.Create(filePath, XmlWriterSettings))
                    {
                        if (writer != null)
                        {
                            doc.Save(writer);
                        }
                    }
                }
            }
            catch (DirectoryNotFoundException exception)
            {
                string message = string.Format("Path [{0}] contains one or more directories that do not exist", filePath);

                throw new SerializationException(message, exception);
            }
        }

        /// <summary>
        /// Serializes the object to a file.
        /// </summary>
        /// <param name="fileStream">The file stream.</param>
        public void SerializeToFile(Stream fileStream)
        {
            string xml = SerializeToXml();

            try
            {
                using (MemoryStream memStream = new MemoryStream(_encodeType.GetBytes(xml)))
                {
                    //calling doc.LoadXml with the xml variable will not work
                    //as the string value is not valid xml
                    XmlDocument doc = new XmlDocument();
                    doc.Load(memStream);

                    using (XmlWriter writer = XmlWriter.Create(fileStream, XmlWriterSettings))
                    {
                        if (writer != null)
                        {
                            doc.Save(writer);
                        }
                    }
                }
            }
            catch (IOException exception)
            {
                string message = string.Format("Unable to write object contents to stream");

                throw new SerializationException(message, exception);
            }
        }

        /// <summary>
        /// Serializes the object to a string of xml.
        /// </summary>
        /// <returns>The xml version of the object</returns>
        public string SerializeToXml()
        {
            return SerializeObject(this, GetType());
        }

        /// <summary>
        /// Deserializes from file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>An instance of the object</returns>
        public static T DeserializeFromFile(string filePath)
        {
            return DeserializeFromFile(filePath, typeof(T)) as T;
        }

        /// <summary>
        /// Deserializes from XML.
        /// </summary>
        /// <param name="objectXml">The object XML.</param>
        /// <returns>An instance of the object</returns>
        public static T DeserializeFromXml(string objectXml)
        {
            return DeserializeFromXml(objectXml, typeof(T)) as T;
        }


        /// <summary>
        /// Deserializes from file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="deserializeType">Type to deserialize.</param>
        /// <returns>The reconstructed object</returns>
        protected static object DeserializeFromFile(string filePath, Type deserializeType)
        {
            object result = null;

            string xml = string.Empty;

            if (File.Exists(filePath))
            {
                StreamReader reader = null;

                try
                {
                    reader = new StreamReader(filePath);

                    if (reader != null)
                    {
                        xml = reader.ReadToEnd();
                    }
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                }

                if (xml.Length > 0)
                {
                    result = DeserializeFromXml(xml, deserializeType);
                }
            }

            return result;
        }

        /// <summary>
        /// Deserializes from an xml string.
        /// </summary>
        /// <param name="objectXml">The object XML.</param>
        /// <param name="deserializeType">Type to deserialize.</param>
        /// <returns>The reconstructed object</returns>
        protected static object DeserializeFromXml(string objectXml, Type deserializeType)
        {
            return DeserializeObject(objectXml, deserializeType);
        }

        /// <summary>
        /// Deserializes the object.
        /// </summary>
        /// <param name="xml">The xml string to deserialize from</param>
        /// <param name="deserializeType">The type of the object to deserialize.</param>
        /// <returns>The reconstructed object</returns>
        protected static object DeserializeObject(string xml, Type deserializeType)
        {
            object result = null;

            if (!string.IsNullOrEmpty(xml) && !string.IsNullOrEmpty(xml.Trim()) && deserializeType != null)
                try
                {
                    using (StringReader reader = new StringReader(xml))
                    {
                        //create a reader to read the attachments file in
                        using (XmlTextReader xmlReader = new XmlTextReader(reader))
                        {

                            //create the serialized
                            XmlSerializer xmlSerializer = new XmlSerializer(deserializeType);

                            //deserialize
                            result = xmlSerializer.Deserialize(xmlReader);
                        }
                    }
                }
                catch (Exception exception)
                {
                    string message = string.Format("Unable to deserialize {0}.", deserializeType.FullName);

                    throw new SerializationException(message, exception);
                }

            return result;
        }

        /// <summary>
        /// Serializes the object.
        /// </summary>
        /// <param name="serializeData">The object to serialize.</param>
        /// <param name="serializeType">The type of the object to serialize.</param>
        /// <returns>The xml serialized version of the object</returns>
        protected string SerializeObject(object serializeData, Type serializeType)
        {
            string xml = string.Empty;

            if (serializeData != null && serializeType != null)
            {
                try
                {
                    MemoryStream memoryStream = null;

                    using (memoryStream = new MemoryStream())
                    {
                        using (XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, _encodeType))
                        {
                            XmlSerializer typedSerializer = new XmlSerializer(serializeType);

                            //dump the serialized content in the writer
                            typedSerializer.Serialize(xmlTextWriter, serializeData);

                            //get a copy of the data and convert to a string
                            memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                            byte[] data = memoryStream.ToArray();
                            xml = _encodeType.GetString(data);
                        }
                    }
                }
                catch (Exception exception)
                {
                    string message = string.Format("Unable to serialize {0} to file.", GetType().FullName);

                    throw new SerializationException(message, exception);
                }
            }

            return xml;
        }

        private XmlWriterSettings XmlWriterSettings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new XmlWriterSettings();
                    _settings.ConformanceLevel = ConformanceLevel.Document;
                    _settings.Indent = true;
                    _settings.IndentChars = "  ";
                    _settings.NewLineChars = Environment.NewLine;
                    _settings.OmitXmlDeclaration = false;
                    _settings.Encoding = _encodeType;
                }

                return _settings;
            }
        }
    }
}
