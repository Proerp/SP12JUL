using RequireJsNet;

using TotalBase.Enums;
using TotalCore.Services;
using TotalPortal.APIs.Sessions;


namespace TotalPortal.Controllers
{
    public abstract class BaseController : CoreController
    {
        private readonly IBaseService baseService;
        public BaseController(IBaseService baseService)
        { this.baseService = baseService; }


        public IBaseService BaseService { get { return this.baseService; } }



        public virtual void AddRequireJsOptions()
        {
            GlobalEnums.NmvnTaskID moduleDetailID = this.ModuleDetailID;
            int moduleID = this.baseService.GetModuleID(moduleDetailID);

            MenuSession.SetModuleID(this.HttpContext, moduleID);

            RequireJsOptions.Add("LocationID", this.baseService.LocationID, RequireJsOptionsScope.Page);
            RequireJsOptions.Add("ModuleID", moduleID, RequireJsOptionsScope.Page);
            RequireJsOptions.Add("ModuleDetailID", (int)moduleDetailID, RequireJsOptionsScope.Page);
            RequireJsOptions.Add("NmvnTaskID", this.baseService.NmvnTaskID, RequireJsOptionsScope.Page);
        }

        public virtual GlobalEnums.NmvnTaskID ModuleDetailID { get { return this.baseService.NmvnTaskID; } }
    }
}