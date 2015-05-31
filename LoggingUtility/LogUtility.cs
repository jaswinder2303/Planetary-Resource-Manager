namespace LoggingUtility
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// The type of log message being written
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// The message does not fit any other description
        /// </summary>
        Undefined = 1,
        /// <summary>
        /// This is a debug statement
        /// </summary>
        Trace = 2,
        /// <summary>
        /// A warning message - for minor problems
        /// </summary>
        Warning = 3,
        /// <summary>
        /// An error message - for critical problems
        /// </summary>
        Error = 4
    }

    /// <summary>
    /// The level of messages that should be included in the output log file
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// All messages to be written out, including trace statements
        /// </summary>
        All = 0,
        /// <summary>
        /// Only warnings and errors to be written out
        /// </summary>
        WarningsAndErrors = 2,
        /// <summary>
        /// Only errors should be written out
        /// </summary>
        ErrorsOnly = 3,
        /// <summary>
        /// No log messages should be written out
        /// </summary>
        None = 4
    }

    /// <summary>
    /// 
    /// </summary>
    public enum LogMode
    {
        /// <summary>
        /// The messages are written to file as soon as they are raised.
        /// In this mode messages are not filtered by type
        /// </summary>
        Implicit,
        /// <summary>
        /// The messages are written to file when the utility is told to do so.
        /// In this mode messages are sorted by type.
        /// Requires the WriteMessagesToFile method to be invoked
        /// </summary>
        Explicit
    }

    /// <summary>
    /// Utility class for logging messages
    /// </summary>
    public class LogUtility
    {
        private static LoggingSettings _settings = new LoggingSettings();
        private Dictionary<MessageType, List<MessageLog>> _logLines;
        private DateTime _runTimestamp;
        private DateTime _lastTraceMsgTime = DateTime.MinValue;
        private bool isInEventLogMode;
        private static int _maxSize = 25000000; // default to 25Mb
        private const string SETTINGS_FILE = "LogUtilitySettings.xml";
        private const string LOG_ROOT = "Logs";

        /// <summary>
        /// A struct for holding a message object
        /// </summary>
        internal struct MessageLog
        {
            #region Implementation

            internal const string WARNING_LBL = "[Warning]";
            internal const string ERROR_LBL = "[Error]  ";
            internal const string TRACE_LBL = "[Trace]  ";
            internal const string GENERAL_LBL = "[General]";

            public MessageLog(string msg, MessageType type)
            {
                this.Message = msg;
                this.MessageType = type;
            }

            public string Message;
            public MessageType MessageType;

            public string Label
            {
                get
                {
                    string label = string.Empty;

                    switch (MessageType)
                    {
                        case MessageType.Trace:
                            label = TRACE_LBL;
                            break;
                        case MessageType.Warning:
                            label = WARNING_LBL;
                            break;
                        case MessageType.Error:
                            label = ERROR_LBL;
                            break;
                        default:
                            label = GENERAL_LBL;
                            break;
                    }

                    return label;
                }
            } 

            #endregion
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogUtility"/> class.
        /// </summary>
        /// <param name="productName">Name of the product.</param>
        /// <param name="outputFolder">The output folder. Pass empty string or null to log to executable directory</param>
        /// <param name="extension">The extension for the log file - leading dot is optional (e.g 'log', '.errors' or '.trc'). 
        /// Pass empty string or null for default extension</param>
        /// <param name="level">The MINIMUM level of logging to be included in the output.</param>
        /// <param name="mode">The log writing mode.</param>
        /// <exception cref="ArgumentNullException">Thrown if the product name is null or empty</exception>
        /// <exception cref="PathTooLongException">Thrown if the log file path will exceed the maximum path length</exception>
        public static LogUtility CreateLogger(string productName, string outputFolder, string extension, LogLevel level, LogMode mode)
        {
            #region Implementation

            LogUtility logger = null;

            lock (_settings)
            {
                // Get the location of the executable, set that to our output Directory
                if (String.IsNullOrEmpty(outputFolder))
                {
                    outputFolder = Application.StartupPath;
                }

                //default to the custom folder
                string tempRoot = string.IsNullOrEmpty(outputFolder) ? Environment.CurrentDirectory : outputFolder;
                tempRoot = Path.Combine(tempRoot, LOG_ROOT);
                string source = Path.Combine(tempRoot, CustomFolder);

                Exception settingsError = null;
                bool noCustomSetting = false;
                LoggingSettings settings = null;

                //if there is no folder, use the application folder
                if (!Directory.Exists(source))
                {
                    source = Application.StartupPath;
                    noCustomSetting = true;
                }

                string settingsFile = (Path.Combine(source, SETTINGS_FILE));

                //load existing settings
                if (File.Exists(settingsFile))
                {
                    try
                    {
                        //update the mode & level from the saved settings
                        settings = LoggingSettings.DeserializeFromFile(settingsFile);

                        if (noCustomSetting)
                        {
                            //if there is no customised setting, overwrite the application
                            //default with the settings requested by the caller
                            settings.LoggingLevel = level;
                            settings.LoggingMode = mode;
                        }
                        else
                        {
                            //if there are customised settings, overwrite the caller's settings
                            //with those from the customised settings file
                            level = settings.LoggingLevel;
                            mode = settings.LoggingMode;
                        }
                    }
                    catch (System.Runtime.Serialization.SerializationException error) { settingsError = error; }
                }

                //read in the max log size from the settings file or use the default if not available
                //int maxSize = settings != null ? settings.MaxLogSize : _maxSize;

                //create the logger
                logger = new LogUtility(productName, outputFolder, extension, _maxSize, level, mode);

                //if settings was found but could not be loaded, inform user
                if (settingsError != null)
                {
                    logger.AddException(string.Format("Error reading logging settings from {0}", source), settingsError);
                }

                //save the current settings for the user
                source = Path.Combine(tempRoot, CustomFolder);

                //only write the settings if the user had permissions to create a custom folder
                if (Directory.Exists(source) && (settings == null || noCustomSetting))
                {
                    settingsFile = (Path.Combine(source, SETTINGS_FILE));

                    settings = new LoggingSettings
                    {
                        LoggingLevel = level,
                        LoggingMode = mode
                    };

                    settings.SerializeToFile(settingsFile);
                }
            }

            return logger; 

            #endregion
        }

        /// <summary>
        /// Trashes any existing log settings file.
        /// </summary>
        /// <param name="outputFolder">The output folder.</param>
        private static void ClearSettings(string outputFolder)
        {
            //default to the custom folder
            string tempRoot = string.IsNullOrEmpty(outputFolder) ? Environment.CurrentDirectory : outputFolder;
            tempRoot = Path.Combine(tempRoot, LOG_ROOT);
            string source = Path.Combine(tempRoot, CustomFolder);

            //clear out the custom settings file
            string settingsFile = (Path.Combine(source, SETTINGS_FILE));

            if (File.Exists(settingsFile))
            {
                File.Delete(settingsFile);
            }

            //clear out the application settings file
            source = Environment.CurrentDirectory;
            settingsFile = (Path.Combine(source, SETTINGS_FILE));

            if (File.Exists(settingsFile))
            {
                File.Delete(settingsFile);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogUtility"/> class.
        /// </summary>
        /// <param name="productName">Name of the product.</param>
        /// <param name="outputFolder">The output folder. Pass empty string or null to log to executable directory</param>
        /// <param name="extension">The extension for the log file - leading dot is optional (e.g 'log', '.errors' or '.trc').
        /// Pass empty string or null for default extension</param>
        /// <param name="maxSize">The maximum size of the log file</param>
        /// <param name="level">The MINIMUM level of logging to be included in the output.</param>
        /// <param name="mode">The log writing mode.</param>
        /// <exception cref="ArgumentNullException">Thrown if the product name is null or empty</exception>
        /// <exception cref="PathTooLongException">Thrown if the log file path will exceed the maximum path length</exception>
        private LogUtility(string productName, string outputFolder, string extension, int maxSize, LogLevel level, LogMode mode)
        {
            _logLines = new Dictionary<MessageType, List<MessageLog>>();
            Level = level;
            Mode = mode;
            MaxLogSize = maxSize;
            VetProductName(productName);
            VetOutputFolder(outputFolder);
            VetExtension(extension);
            EnsureUsablePath();

            Reset();
        }

        /// <summary>
        /// Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        public bool HasErrors { get; private set; }
        /// <summary>
        /// Gets a value indicating whether this instance has warnings.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has warnings; otherwise, <c>false</c>.
        /// </value>
        public bool HasWarnings { get; private set; }
        /// <summary>
        /// Gets a value indicating whether this instance has trace.
        /// </summary>
        /// <value><c>true</c> if this instance has trace; otherwise, <c>false</c>.</value>
        public bool HasTrace { get; private set; }
        /// <summary>
        /// Gets the log extension.
        /// </summary>
        /// <value>The log extension.</value>
        public string LogExtension { get; private set; }
        /// <summary>
        /// Gets the output folder.
        /// </summary>
        /// <value>The output folder.</value>
        public string OutputFolder { get; private set; }
        /// <summary>
        /// Gets the name of the product.
        /// </summary>
        /// <value>The name of the product.</value>
        public string ProductName { get; private set; }
        /// <summary>
        /// Gets the log level.
        /// </summary>
        /// <value>The level.</value>
        public LogLevel Level { get; private set; }
        /// <summary>
        /// Gets the log mode.
        /// </summary>
        /// <value>The mode.</value>
        public LogMode Mode { get; private set; }
        /// <summary>
        /// Gets the log file.
        /// </summary>
        /// <value>The log file.</value>
        public string LogFile { get; private set; }

        /// <summary>
        /// Gets or sets the maximum size of the log file.
        /// </summary>
        /// <value>The size of the max log.</value>
        public int MaxLogSize { get; private set; }

        /// <summary>
        /// Gets the custom folder for this user.
        /// </summary>
        /// <value>The user's custom folder.</value>
        private static string CustomFolder
        {
            get
            {
                return MakeIOSafe(string.Format("{0}.{1}", Environment.MachineName, Environment.UserName));
            }
        }

        /// <summary>
        /// Resets this instance.
        /// This clears out all existing errors and creates a new timestamp
        /// </summary>
        public void Reset()
        {
            _runTimestamp = DateTime.Now;
            _lastTraceMsgTime = DateTime.MinValue;
            HasErrors = false;
            HasWarnings = false;
            HasTrace = false;
            LogFile = GenerateLogFileName();

            _logLines.Clear();

            foreach (MessageType messageType in Enum.GetValues(typeof(MessageType)))
            {
                _logLines.Add(messageType, new List<MessageLog>());
            }
        }

        /// <summary>
        /// Adds the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="error">The error.</param>
        public void AddException(string message, Exception error)
        {
            AddMessage(MessageType.Error, message);
            AddException(error);
        }

        /// <summary>
        /// Adds the message.
        /// </summary>
        /// <param name="error">The error.</param>
        public void AddException(Exception error)
        {
            if (error != null)
            {
                AddMessage(MessageType.Error, error.Message);

                string trace = error.StackTrace;
                error = error.InnerException;

                while (error != null)
                {
                    AddMessage(MessageType.Error, error.Message);
                    error = error.InnerException;
                }

                AddMessage(MessageType.Error, trace);
            }
        }


        /// <summary>
        /// Adds the message range.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="messages">The messages.</param>
        public void AddMessageRange(MessageType type, IEnumerable<string> messages)
        {
            if (null != messages)
            {
                foreach (string message in messages)
                {
                    AddMessage(type, message);
                }
            }
        }


        /// <summary>
        /// Adds the message.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The args.</param>
        public void AddMessage(MessageType type, string messageFormat, params object[] args)
        {
            if (!string.IsNullOrEmpty(messageFormat))
            {
                AddMessage(type, string.Format(messageFormat, args));
            }
        }

        /// <summary>
        /// Adds the message.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="message">The message.</param>
        public void AddMessage(MessageType type, string message)
        {
            #region Implementation

            if (!string.IsNullOrEmpty(message))
            {
                if (Mode == LogMode.Explicit)
                {
                    //cache the message
                    _logLines[type].Add(new MessageLog(message, type));

                    switch (type)
                    {
                        case MessageType.Warning:
                            HasWarnings = true;
                            break;
                        case MessageType.Error:
                            HasErrors = true;
                            break;
                        default:
                            HasTrace = true;
                            break;
                    }
                }
                else
                {
                    //determine if the message should be written or not
                    if ((int)type > (int)Level)
                    {
                        string label = string.Empty;

                        switch (type)
                        {
                            case MessageType.Trace:
                                HasTrace = true;
                                label = MessageLog.TRACE_LBL;
                                break;
                            case MessageType.Warning:
                                HasWarnings = true;
                                label = MessageLog.WARNING_LBL;
                                break;
                            case MessageType.Error:
                                HasErrors = true;
                                label = MessageLog.ERROR_LBL;
                                break;
                            default:
                                HasTrace = true;
                                label = MessageLog.GENERAL_LBL;
                                break;
                        }

                        //format the message and send off to be written
                        WriteMessageToFile(string.Format("[{0}]{1}: {2}{3}", DateTime.Now, label, message, Environment.NewLine));
                    }
                }
            } 

            #endregion
        }

        /// <summary>
        /// Adds a timed trace for detecting timespan differences between traces.
        /// Useful for tracing code bottlenecks.
        /// </summary>
        /// <param name="message">The message.</param>
        public void AddTimedTrace(string message)
        {
            lock (_settings)
            {
                if (!string.IsNullOrEmpty(message) && (int)MessageType.Trace > (int)Level)
                {
                    DateTime now = DateTime.Now;
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat("[{0}]{1}", now, MessageLog.TRACE_LBL);

                    if (_lastTraceMsgTime != DateTime.MinValue)
                    {
                        builder.AppendFormat(" Duration=[{0}]", now - _lastTraceMsgTime);
                    }

                    builder.AppendFormat(" {0}", message);

                    if (Mode == LogMode.Explicit)
                    {
                        _logLines[MessageType.Trace].Add(new MessageLog(builder.ToString(), MessageType.Trace));
                    }
                    else
                    {
                        //format the message and send off to be written
                        WriteMessageToFile(string.Format("{0}{1}", builder.ToString(), Environment.NewLine));
                    }

                    HasTrace = true;
                    _lastTraceMsgTime = now;
                }
            }
        }

        /// <summary>
        /// Writes the messages to file.
        /// </summary>
        public void WriteMessagesToFile()
        {
            if (Mode == LogMode.Explicit)
            {
                switch (Level)
                {
                    case LogLevel.WarningsAndErrors:
                        HasTrace = false;
                        break;
                    case LogLevel.ErrorsOnly:
                        HasTrace = false;
                        HasWarnings = false;
                        break;
                    case LogLevel.None:
                        HasTrace = false;
                        HasWarnings = false;
                        HasErrors = false;
                        break;
                    default:
                        break;
                }

                if (HasErrors || HasTrace || HasWarnings)
                {
                    WriteMessageToFile(GetMessages());
                }
            }
        }

        /// <summary>
        /// Writes the message to file.
        /// </summary>
        /// <param name="message">The message.</param>
        private void WriteMessageToFile(string message)
        {
            //lock here to prevent conflicts between threads using the same logger
            lock (this)
            {
                if (isInEventLogMode)
                {
                    WriteMessageToEventLog(ProductName, message);
                }
                else
                {
                    try
                    {
                        //lock here to prevent conflicts between different loggers using the same product name
                        lock (_settings)
                        {
                            bool append = Mode == LogMode.Implicit ? true : false;

                            using (StreamWriter writer = new StreamWriter(LogFile, append))
                            {
                                writer.Write(message);
                                writer.Flush();
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        WriteMessageToEventLog("Iris.Tools.Logger",
                            string.Format("Error writing to log file: {0} ", error.Message));

                        WriteMessageToEventLog(ProductName, message);

                        isInEventLogMode = true;
                    }
                }
            }         
        }

        private void WriteMessageToEventLog(string source, string message)
        {
            //create logging source
            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(source, "Application");
            }

            EventLog.WriteEntry(source, message);
        }

        /// <summary>
        /// Gets the cached messages. Only works in Explicit mode
        /// </summary>
        /// <param name="type">The type of message to retrieve.</param>
        /// <returns></returns>
        public  string GetMessages(MessageType type)
        {
            StringBuilder builder = new StringBuilder();

            foreach(MessageLog line in _logLines[type])
            {
                builder.AppendFormat("{0}: {1}", line.Label, line.Message);
                builder.AppendLine();
            }

            return builder.ToString();
        }

        /// <summary>
        /// Generates the name of the log file.
        /// </summary>
        /// <returns>The file name for the log in this run</returns>
        private string GenerateLogFileName()
        {
            Thread.Sleep(10);//ensure no two files can ever be the same

            return Path.Combine(OutputFolder,
                             string.Format(@"{0}_{1:yyyy-MM-dd_HH-mm-ss-fff}{2}",
                            ProductName, _runTimestamp, LogExtension));
        }

        /// <summary>
        /// Generates the name of the log file to use in this session
        /// </summary>
        /// <returns>A valid log file that can be written to</returns>
        private string GenerateLogFileName1()
        {
            #region Implementation

            bool isValidLog = false;
            int logIndex = 1;
            string logFile = string.Format("{0}{1}", ProductName, LogExtension);
            string currentLogFile = Path.Combine(OutputFolder, logFile);

            //determine the name of the logfile
            while (!isValidLog)
            {
                currentLogFile = Path.Combine(OutputFolder, logFile);

                if (File.Exists(currentLogFile))
                {
                    FileInfo fileInfo = new FileInfo(currentLogFile);

                    if (fileInfo.Length > MaxLogSize)
                    {
                        //update the filename
                        logFile = string.Format("{0}_{1}{2}", ProductName, logIndex, LogExtension);

                        //increment the log index
                        logIndex++;
                    }
                    else
                    {
                        isValidLog = true;
                    }

                    //detach the IO object from the file
                    fileInfo = null;
                }
                else
                {
                    isValidLog = true;
                }
            }

            return currentLogFile;

            #endregion
        }

        /// <summary>
        /// Vets the name of the product.
        /// </summary>
        /// <param name="productName">Name of the product.</param>
        private void VetProductName(string productName)
        {
            if (string.IsNullOrEmpty(productName)) { throw new ArgumentNullException("productName"); }

            // Check product name doesn't contain invalid file characters
            ProductName = MakeIOSafe(productName);
        }

        /// <summary>
        /// Vets the output folder.
        /// </summary>
        /// <param name="outputFolder">The output folder.</param>
        private void VetOutputFolder(string outputFolder)
        {
            string root = string.Empty;
            string customFolder = CustomFolder;

            if (!string.IsNullOrEmpty(outputFolder) && Directory.Exists(outputFolder))
            {
                root = outputFolder;
            }
            else
            {
                //TODO: make this default to user's profile as it's safer

                //default to application folder
                root = Application.StartupPath;
            }

            try
            {
                //lock to prevent multi threaded conflict
                lock (_settings)
                {
                    //make sure we have a common location for log files
                    root = Path.Combine(root, LOG_ROOT);

                    if (!Directory.Exists(root))
                    {
                        Directory.CreateDirectory(root);
                    }

                    //now that the root has been determined, try to customise it for the user
                    string customisedOutputFolder = Path.Combine(root, customFolder);

                    if (!Directory.Exists(customisedOutputFolder))
                    {
                        Directory.CreateDirectory(customisedOutputFolder);
                    }
                }
            }
            catch
            {
                //unable to customise - use the default root instead although
                //this could be a sign that we will not be able to create the log file later.
                customFolder = string.Empty;
            }

            OutputFolder = Path.Combine(root, customFolder);
        }

        /// <summary>
        /// Vets the extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
        private void VetExtension(string extension)
        {
            if (!string.IsNullOrEmpty(extension) && !extension.EndsWith("."))
            {
                // Check product name doesn't contain invalid file characters
                extension = MakeIOSafe(extension);

                if (!extension.StartsWith("."))
                {
                    extension = string.Format(".{0}", extension);
                }

                LogExtension = extension;
            }
            else
            {
                LogExtension = ".log";
            }
        }

        private void EnsureUsablePath()
        {
            string projectedName = GenerateLogFileName();

            //ensure full path name is within the allowed limit - for windows
            if (projectedName.Length > 259)
            {
                throw new PathTooLongException(string.Format(
                    "The provided product name, output folder & extension results in a path " + 
                    "that exceeds the maximum path length of 259 characters" + 
                    "\nProduct Name:{0} \nOutput Folder:{1} \nExtension:{2}", 
                    ProductName, OutputFolder, LogExtension));
            }
        }

        /// <summary>
        /// Makes the name IO-safe.
        /// </summary>
        /// <param name="name">The name.</param>
        private static string MakeIOSafe(string name)
        {
            foreach (char ch in Path.GetInvalidFileNameChars())
            {
                if (name.IndexOf(ch) != -1) { name = name.Replace(ch, '_'); }
            }

            return name;
        }

        /// <summary>
        /// Gets the messages.
        /// </summary>
        /// <returns></returns>
        private string GetMessages()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Log Messages");
            sb.Append("Product  : ").AppendLine(ProductName);
            sb.AppendFormat("Timestamp: {0:yyyy-MM-dd HH:mm:ss}", _runTimestamp).AppendLine().AppendLine();

            foreach(MessageType type in Enum.GetValues(typeof(MessageType)))
            {
                if (HasMessageOfType(type))
                {
                    string header = string.Format("{0} Messages  [Count = {1}]", type, _logLines[type].Count);
                    string underline = new string('=', header.Length);
                    sb.AppendFormat(header).AppendLine();
                    sb.AppendFormat(underline).AppendLine();

                    if (_logLines[type].Count != 0)
                    {
                        sb.AppendLine(GetMessages(type));
                    }
                    else
                    {
                        sb.AppendLine("No Messages");
                    }

                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Determines whether this instance has messages of the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can output the specified type; otherwise, <c>false</c>.
        /// </returns>
        private bool HasMessageOfType(MessageType type)
        {
            switch (type)
            {
                case MessageType.Trace:
                    return HasTrace;
                case MessageType.Warning:
                    return HasWarnings;
                case MessageType.Error:
                    return HasErrors;
                default:
                    return false;
            }
        }
    }
}

