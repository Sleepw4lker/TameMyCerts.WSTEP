// Copyright © Uwe Gradenegger <uwe@gradenegger.eu>

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.ServiceModel;
using System.ServiceModel.Channels;

namespace TameMyCerts.WSTEP
{
    [ServiceContract(Namespace = "http://docs.oasis-open.org/ws-sx/ws-trust/200512")]
    public interface ISecurityTokenService
    {
        [OperationContract(
            Action = "http://schemas.microsoft.com/windows/pki/2009/01/enrollment/RST/wstep",
            ReplyAction = "http://schemas.microsoft.com/windows/pki/2009/01/enrollment/RSTRC/wstep",
            Name = "RequestSecurityTokenType")]
        Message RequestSecurityToken(Message message);
    }
}