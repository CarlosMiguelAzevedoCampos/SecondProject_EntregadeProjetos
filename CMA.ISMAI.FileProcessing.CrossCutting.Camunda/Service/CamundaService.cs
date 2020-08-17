using CamundaClient;
using CamundaClient.Dto;
using CMA.ISMAI.Core.Interface;
using CMA.ISMAI.Core.Model;
using CMA.ISMAI.Delivery.FileProcessing.CrossCutting.Camunda.Interface;
using CMA.ISMAI.Delivery.FileProcessing.Domain.Models;
using CMA.ISMAI.Delivery.Logging.Interface;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.FileProcessing.CrossCutting.Camunda.Service
{
    public class CamundaService : ICamundaService
    {
        private readonly CamundaEngineClient camundaEngineClient;
        private readonly string filePath;
        private Timer pollingTimer;
        private readonly IDictionary<string, Action<ExternalTask>> workers;
        private readonly IMediator _mediator;
        private readonly INotificationService _notificationService;
        private readonly ILoggingService _log;
        private readonly IConfiguration _config;

        public CamundaService(IMediator mediator, INotificationService notificationService, ILoggingService log, IConfiguration config)
        {
            camundaEngineClient = new CamundaEngineClient(new System.Uri(_config.GetSection("Camunda:Uri").Value), null, null);
            filePath = $"CMA.ISMAI.Delivery.FileProcessing.CrossCutting.Camunda.WorkFlow.FileProcessingISMAI.bpmn";
            workers = new Dictionary<string, Action<ExternalTask>>();
            _mediator = mediator;
            _notificationService = notificationService;
            _log = log;
            _config = config;
        }

        public void RegistWorkers()
        {
            registerWorker("generate_watermark", async externalTask =>
            {
                try
                {
                    var delivery = externalTask.Variables;
                    var getInstituteName = returnVariableValue(delivery, "instituteName");
                    var getCourseName = returnVariableValue(delivery, "courseName"); 
                    var getDeliveryTime = returnVariableValue(delivery, "deliveryTime");
                    var getStudentNumber = returnVariableValue(delivery, "studentNumber");
                    var getpublicdefenition = returnVariableValue(delivery, "publicPDFVersionName");
                    var getprivatedefenition = returnVariableValue(delivery, "privatePDFVersionName");
                    var getPath = returnVariableValue(delivery, "deliveryPath");

                    var generateWaterMark = new GenerateWaterMarkCommand(getStudentNumber.Value.ToString(), getPath.Value.ToString(), getCourseName.Value.ToString(), getInstituteName.Value.ToString(), DateTime.Parse(getDeliveryTime.Value.ToString()),
                        getpublicdefenition.Value.ToString(), getprivatedefenition.Value.ToString());

                    var result = await _mediator.Send(generateWaterMark);

                    Dictionary<string, object> dictionaryToPassVariable = returnDictionary(delivery);
                    dictionaryToPassVariable.Add("ok", result.IsValid);
                    dictionaryToPassVariable.Add("Worker", "generate_watermark");
                    camundaEngineClient.ExternalTaskService.Complete("FileProcessingISMAI", externalTask.Id, dictionaryToPassVariable);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    _log.Fatal(ex.ToString());
                }
            });

            registerWorker("generate_cover", async externalTask =>
            {
                try
                {
                    var delivery = externalTask.Variables;
                    var getStudentName = returnVariableValue(delivery, "studentName");
                    var getCordenatorName = returnVariableValue(delivery, "cordenatorName");
                    var getTitle = returnVariableValue(delivery, "title");
                    var getDefenition = returnVariableValue(delivery, "defenitionOfDelivery");
                    var getPath = returnVariableValue(delivery, "deliveryPath");

                    var generateCoverPageCommand = new GenerateCoverPageCommand(getStudentName.Value.ToString(), getCordenatorName.Value.ToString(),getDefenition.Value.ToString(),getTitle.Value.ToString(), getPath.Value.ToString());


                    var result = await _mediator.Send(generateCoverPageCommand);

                    Dictionary<string, object> dictionaryToPassVariable = returnDictionary(delivery);
                    dictionaryToPassVariable["ok"] = result.IsValid;
                    dictionaryToPassVariable["Worker"] = "generate_cover";
                    camundaEngineClient.ExternalTaskService.Complete("FileProcessingISMAI", externalTask.Id, dictionaryToPassVariable);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    _log.Fatal(ex.ToString());
                }
            });

            registerWorker("generate_jurypage", async externalTask =>
            {
                try
                {
                    var delivery = externalTask.Variables;
                    var getStudentName = returnVariableValue(delivery, "studentName");
                    var getInstituteName = returnVariableValue(delivery, "instituteName");
                    var getCourseName = returnVariableValue(delivery, "courseName");
                    var getStudentNumber = returnVariableValue(delivery, "studentNumber");
                    var getPath = returnVariableValue(delivery, "deliveryPath");

                    var generateJuryPageCommand = new GenerateJuryPageCommand(getStudentName.Value.ToString(), getStudentNumber.Value.ToString(), getCourseName.Value.ToString(), getInstituteName.Value.ToString(), getPath.Value.ToString());


                    var result = await _mediator.Send(generateJuryPageCommand);

                    Dictionary<string, object> dictionaryToPassVariable = returnDictionary(delivery);
                    dictionaryToPassVariable["ok"] = result.IsValid;
                    _log.Info($"Jury page was generated? {result.IsValid}");
                    dictionaryToPassVariable["Worker"] = "generate_jurypage";
                    camundaEngineClient.ExternalTaskService.Complete("FileProcessingISMAI", externalTask.Id, dictionaryToPassVariable);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    _log.Fatal(ex.ToString());
                }
            });


            registerWorker("send_to_manual", externalTask =>
            {
                try
                {
                    var delivery = externalTask.Variables;
                    var getStudentName = returnVariableValue(delivery, "studentName");
                    var getInstituteName = returnVariableValue(delivery, "instituteName");
                    var getStudentEmail = returnVariableValue(delivery, "studentEmail");
                    var getCourseName = returnVariableValue(delivery, "courseName");
                    var getStudentNumber = returnVariableValue(delivery, "studentNumber");
                    var getWorker = returnVariableValue(delivery, "worker");

                    _notificationService.SendEmail(_config.GetSection("Notification:Email").Value, string.Format("Hello, <br/> Something went wrong on the delivery. <br/> <br/> The delivery failed on the Payment diagram.<br/> <br/> Student Name:{0}, Institution Name: {1}, Student Number:{2}, Course Name:{3}. <br/> <br/> It failed on the {4} phase. <br/> <br/> Thanks",
                        getStudentName.Value.ToString(), getInstituteName.Value.ToString(), getStudentNumber.Value.ToString(), getCourseName.Value.ToString(), getWorker.Value.ToString()));
                    Dictionary<string, object> dictionaryToPassVariable = returnDictionary(delivery);
                    camundaEngineClient.ExternalTaskService.Complete("FileProcessingISMAI", externalTask.Id, dictionaryToPassVariable);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    _log.Fatal(ex.ToString());
                }
            });

            pollingTimer = new Timer(_ => StartPolling(), null, 1, Timeout.Infinite);

        }

        private Dictionary<string, object> returnDictionary(Dictionary<string, Variable> delivery)
        {
            Dictionary<string, object> valuesforNextIteration = new Dictionary<string, object>();
            foreach (var item in delivery)
                valuesforNextIteration.Add(item.Key, item.Value.Value);
            return valuesforNextIteration;
        }

        public Variable returnVariableValue(Dictionary<string, Variable> externalTaskVariables, string key)
        {
            var delivery = externalTaskVariables;
            var getValue = new Variable();
            delivery.TryGetValue(key, out getValue);
            return getValue;
        }

        private void StartPolling()
        {
            PollTasks();
            pollingTimer.Change(1, Timeout.Infinite);
        }

        private void PollTasks()
        {
            try
            {
                var tasks = camundaEngineClient.ExternalTaskService.FetchAndLockTasks("FileProcessingISMAI", 1000000, workers.Keys, 30000, null);
                Parallel.ForEach(
                    tasks,
                    new ParallelOptions { MaxDegreeOfParallelism = 1 },
                    (externalTask) =>
                    {
                        workers[externalTask.TopicName](externalTask);
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                _log.Fatal(ex.ToString());
            }
        }

        private void registerWorker(string topicName, Action<ExternalTask> action)
        {
            workers.Add(topicName, action);
        }


        public bool StartWorkFlow(DeliveryFileSystem delivery)
        {

            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters = AddValuesToTheDictionary(parameters, delivery);
                FileParameter file = FileParameter.FromManifestResource(Assembly.GetExecutingAssembly(), filePath);
                string deployId = camundaEngineClient.RepositoryService.Deploy("FileProcessingISMAI", new List<object> {
                    file });
                if (TheDeployWasDone(deployId))
                {
                    camundaEngineClient.BpmnWorkflowService.StartProcessInstance("FileProcessingISMAI", parameters);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                _log.Fatal(ex.ToString());
            }
            return false;
        }

        private bool TheDeployWasDone(string deployId)
        {
            return deployId != string.Empty;
        }



        private Dictionary<string, object> AddValuesToTheDictionary(Dictionary<string, object> parameters, Core.Model.DeliveryFileSystem message)
        {
            foreach (PropertyInfo propertyInfo in message.GetType().GetProperties())
                parameters.Add(propertyInfo.Name, propertyInfo.GetValue(message, null));
            parameters.Add("download", message.GetType() == typeof(Core.Model.DeliveryWithLink));

            return parameters;
        }
    }
}
