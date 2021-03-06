﻿namespace AdvancedMvc.Framework
{
    public class MvcContext
    {
        private static MvcContext instance;

        private MvcContext()
        {
        }

        public static MvcContext Get
        {
            get { return instance == null ? (instance = new MvcContext()) : instance; }
        }

        public string AssemblyName { get; set; }

        public string ControllersFolder { get; set; } = "Controllers";

        public string ControllersSuffix { get; set; } = "Controller";

        public string ViewsFolder { get; set; } = "Views";

        public string ModelsFolder { get; set; } = "Models";

        public string ResourceFolder { get; set; } = "Resources";
    }
}