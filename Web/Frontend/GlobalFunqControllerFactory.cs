﻿using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Funq;
using ServiceStack.ServiceHost;
using ServiceStack.Mvc;

namespace Web
{
    public class GlobalFunqControllerFactory : DefaultControllerFactory
    {
        private readonly ContainerResolveCache funqBuilder;

        public GlobalFunqControllerFactory(Container container)
        {
            this.funqBuilder = new ContainerResolveCache(container);
            
            // Also register all the controller types as transient
            var controllerTypes =
                AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(ass =>
                    {
                        try
                        {
                            return ass.GetTypes();
                        }
                        catch (ReflectionTypeLoadException ex)
                        {
                            return ex.Types.Where(t => t != null);
                        }
                    })
                    .Where(type =>
                    {
                        try
                        {
                            return typeof(IController).IsAssignableFrom(type);
                        }
                        catch (TypeLoadException ex)
                        {
                            return false;
                        }
                    })
                    .ToList();

            container.RegisterAutoWiredTypes(controllerTypes, ReuseScope.None);
        }

        protected override IController GetControllerInstance(
            RequestContext requestContext, Type controllerType)
        {
            try
            {
                if (controllerType == null)
                    return base.GetControllerInstance(requestContext, null);

                var controller = funqBuilder.CreateInstance(controllerType) as IController;

                return controller ?? base.GetControllerInstance(requestContext, controllerType);
            }
            catch (HttpException ex)
            {
                if (ex.GetHttpCode() == 404)
                {
                    try
                    {
                        if (ServiceStackController.CatchAllController != null)
                        {
                            return ServiceStackController.CatchAllController(requestContext);
                        }
                    }
                    catch { } //ignore not found CatchAllController
                }
                throw;
            }
        }
    }
}