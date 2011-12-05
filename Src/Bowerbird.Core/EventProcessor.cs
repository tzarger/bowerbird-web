﻿using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Bowerbird.Core.Events;
using Bowerbird.Core.EventHandlers;

namespace Bowerbird.Core
{
    public class EventProcessor
	{
        [ThreadStatic]
        private static List<Delegate> actions;

        public static IServiceLocator ServiceLocator { get; set; }

        public static void ClearCallbacks()
        {
            actions = null;
        }

        public static void Raise<T>(T args) where T : IDomainEvent
        {
            if (ServiceLocator != null)
            {
                foreach (var handler in ServiceLocator.GetAllInstances<IEventHandler<T>>())
                {
                    handler.Handle(args);
                }
            }

            if (actions != null)
            {
                foreach (var action in actions)
                {
                    if (action is Action<T>)
                    {
                       ((Action<T>)action)(args);
                    }
                }
            }
        }

        public static void Register<T>(Action<T> callback) where T : IDomainEvent
        {
            if (actions == null)
            {
                actions = new List<Delegate>();
            }
            actions.Add(callback);
        }
    }
}