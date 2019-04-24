<%@ Page Language="C#" %>
<%@ Import Namespace="Rhythm" %>
<%@ Import Namespace="Rhythm.ErrorHandling" %>
<%@ Import Namespace="Rhythm.Web" %>
<script runat="server">
    public static IErrorDisplayResult GetDisplay(HttpApplication app, HttpContext context)
    {
        var workContext = context == null ? null : context.WorkContext();
        var exists = workContext == null ? null : (IErrorDisplayResult)workContext.Items["ErrorDisplayResult"];
        if (exists != null)
        {
            return exists;
        }
        else
        {
            var errorDisplay = ErrorDisplayDriver.Instance;
            if (errorDisplay != null)
            {
                return errorDisplay.Display(new ErrorDisplayContext
                {
                    Configuration = ErrorDisplayConfigurationUtility.GetConfiguration(),
                    Exception = HttpApplicationUtility.GetException(app, context),
                    WorkContext = workContext,
                    IsLocal = context.Request.IsLocal,
                });
            }
            else
            {
                return null;
            }
        }
    }

</script>
<%
    try
    {
        var display = GetDisplay(this.ApplicationInstance, this.Context);
        if (display != null)
        {
%>
<%=display.ToString()%>
<%
            ErrorDisplayUtility.SetResponse(display, this.Response);
        }
    }
    catch (Exception displayEx)
    {
%>
<!DOCTYPE html>
<!--
    An error occured, but faild to execute ErrorDisplayDriver:
    <%=displayEx.ToString()%>
-->
<html lang="zh-CN">
<head>
    <meta charset="utf-8" />
    <meta http-equiv="content-type" content="text/html; charset=utf-8" />
    <title>呃...</title>
</head>
<body>
    An error occured but unable to display.
</body>
</html>
<% 
    }
%>
