namespace PluginsEngine.FileAccess
{
    public static class FileAccessorFactory
    {
        private static IPlugin _plugin;
        private static IAsyncPlugin _asyncPlugin;

        public static IPlugin GetPlugin
        {
            get
            {
                if (_plugin == null)
                    InitializePlugin();
                return _plugin;
            }
        }

        private static void InitializePlugin()
        {
            _plugin = new Standalone.FileAccessPlugin(Config);
            Platform = "Editor";
        }

        public static IAsyncPlugin GetAsyncPlugin
        {
            get
            {
                if (_asyncPlugin == null)
                    InitializeAsyncPlugin();
                return _asyncPlugin;
            }
        }

        private static void InitializeAsyncPlugin()
        {
            _asyncPlugin = new AsyncPluginWrapper(GetPlugin, Config);
        }

        private static FileAccessConfig _config;

        public static FileAccessConfig Config
        {
            get
            {
                if (_config == null)
                {
                    _config = new FileAccessConfig();
                    _config.SavesLocation = System.IO.Path.Combine(
                        System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),
                        _config.SavesLocation);
                }
                return _config;
            }
            set
            {
                _config = value;
            }
        }

        public static void InitializeWithConfig(FileAccessConfig config)
        {
            Config = config;
            InitializeAsyncPlugin();
            InitializePlugin();
        }

        public static void InitializeAsyncWithConfig(FileAccessConfig config)
        {
            Config = config;
            InitializeAsyncPlugin();
        }

        public static string Platform { get; private set; } = "N/A";
    }
}
