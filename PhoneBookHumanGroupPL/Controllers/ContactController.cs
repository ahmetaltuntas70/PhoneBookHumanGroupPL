using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PhoneBookHumanGroupBL.InterfacesOfManagers;
using PhoneBookHumanGroupDL.InterfacesofRepos;
using PhoneBookHumanGroupEL.Entities;

namespace PhoneBookHumanGroupPL.Controllers
{
    public class ContactController : Controller
    {
        private readonly IContactRepo _contactRepo;
        private readonly IMapper _mapper;

        public ContactController(IContactRepo contactRepo, IMapper mapper)
        {
            _contactRepo = contactRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(Contact a)
        {
            a.CreatedDate = DateTime.Parse(DateTime.Now.ToShortDateString());
            a.IsActive = true;
            _contactRepo.Add(a);
            return RedirectToAction("Phones", "Home");
        }
    }
}
