using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace ColonyCoalition
{
    public class MyContentManager : ContentManager
    {
        public MyContentManager(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }

        public MyContentManager(IServiceProvider serviceProvider, string rootDirectory)
            : base(serviceProvider, rootDirectory)
        { }

        public T LoadCopy<T>(string assetName)
        {
            return ReadAsset<T>(assetName, null);
        }
    }
}
