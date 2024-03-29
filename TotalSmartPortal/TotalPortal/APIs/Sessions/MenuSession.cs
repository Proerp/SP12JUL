﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TotalPortal.APIs.Sessions
{
    public class HomeSession
    {
        public static int? TryParseID(string storedSessionString)
        {
            try
            {
                if (storedSessionString != null && storedSessionString != "")
                {
                    if (storedSessionString.IndexOf("#@#") > 0)
                    {
                        int id;
                        if (int.TryParse(storedSessionString.Substring(0, storedSessionString.IndexOf("#@#")), out id))
                            return id;
                        else
                            return null;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }


        public static string TryParseName(string storedSessionString)
        {
            try
            {
                if (storedSessionString != null && storedSessionString != "")
                {
                    if (storedSessionString.IndexOf("#@#") > 0)
                        return storedSessionString.Substring(storedSessionString.IndexOf("#@#") + 3);
                    else
                        return null;
                }
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }


        public static DateTime GetGlobalFromDate(HttpContextBase context)
        {
            if (context.Session["GlobalFromDate"] == null)
                return DateTime.Today.AddDays(TotalBase.Enums.GlobalEnums.CBPP ? -10 : -10);
            else
                return (DateTime)context.Session["GlobalFromDate"];
        }

        public static void SetGlobalFromDate(HttpContextBase context, DateTime globalFromDate)
        {
            context.Session["GlobalFromDate"] = globalFromDate;
        }



        public static DateTime GetGlobalToDate(HttpContextBase context)
        {
            if (context.Session["GlobalToDate"] == null)
                return DateTime.Today.AddDays(60).AddHours(23).AddMinutes(59).AddSeconds(59);
            else
                return (DateTime)context.Session["GlobalToDate"];
        }

        public static void SetGlobalToDate(HttpContextBase context, DateTime globalToDate)
        {
            context.Session["GlobalToDate"] = globalToDate;
        }







        public static DateTime GetReportFromDate(HttpContextBase context)
        {
            if (context.Session["ReportFromDate"] == null)
                return TotalBase.Enums.GlobalEnums.CBPP ? DateTime.Today : new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            else
                return (DateTime)context.Session["ReportFromDate"];
        }

        public static void SetReportFromDate(HttpContextBase context, DateTime globalFromDate)
        {
            context.Session["ReportFromDate"] = globalFromDate;
        }



        public static DateTime GetReportToDate(HttpContextBase context)
        {
            if (context.Session["ReportToDate"] == null)
            {
                DateTime toDate = DateTime.Today;
                if (!TotalBase.Enums.GlobalEnums.CBPP)
                {
                    toDate = toDate.AddMonths(1);
                    toDate = (new DateTime(toDate.Year, toDate.Month, 1)).AddDays(-1);
                }
                return toDate.AddHours(23).AddMinutes(59).AddSeconds(59);
            }
            else
                return (DateTime)context.Session["ReportToDate"];
        }

        public static void SetReportToDate(HttpContextBase context, DateTime globalToDate)
        {
            context.Session["ReportToDate"] = globalToDate;
        }





        public static Nullable<int> GetSessionCash(HttpContextBase context, string cacheName, int anyIntValue)
        {
            if (context.Session[cacheName] == null)
                return null;
            else
                return (int)context.Session[cacheName];
        }

        public static void SetSessionCash(HttpContextBase context, string cacheName, int cacheValue)
        {
            context.Session[cacheName] = cacheValue;
        }


        public static string GetSessionCash(HttpContextBase context, string cacheName)
        {
            if (context.Session[cacheName] == null)
                return null;
            else
                return (string)context.Session[cacheName];
        }

        public static void SetSessionCash(HttpContextBase context, string cacheName, string cacheValue)
        {
            context.Session[cacheName] = cacheValue;
        }
    }

    public class MenuSession
    {
        public static int GetUserLocked(HttpContextBase context)
        {
            if (context.Session["UserLocked"] == null)
                return 0;
            else
                return (int)context.Session["UserLocked"];
        }

        public static void SetUserLocked(HttpContextBase context, int UserLocked)
        {
            context.Session["UserLocked"] = UserLocked;
        }


        public static int GetModuleID(HttpContextBase context)
        {
            if (context.Session["ModuleID"] == null)
                return 0;
            else
                return (int)context.Session["ModuleID"];
        }

        public static void SetModuleID(HttpContextBase context, int moduleID)
        {
            context.Session["ModuleID"] = moduleID;
        }

        public static int GetTaskID(HttpContextBase context)
        {
            if (context.Session["TaskID"] == null)
                context.Session["TaskID"] = 0;

            return (int)context.Session["TaskID"];
        }

        public static void SetTaskID(HttpContextBase context, int taskID)
        {
            context.Session["TaskID"] = taskID;
        }

        public static string GetModuleName(HttpContextBase context)
        {
            if (context.Session["ModuleName"] == null)
                context.Session["ModuleName"] = "";

            return (string)context.Session["ModuleName"];
        }

        public static void SetModuleName(HttpContextBase context, string moduleName)
        {
            if (!string.IsNullOrWhiteSpace(moduleName))
            {
                context.Session["ModuleName"] = moduleName;
            }
            else
                context.Session["ModuleName"] = "";
        }

        public static string GetTaskName(HttpContextBase context)
        {
            if (context.Session["TaskName"] == null)
                context.Session["TaskName"] = "";

            return (string)context.Session["TaskName"];
        }

        public static void SetTaskName(HttpContextBase context, string taskName)
        {
            if (!string.IsNullOrWhiteSpace(taskName))
            {
                //context.Session["TaskName"] = "\\ " + taskName;
                context.Session["TaskName"] = taskName;
            }
            else
                context.Session["TaskName"] = "";
        }

        public static string GetTaskController(HttpContextBase context)
        {
            if (context.Session["TaskController"] == null)
                context.Session["TaskController"] = "";

            return (string)context.Session["TaskController"];
        }

        public static void SetTaskController(HttpContextBase context, string taskController)
        {
            if (!string.IsNullOrWhiteSpace(taskController))
            {
                context.Session["TaskController"] = taskController;
            }
            else
                context.Session["TaskController"] = "";
        }





        public static string GetFreshSession(HttpContextBase context)
        {
            return (string)context.Session["FreshSession"];
        }

        public static void SetFreshSession(HttpContextBase context, string freshSession)
        {
            context.Session["FreshSession"] = freshSession;
        }
    }

    public class Helpers
    {
        public static DateTime InitDateTime(int year, int month, int day)
        {
            return Sessions.Helpers.InitDateTime(year, month, day, 0, 0, 0);
        }
        public static DateTime InitDateTime(int year, int month, int day, int hours, int minutes, int seconds)
        {
            return new DateTime(year, month, day).AddHours(hours).AddMinutes(minutes).AddSeconds(seconds);
        }
    }
}