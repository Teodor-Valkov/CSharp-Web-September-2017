namespace HandmadeHttpServer.Server.Models
{
    using System.Collections.Generic;

    public class UserModel
    {
        private readonly Dictionary<string, object> objects;

        public UserModel()
        {
            this.objects = new Dictionary<string, object>();
        }

        public object this[string key]
        {
            get { return this.objects[key]; }
            set { this.objects[key] = value; }
        }

        public void AddObject(string key, object value)
        {
            this.objects[key] = value;
        }
    }
}