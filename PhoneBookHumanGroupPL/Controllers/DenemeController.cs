using AutoMapper;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using PhoneBookHumanGroupDL.ContextInfo;
using PhoneBookHumanGroupDL.ImplementationofRepos;
using PhoneBookHumanGroupDL.InterfacesofRepos;
using PhoneBookHumanGroupEL.Entities;
using PhoneBookHumanGroupEL.ViewModels;
using PhoneBookHumanGroupPL.Models;

namespace PhoneBookHumanGroupPL.Controllers
{
    public class DenemeController : Controller
    {
        //NOT: BL katmanındaki managerlar olmadan da bir controller içinde işlem yapalım

        private readonly IPhoneGroupRepo _phoneGroupRepo;
        private readonly IMemberPhoneRepo _memberPhoneRepo;
        private readonly IMapper _mapper;

        public DenemeController(IPhoneGroupRepo phoneGroupRepo, IMemberPhoneRepo memberPhoneRepo, IMapper mapper)
        {
            _phoneGroupRepo = phoneGroupRepo;
            _memberPhoneRepo = memberPhoneRepo;
            _mapper = mapper;
        }

        public IActionResult AllPhoneGroups()
        {
            try
            {
                //1. yol 
                //var data = _phoneGroupRepo.GetAll().ToList();
                //return View("AllPhoneGroupView1", data);

                //2.yol Bu işlem best practice
                var data = _phoneGroupRepo.GetAll();

                List<PhoneGroupVM> datalist = new List<PhoneGroupVM>();

                var model = _mapper.Map<IQueryable<PhoneGroup>, List<PhoneGroupVM>>(data);
                foreach (var item  in model)
                {
                    item.ContactCount=_memberPhoneRepo.GetAll(x=> x.PhoneGroupId==item.Id).ToList().Count;
                }
                return View("AllPhoneGroupView2", model);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IActionResult StaticExcelReport()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Sayfa1");
            workSheet.Cells[1, 1].Value = "İsim";
            workSheet.Cells[1, 2].Value = "Soyisim";
            workSheet.Cells[1, 3].Value = "Email";

            workSheet.Cells[2, 1].Value = "Ahmet";
            workSheet.Cells[2, 2].Value = "Altuntaş";
            workSheet.Cells[2, 3].Value = "ferhatfero53@gmail.com";

            var bytes = excel.GetAsByteArray();
            return File(bytes, "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet", "dosya1.xlsx");
        }
        //public List<MemberModel> MemberList()
        //{
        //    List<MemberModel> bm = new List<MemberModel>();
        //    using (var c = new PhoneBookHumanGroupContext())
        //    {
        //        bm= c.MemberTable.Select(x=> new MemberModel
        //        {
        //            Name = x.Name,
        //            Surname = x.Surname,
        //            Email = x.Email,
        //            BirthDate = x.BirthDate
        //        }).ToList();
        //        return bm;  
        //    }
        //}
        //public IActionResult MemberExcelReport()
        //{
        //    using (var workBook = new XLWorkbook())
        //    {
        //        var workSheet = workBook.Worksheets.Add("Liste");
        //        workSheet.Cell(1, 1).Value = "İsim";
        //        workSheet.Cell(1, 2).Value = "Soyisim";
        //        workSheet.Cell(1, 3).Value = "Mail";

        //        int rowCount = 2;
        //        foreach (var item in MemberList())
        //        {
        //            workSheet.Cell(rowCount, 1).Value = item.Name;
        //            workSheet.Cell(rowCount, 2).Value = item.Surname;
        //            workSheet.Cell(rowCount, 3).Value = item.Email;
        //            rowCount++;
        //        }
        //        using (var stream = new MemoryStream())
        //        {
        //            workBook.SaveAs(stream);
        //            var content = stream.ToArray();
        //            return File(content, "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet", "YeniListe.xlsx");
        //        }
        //    }
        //}
    }
}
