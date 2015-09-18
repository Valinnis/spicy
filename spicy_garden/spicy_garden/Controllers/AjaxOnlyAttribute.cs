using System;
using System.Reflection;
using System.Web.Mvc;

namespace spicy_garden.Controllers
{
	public class AjaxOnlyAttribute : ActionMethodSelectorAttribute
	{
		public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
		{
			return controllerContext.RequestContext.HttpContext.Request.IsAjaxRequest();
		}
	}
}