    /// <summary>
    /// ������¼����
    /// </summary>
    //[AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ExceptionLog : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            log4net.LogHelper.WriteError(this.GetType(), filterContext.Exception);
            //log4net.LogHelper.WriteError(this.GetType(), new Exception(string.Format($"�����ַ={HttpContext.Current.Request.Params}")));

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                JsonResult result = new JsonResult();
                result.Data = new BaseResult()
                {
                    result = false, msg = filterContext.Exception.Message,
                    obj = string.Format($"�쳣����{filterContext.Exception.Source}���쳣ʵ��{filterContext.Exception.InnerException}" ),
                    errcode = filterContext.Exception.HResult
                };
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                filterContext.Result = result;
            }
            else
            {
                string errorTraceInfo = filterContext.Exception.StackTrace;
                string errorLine = errorTraceInfo.Substring(errorTraceInfo.IndexOf("λ��"), errorTraceInfo.Length - errorTraceInfo.IndexOf("λ��")).Replace("λ��","");
                errorLine = errorLine.Substring(0, errorLine.IndexOf("\r\n"));
                JsonResult result = new JsonResult();
                result.Data = new BaseResult()
                {
                    result = false,
                    msg = filterContext.Exception.Message,
                    obj = string.Format($"�쳣����{filterContext.Exception.Source}���쳣ʵ��{filterContext.Exception.InnerException}���쳣λ�ã�({errorLine})��"),
                    errcode = filterContext.Exception.HResult
                };
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                filterContext.Result = result;
            }
            filterContext.ExceptionHandled = true;
            base.OnException(filterContext);
        }
    }