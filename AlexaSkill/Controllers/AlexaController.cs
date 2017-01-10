using System.Net.Http;
using System.Web.Http;
using Sample.Controllers;


namespace AlexaSkill.Controllers
{
    public class AlexaController : ApiController
    {
        [HttpPost, Route("api/alexa/demo")]
        public HttpResponseMessage SampleSession()
        {
            var speechlet = new SessionSpeechlet();
            return speechlet.GetResponse(Request);
        } 
    }
}