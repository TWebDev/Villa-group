using System.Web;
using System.Web.Mvc;
/// <summary>
/// Transfers execution to the supplied url.
/// </summary>
public class TransferResult : RedirectResult
{
    public TransferResult(string url) : base(url) { }

    public override void ExecuteResult(ControllerContext context)
    {
        var httpContext = HttpContext.Current;

        // ASP.NET MVC 3.0
        if (context.Controller.TempData != null &&
            context.Controller.TempData.Count > 0)
        {
            throw new System.ApplicationException("TempData won't work with Server.TransferRequest!");
        }
        try
        {

            httpContext.Server.TransferRequest(Url, true); // change to false to pass query string parameters if you have already processed them
        }
        catch
        {
            //var httpContext = HttpContext.Current;
            httpContext.RewritePath(Url, false);
            IHttpHandler httpHandler = new MvcHttpHandler();
            httpHandler.ProcessRequest(HttpContext.Current);
        }

    }
}