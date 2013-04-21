using Sample.Website.Mapping.Tridion.Helpers;
using Sample.Website.Providers;
using Sample.Website.Providers.Contracts;
using Sample.Models;
using DD4T.ContentModel;
using DD4T.ContentModel.Factories;
using DD4T.Mvc.Providers;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Sample.Website.Web.Controllers
{
    public class MenuController : Controller
    {

        private readonly MenuHelper menuHelper;
        private readonly ILinkFactory linkFactory;

        public MenuController(IPageFactory pageFactory, ILinkFactory linkFactory, IPageLinkProvider pageLinkProvider,
                              IComponentFactory componentFactory)
        {
            menuHelper = new MenuHelper(pageFactory, componentFactory, pageLinkProvider);
           this.linkFactory = linkFactory;
        }

        

        
    }
}
