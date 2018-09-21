﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
using System.Management.Automation;
using Microsoft.Azure.Commands.EventHub.Models;

namespace Microsoft.Azure.Commands.EventHub.Commands.EventHub
{
    /// <summary>
    /// 'Remove-AzureRmEventHub' Cmdlet removes the specified EventHub
    /// </summary>
    [Cmdlet("Remove", ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "EventHubVNetRule", DefaultParameterSetName = VnetRulePropertiesParameterSet, SupportsShouldProcess = true), OutputType(typeof(bool))]
    public class RemoveAzureEventHubVNetRule : AzureEventHubsCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = VnetRulePropertiesParameterSet, Position = 0, HelpMessage = "Resource Group Name")]
        [ResourceGroupCompleter]
        [ValidateNotNullOrEmpty]
         public string ResourceGroupName { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = VnetRulePropertiesParameterSet, Position = 1, HelpMessage = "Namespace Name")]
        [ValidateNotNullOrEmpty]
        [Alias(AliasNamespaceName)]
        public string Namespace { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = VnetRulePropertiesParameterSet, Position = 2, HelpMessage = "Virtual Network Rule Name")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = VnetRuleInputObjectParameterSet, ValueFromPipeline = true, Position = 0, HelpMessage = "Virtual Network Rule Object")]
        [ValidateNotNullOrEmpty]
        public PSIpFilterRuleAttributes InputObject { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = VnetRuleResourceIdParameterSet, ValueFromPipelineByPropertyName = true, Position = 0, HelpMessage = "Virtual Network Rule Resource Id")]
        [ValidateNotNullOrEmpty]
        public string ResourceId { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter PassThru { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Run cmdlet in the background")]
        public SwitchParameter AsJob { get; set; }

        public override void ExecuteCmdlet()
        {

            if (ParameterSetName.Equals(VnetRuleInputObjectParameterSet))
            {
                LocalResourceIdentifier identifier = new LocalResourceIdentifier(InputObject.Id);
                ResourceGroupName = identifier.ResourceGroupName;
                Namespace = identifier.ParentResource;
                Name = identifier.ResourceName;
            }
            else if (ParameterSetName.Equals(VnetRuleResourceIdParameterSet))
            {
                LocalResourceIdentifier identifier = new LocalResourceIdentifier(ResourceId);
                ResourceGroupName = identifier.ResourceGroupName;
                Namespace = identifier.ParentResource;
                Name = identifier.ResourceName;
            }

            // delete a Ip Filter Rule 
            if (ShouldProcess(target:Name, action:string.Format(Resources.RemoveIpFilterRule,Name,Namespace)))
            {
                try
                {
                    var result = Client.DeleteVNetRule(ResourceGroupName, Namespace, Name);

                    if (PassThru.IsPresent)
                    {
                        WriteObject(result);
                    }
                }
                catch (Management.EventHub.Models.ErrorResponseException ex)
                {
                    WriteError(Eventhub.EventHubsClient.WriteErrorforBadrequest(ex));
                }
            }            
        }
    }
}
