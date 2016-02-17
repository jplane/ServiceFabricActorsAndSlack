using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Actor1.Interfaces;
using Microsoft.AspNet.Mvc;
using Microsoft.ServiceFabric.Actors;

namespace Web1.Controllers
{
    [Route("api/[controller]")]
    public class CounterController : Controller
    {
        private readonly Uri _uri = null;

        public CounterController()
        {
            _uri = new Uri("fabric:/Application1/Actor1ActorService");
        }

        [HttpGet("{id}")]
        public Task<string> Get(string id)
        {
            var proxy = ActorProxy.Create<IActor1>(new ActorId(id), _uri);

            return proxy.GetCountAsync().ContinueWith(t => t.Result.ToString());
        }

        [HttpPut("{id}")]
        public Task Put(string id, [FromBody]string value)
        {
            var proxy = ActorProxy.Create<IActor1>(new ActorId(id), _uri);

            return proxy.SetCountAsync(int.Parse(value));
        }

        [HttpPost]
        public async Task<JsonResult> Post([FromForm]string text)
        {
            var payload = text.Split(':').Last().Trim().Split(' ');

            var verb = payload[0];

            var id = payload[1];

            var proxy = ActorProxy.Create<IActor1>(new ActorId(id), _uri);

            switch (verb)
            {
                case "get":
                    var result = await proxy.GetCountAsync();
                    return Json(new {text = result.ToString()});

                case "put":
                    var value = int.Parse(payload[2]);
                    await proxy.SetCountAsync(value);
                    return Json(new {text = string.Empty});

                default:
                    throw new InvalidOperationException("Unrecognized verb: " + verb);
            }
        }
    }

    public class SlackData
    {
        public string token { get; set; }
        public string team_id { get; set; }
        public string team_domain { get; set; }
        public string channel_id { get; set; }
        public string channel_name { get; set; }
        public string timestamp { get; set; }
        public string user_id { get; set; }
        public string user_name { get; set; }
        public string text { get; set; }
        public string trigger_word { get; set; }
    }
}
