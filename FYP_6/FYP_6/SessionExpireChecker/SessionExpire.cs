using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace FYP_6.SessionExpireChecker
{
    public class SessionExpire:ActionFilterAttribute
    {
        //Shared For Both Employees and Admins
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["EmpID"] == null 
                && HttpContext.Current.Session["AdminID"] == null)  
	        {
                HttpContext.Current.Session.Clear();
	            FormsAuthentication.SignOut();  
	           filterContext.Result =  
	          new RedirectToRouteResult(new RouteValueDictionary   
	            {  
	             { "action", "login" },  
	            { "controller", "Home" },  
                //{ "returnUrl", filterContext.HttpContext.Request.RawUrl}  
	             });
	            return;  
	        }
	    }
    }
    public class SessionExpireEmp : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["EmpID"] == null)
            {
                HttpContext.Current.Session.Clear();
                FormsAuthentication.SignOut();
                filterContext.Result =
               new RedirectToRouteResult(new RouteValueDictionary   
	            {  
	             { "action", "login" },  
	            { "controller", "Home" },  
                //{ "returnUrl", filterContext.HttpContext.Request.RawUrl}  
	             });
                return;
            }
        }
    }
    public class SessionExpireStudent : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["rollno"] == null)
            {
                HttpContext.Current.Session.Clear();
                FormsAuthentication.SignOut();
                filterContext.Result =
               new RedirectToRouteResult(new RouteValueDictionary   
	            {  
	             { "action", "login" },  
	            { "controller", "Home" },  
                //{ "returnUrl", filterContext.HttpContext.Request.RawUrl}  
	             });
                return;
            }
        }
    }
    public class SessionExpireTeacher : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["ID"] == null)
            {
                HttpContext.Current.Session.Clear();
                FormsAuthentication.SignOut();
                filterContext.Result =
               new RedirectToRouteResult(new RouteValueDictionary   
	            {  
	             { "action", "login" },  
	            { "controller", "Home" },  
                //{ "returnUrl", filterContext.HttpContext.Request.RawUrl}  
	             });
                return;
            }
        }
    }
    public class SessionExpireAdmin : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["AdminID"] == null)
            {
                HttpContext.Current.Session.Clear();
                FormsAuthentication.SignOut();
                filterContext.Result =
               new RedirectToRouteResult(new RouteValueDictionary   
	            {  
	             { "action", "login" },  
	            { "controller", "Home" },  
                //{ "returnUrl", filterContext.HttpContext.Request.RawUrl}  
	             });
                return;
            }
        }
    }
}