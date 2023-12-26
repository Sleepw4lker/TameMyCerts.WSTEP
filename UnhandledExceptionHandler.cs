// Copyright (c) Uwe Gradenegger <info@gradenegger.eu>

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.ObjectModel;
using CoreWCF.Dispatcher;

namespace TameMyCerts.WSTEP;

public class UnhandledExceptionHandler : IErrorHandler
{
    private readonly ILogger<UnhandledExceptionHandler> logger;

    public UnhandledExceptionHandler(ILogger<UnhandledExceptionHandler> logger)
    {
        this.logger = logger;
    }

    public bool HandleError(Exception error)
    {
        return false;
    }

    public void ProvideFault(Exception ex, MessageVersion version, ref Message fault)
    {
        switch (ex)
        {
            case null:
                return;
            case FaultException:
                logger.LogWarning(ex, ex.Message);
                break;
            default:
                logger.LogError(ex, ex.Message);
                break;
        }
    }
}

public class UnhandledExceptionLoggingBehavior : IServiceBehavior
{
    private readonly ILogger<UnhandledExceptionHandler> logger;

    public UnhandledExceptionLoggingBehavior(ILogger<UnhandledExceptionHandler> logger)
    {
        this.logger = logger;
    }

    public void Validate(ServiceDescription description, ServiceHostBase serviceHostBase)
    {
    }

    public void AddBindingParameters(ServiceDescription description, ServiceHostBase serviceHostBase,
        Collection<ServiceEndpoint> endpoints, BindingParameterCollection parameters)
    {
    }

    public void ApplyDispatchBehavior(ServiceDescription description, ServiceHostBase serviceHostBase)
    {
        var errorHandler = (IErrorHandler)Activator.CreateInstance(typeof(UnhandledExceptionHandler), logger);

        foreach (var channelDispatcherBase in serviceHostBase.ChannelDispatchers)
        {
            var channelDispatcher = channelDispatcherBase as ChannelDispatcher;
            channelDispatcher.ErrorHandlers.Add(errorHandler);
        }
    }
}