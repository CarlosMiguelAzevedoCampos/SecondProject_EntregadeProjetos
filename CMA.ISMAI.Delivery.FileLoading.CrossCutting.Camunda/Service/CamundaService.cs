using CamundaClient;
using CamundaClient.Dto;
using CMA.ISMAI.Delivery.FileLoading.CrossCutting.Camunda.Interface;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CMA.ISMAI.Delivery.FileLoading.CrossCutting.Camunda.Service
{
    public class CamundaService : ICamundaService
    {
        private readonly CamundaEngineClient camundaEngineClient;
        private readonly string filePath;

        public CamundaService()
        {
            camundaEngineClient = new CamundaEngineClient(new System.Uri("http://localhost:8080/engine-rest/engine/default/"), null, null);
            filePath = $"CMA.ISMAI.Delivery.FileLoading.CrossCutting.Camunda.WorkFlow.FileLoadingISMAI.bpmn";

        }

        public bool StartWorkFlow(Core.Model.Delivery delivery)
        {
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters = AddValuesToTheDictionary(parameters, delivery);
                FileParameter file = FileParameter.FromManifestResource(Assembly.GetExecutingAssembly(), filePath);
                string deployId = camundaEngineClient.RepositoryService.Deploy("FileLoading", new List<object> {
                    file });
                if (TheDeployWasDone(deployId))
                {
                    camundaEngineClient.BpmnWorkflowService.StartProcessInstance("FileLoading", parameters);
                }
                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        private Dictionary<string, object> AddValuesToTheDictionary(Dictionary<string, object> parameters, Core.Model.Delivery message)
        {
            foreach (PropertyInfo propertyInfo in message.GetType().GetProperties())
                parameters.Add(propertyInfo.Name, propertyInfo.GetValue(message, null));
            parameters.Add("download", message.GetType() == typeof(Core.Model.DeliveryWithLink));

            return parameters;
        }

        private bool TheDeployWasDone(string deployId)
        {
            return deployId != string.Empty;
        }

    }
}
