using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeeNet.Ee
{
    public class Ee
    {
        private readonly GeeNetClient _client;
        private readonly IOptions<GeeNetOptions> _options;
        public Ee(GeeNetClient client, IOptions<GeeNetOptions> options) 
        {
            _client = client;
            _options = options;
        }
        public ImageCollectionBuilder LoadImageCollection(string id) => new ImageCollectionBuilder(id, _client, _options);
        public ImagePixelsBuilder LoadImagePixels(string id) => new ImagePixelsBuilder(id, _client, _options);
    }
}
