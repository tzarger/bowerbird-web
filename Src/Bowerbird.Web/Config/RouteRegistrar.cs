﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Reflection;
using Bowerbird.Web.Controllers;

namespace Bowerbird.Web.Config
{
    public class RouteRegistrar
    {
        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public static void RegisterRoutesTo(RouteCollection routes)
        {
            routes.IgnoreRoute(
                "{resource}.axd/{*pathInfo}");

            routes.IgnoreRoute(
                "{*favicon}",
                new {favicon = @"(.*/)?favicon.ico(/.*)?"});

            routes.MapRoute(
               "video-upload",
               "videoupload",
               new { controller = "mediaresources", action = "videoupload" });

            routes.MapRoute(
               "video-preview",
               "videopreview",
               new { controller = "mediaresources", action = "videopreview" });
            //new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("POST") });

            routes.MapRoute(
                "account-resetpassword",
                "account/resetpassword/{resetpasswordkey}",
                new {controller = "account", action = "resetpassword", resetpasswordkey = UrlParameter.Optional});

            routes.MapRoute(
                "home-index-private",
                "",
                new {controller = "home", action = "privateindex"},
                new {authorised = new AuthenticatedConstraint()});

            routes.MapRoute(
                "home-index-public",
                "",
                new {controller = "home", action = "publicindex"});

            routes.MapRoute(
                "home-index-private-stream",
                "stream",
                new { controller = "home", action = "stream" });

            //routes.MapRoute(
            //    "user-update",
            //    "user/update",
            //    new { controller = "user", action = "updateform" },
            //    new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("GET") });


            RegisterGroupControllerRoutes(routes);

            // Load up restful controllers and create routes
            RegisterRestfulControllerRoutes(routes);

            routes.MapRoute("Templates", "templates/{name}",
                            new {controller = "Template", action = "Get"});

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new {controller = "home", action = "index", id = UrlParameter.Optional},
                new[] {"Bowerbird.Web.Controllers"});
        }

        private static void RegisterGroupControllerRoutes(RouteCollection routes)
        {
            var controllers = new[] 
                {
                    typeof(OrganisationsController).Name.ToLower(),
                    typeof(TeamsController).Name.ToLower(),
                    typeof(ProjectsController).Name.ToLower()
                };

            foreach (var controller in controllers)
            {
                CreateGroupControllerRoute(routes, controller.Replace("controller", "").Trim());
            }
        }        

        private static void RegisterRestfulControllerRoutes(RouteCollection routes)
        {
            var controllers = Assembly.Load("Bowerbird.Web").GetTypes().Where(x => x.Namespace == "Bowerbird.Web.Controllers" && x.BaseType != null && x.BaseType.Name == "ControllerBase");

            foreach (var controller in controllers.Where(controller => controller.GetCustomAttributes(true).Any(x => x is RestfulAttribute)))
            {
                CreateRestfulControllerRoute(routes, controller.Name.ToLower().Replace("controller", "").Trim());
            }
        }

        private static void CreateGroupControllerRoute(RouteCollection routes, string controllerName)
        {
            routes.MapRoute(
                controllerName + "-activity",
                controllerName + "/{id}/activity",
                new { controller = controllerName, action = "activity" },
                new { httpMethod = new HttpMethodConstraint("GET"), id = @"^((?!create|update|delete|explore).*)$", acceptType = new AcceptTypeContstraint("application/json") });

            routes.MapRoute(
                controllerName + "-sections",
                controllerName + "/{id}/{action}",
                new { controller = controllerName, action = "activity" },
                new { httpMethod = new HttpMethodConstraint("GET"), id = @"^((?!create|update|delete|explore).*)$", acceptType = new AcceptTypeContstraint("text/html") });

            routes.MapRoute(
                controllerName + "-explore",
                controllerName + "/explore",
                new { controller = controllerName, action = "explore" },
                new { httpMethod = new HttpMethodConstraint("GET") });
        }

        private static void CreateRestfulControllerRoute(RouteCollection routes, string controllerName)
        {
            // Restful get many method
            routes.MapRoute(
                controllerName + "-get-many",
                controllerName,
                new { controller = controllerName, action = "getmany" },
                new { httpMethod = new HttpMethodConstraint("GET") });

            // Restful get one method
            routes.MapRoute(
                controllerName + "-get-one",
                controllerName + "/{id}", 
                new { controller = controllerName, action = "getone" },
                new { httpMethod = new HttpMethodConstraint("GET"), id = @"^((?!create|update|delete|explore).*)$" });

            // Non-restful utility method to get data for displaying form
            routes.MapRoute(
                controllerName + "-create-form",
                controllerName + "/create",
                new { controller = controllerName, action = "createform" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("GET") });

            // Non-restful utility method to get data for displaying form
            routes.MapRoute(
                controllerName + "-update-form",
                controllerName + "/{id}/update",
                new { controller = controllerName, action = "updateform" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("GET"), id = @"^((?!create|update|delete).*)$" });

            // Non-restful utility method to get data for displaying form
            routes.MapRoute(
                controllerName + "-delete-form",
                controllerName + "/{id}/delete",
                new { controller = controllerName, action = "deleteform" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("GET"), id = @"^((?!create|update|delete).*)$" });

            // Restful update method
            routes.MapRoute(
                controllerName + "-update",
                controllerName + "/{id}",
                new { controller = controllerName, action = "update" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("PUT") },
                new[] { "Bowerbird.Web.Controllers" });

            // Restful create method
            routes.MapRoute(
                controllerName + "-create",
                controllerName + "/",
                new { controller = controllerName, action = "create" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("POST") },
                new[] { "Bowerbird.Web.Controllers" });

            // Restful delete method
            routes.MapRoute(
                controllerName + "-delete",
                controllerName + "/{id}",
                new { controller = controllerName, action = "delete" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("DELETE") },
                new[] { "Bowerbird.Web.Controllers" });
        }

        #endregion
    }
}

#region old manual controller routes

//routes.MapRoute(
//    "observations-get-many",
//    "observations",
//    new {controller = "observations", action = "getmany"},
//    new {httpMethod = new HttpMethodConstraint("GET")});

//routes.MapRoute(
//    "observations-get-one",
//    "observations/{id}",
//    new {controller = "observations", action = "getone"},
//    new {httpMethod = new HttpMethodConstraint("GET"), id = @"^((?!create|update|delete).*)$"});

//routes.MapRoute(
//    "observations-create-form",
//    "observations/create",
//    new {controller = "observations", action = "createform"},

//    new {authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("GET")});

//routes.MapRoute(
//    "observations-update-form",
//    "observations/update/{id}",
//    new {controller = "observations", action = "updateform"},
//    new {authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("GET")});

//routes.MapRoute(
//    "observations-delete-form",
//    "observations/delete/{id}",
//    new {controller = "observations", action = "deleteform"},
//    new {authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("GET")});

//routes.MapRoute(
//    "observations-create",
//    "observations/",
//    new {controller = "observations", action = "create"},
//    new {authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("POST")});

//routes.MapRoute(
//    "observations-update",
//    "observations/{id}",
//    new {controller = "observations", action = "update"},
//    new {authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("PUT")});

//routes.MapRoute(
//    "observations-delete",
//    "observations/{id}",
//    new {controller = "observations", action = "delete"},
//    new {authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("DELETE")});

//routes.MapRoute(
//    "projects",
//    "projects",
//    new {controller = "projects", action = "list", id = UrlParameter.Optional},
//    new {httpMethod = new HttpMethodConstraint("GET")},
//    new[] {"Bowerbird.Web.Controllers"});

//routes.MapRoute(
//    "project-list",
//    "projects/{id}",
//    new {controller = "projects", action = "index", id = UrlParameter.Optional},
//    new {httpMethod = new HttpMethodConstraint("GET")},
//    new[] {"Bowerbird.Web.Controllers"});

//routes.MapRoute(
//    "project-update",
//    "projects/{id}",
//    new {controller = "projects", action = "update"},
//    new {authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("PUT")},
//    new[] {"Bowerbird.Web.Controllers"});

//routes.MapRoute(
//    "project-delete",
//    "projects/{id}",
//    new {controller = "projects", action = "delete"},
//    new {authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("DELETE")},
//    new[] {"Bowerbird.Web.Controllers"});

//routes.MapRoute(
//    "project-create",
//    "projects/",
//    new {controller = "projects", action = "create"},
//    new {authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("POST")},
//    new[] {"Bowerbird.Web.Controllers"});

//routes.MapRoute(
//    "teams",
//    "teams",
//    new {controller = "teams", action = "list", id = UrlParameter.Optional},
//    new {httpMethod = new HttpMethodConstraint("GET")},
//    new[] {"Bowerbird.Web.Controllers"});

//routes.MapRoute(
//    "team-list",
//    "teams/{id}",
//    new {controller = "teams", action = "list", id = UrlParameter.Optional},
//    new {httpMethod = new HttpMethodConstraint("GET")},
//    new[] {"Bowerbird.Web.Controllers"});

//routes.MapRoute(
//    "team-update",
//    "teams/{id}",
//    new {controller = "teams", action = "update"},
//    new {authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("PUT")},
//    new[] {"Bowerbird.Web.Controllers"});

//routes.MapRoute(
//    "team-delete",
//    "teams/{id}",
//    new {controller = "teams", action = "delete"},
//    new {authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("DELETE")},
//    new[] {"Bowerbird.Web.Controllers"});

//routes.MapRoute(
//    "team-create",
//    "teams/",
//    new {controller = "teams", action = "create"},
//    new {authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("POST")},
//    new[] {"Bowerbird.Web.Controllers"});

//routes.MapRoute(
//    "organisations",
//    "organisations",
//    new {controller = "organisations", action = "list", id = UrlParameter.Optional},
//    new {httpMethod = new HttpMethodConstraint("GET")},
//    new[] {"Bowerbird.Web.Controllers"});

//routes.MapRoute(
//    "organisation-list",
//    "organisations/{id}",
//    new {controller = "organisations", action = "list", id = UrlParameter.Optional},
//    new {httpMethod = new HttpMethodConstraint("GET")},
//    new[] {"Bowerbird.Web.Controllers"});

//routes.MapRoute(
//    "organisation-update",
//    "organisations/{id}",
//    new {controller = "organisations", action = "update"},
//    new {authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("PUT")},
//    new[] {"Bowerbird.Web.Controllers.Members"});

//routes.MapRoute(
//    "organisation-delete",
//    "organisation/{id}",
//    new {controller = "organisatiosn", action = "delete"},
//    new {authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("DELETE")},
//    new[] {"Bowerbird.Web.Controllers"});

//routes.MapRoute(
//    "organisation-create",
//    "organisations/",
//    new {controller = "organisations", action = "create"},
//    new {authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("POST")},
//    new[] {"Bowerbird.Web.Controllers"});

////routes.MapRoute(
////    "MembersDefault",
////    "{controller}/{action}/{id}",
////    new {action = "index", id = UrlParameter.Optional},
////    new[] {"Bowerbird.Web.Controllers"});

#endregion