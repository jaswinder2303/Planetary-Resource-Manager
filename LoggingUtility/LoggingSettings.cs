namespace LoggingUtility
{
    using System.Xml.Serialization;

    /// <summary>
    /// Class for storing customised logging settings
    /// </summary>
    [XmlRoot("LoggingSettings", Namespace = "http://www.folatech.com")]
    public class LoggingSettings : InfoSerializer<LoggingSettings>
    {
        /// <summary>
        /// Gets or sets the logging mode.
        /// </summary>
        /// <value>The logging mode.</value>
        [XmlElement("LoggingMode", typeof(LogMode))]
        public LogMode LoggingMode { get; set; }

        /// <summary>
        /// Gets or sets the logging level.
        /// </summary>
        /// <value>The logging level.</value>
        [XmlElement("LoggingLevel", typeof(LogLevel))]
        public LogLevel LoggingLevel { get; set; }
    }
}
