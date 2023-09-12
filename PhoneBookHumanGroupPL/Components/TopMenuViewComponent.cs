using Microsoft.AspNetCore.Mvc;
using PhoneBookHumanGroupBL.InterfacesOfManagers;
using PhoneBookHumanGroupEL.ViewModels;

namespace PhoneBookHumanGroupPL.Components
{
    public class TopMenuViewComponent: ViewComponent
    {
        private readonly IMemberManager _memberManager;

        public TopMenuViewComponent(IMemberManager memberManager)
        {
            _memberManager = memberManager;
        }

        public IViewComponentResult Invoke()
        {
            try
            {
                var loggedInUserEmail = HttpContext.User.Identity.Name;
                if (!string.IsNullOrEmpty(loggedInUserEmail))
                {
                    var loggedInUser = _memberManager.GetByCondition(x => x.Email.ToLower() == loggedInUserEmail.ToLower()).Data;
                    //return View(loggedInUserEmail);
                    //1. yol 1. yöntemi
                    //return View(null, loggedInUser);
                    //1. yol 2. yöntemi
                    // return View("Default", loggedInUser);

                    //2. yol
                    //return View<MemberVM>(loggedInUser);


                    //ViewComponentiçinde return View() yaptığımız yerde defaulttan farklı bir sayfaya gönderme örneği yapalım.
                     return View("TopMenuTheme", loggedInUser);

                }

                return View("TopMenuTheme", new MemberVM());
            }
            catch (Exception ex)
            {
                return View("TopMenuTheme", new MemberVM());
            }
        }
    }
}
