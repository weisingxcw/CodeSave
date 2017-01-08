    /// <summary>
    /// 处理并记录错误
    /// </summary>
    //[AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ExceptionLog : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            log4net.LogHelper.WriteError(this.GetType(), filterContext.Exception);
            //log4net.LogHelper.WriteError(this.GetType(), new Exception(string.Format($"出错地址={HttpContext.Current.Request.Params}")));

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                JsonResult result = new JsonResult();
                result.Data = new BaseResult()
                {
                    result = false, msg = filterContext.Exception.Message,
                    obj = string.Format($"异常对象：{filterContext.Exception.Source}；异常实例{filterContext.Exception.InnerException}" ),
                    errcode = filterContext.Exception.HResult
                };
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                filterContext.Result = result;
            }
            else
            {
                string errorTraceInfo = filterContext.Exception.StackTrace;
                string errorLine = errorTraceInfo.Substring(errorTraceInfo.IndexOf("位置"), errorTraceInfo.Length - errorTraceInfo.IndexOf("位置")).Replace("位置","");
                errorLine = errorLine.Substring(0, errorLine.IndexOf("\r\n"));
                JsonResult result = new JsonResult();
                result.Data = new BaseResult()
                {
                    result = false,
                    msg = filterContext.Exception.Message,
                    obj = string.Format($"异常对象：{filterContext.Exception.Source}；异常实例{filterContext.Exception.InnerException}；异常位置：({errorLine})；"),
                    errcode = filterContext.Exception.HResult
                };
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                filterContext.Result = result;
            }
            filterContext.ExceptionHandled = true;
            base.OnException(filterContext);
        }
    }