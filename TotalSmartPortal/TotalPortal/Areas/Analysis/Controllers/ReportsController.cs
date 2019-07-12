﻿using System.Net;
using System.Linq;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;

using TotalPortal.APIs.Sessions;

using TotalModel.Models;

using TotalDTO.Analysis;
using TotalCore.Services.Analysis;
using TotalCore.Repositories.Analysis;

using TotalPortal.Controllers;
using TotalPortal.Areas.Analysis.ViewModels;
using TotalPortal.Areas.Analysis.Builders;
using TotalPortal.ViewModels.Helpers;


namespace TotalPortal.Areas.Analysis.Controllers
{
    public class ReportsController : GenericSimpleController<Report, ReportDTO, ReportPrimitiveDTO, ReportViewModel>
    {
        private IReportService reportService;
        private IReportAPIRepository reportAPIRepository;

        public ReportsController(IReportAPIRepository reportAPIRepository, IReportService reportService, IReportSelectListBuilder reportViewModelSelectListBuilder)
            : base(reportService, reportViewModelSelectListBuilder)
        {
            this.reportService = reportService;
            this.reportAPIRepository = reportAPIRepository;
        }

        protected override ReportViewModel InitViewModel(ReportViewModel simpleViewModel)
        {
            simpleViewModel = base.InitViewModel(simpleViewModel);
            simpleViewModel.FromDate = HomeSession.GetReportFromDate(this.HttpContext);
            simpleViewModel.ToDate = HomeSession.GetReportToDate(this.HttpContext);

            return simpleViewModel;
        }

        public ActionResult Viewer(int? id, int? detailID)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ReportIndex reportIndex = this.reportAPIRepository.GetEntityIndexes<ReportIndex>(User.Identity.GetUserId(), HomeSession.GetGlobalFromDate(this.HttpContext), HomeSession.GetGlobalToDate(this.HttpContext)).Where(w => w.ReportUniqueID == id).FirstOrDefault();
            if (reportIndex == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            PrintViewModel printViewModel = new PrintViewModel() { Id = id, DetailID = detailID, PrintOptionID = reportIndex.PrintOptionID, LocationID = this.reportService.LocationID, ReportPath = reportIndex.ReportURL };
            printViewModel.FromDate = HomeSession.GetReportFromDate(this.HttpContext);
            printViewModel.ToDate = HomeSession.GetReportToDate(this.HttpContext);

            return View(viewName: "Viewer", model: printViewModel);
        }
    }
}