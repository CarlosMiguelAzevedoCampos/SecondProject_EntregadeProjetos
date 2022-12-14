using CamundaClient;
using CamundaClient.Dto;
using CMA.ISMAI.Core.Interface;
using CMA.ISMAI.Core.Model;
using CMA.ISMAI.Delivery.FileProcessing.CrossCutting.Camunda.Interface;
using CMA.ISMAI.Delivery.FileProcessing.Domain.Models;
using CMA.ISMAI.Delivery.FileProcessing.Domain.Models.Commands;
using CMA.ISMAI.Delivery.Logging.Interface;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
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
        private IDictionary<string, Action<ExternalTask>> workers;
        private readonly IMediator _mediator;
        private readonly INotificationService _notificationService;
        private readonly ILoggingService _log;
        private readonly IConfiguration _config;

        public CamundaService(IMediator mediator, INotificationService notificationService, ILoggingService log)
        {
            _config = new ConfigurationBuilder()
                                                      .SetBasePath(Directory.GetCurrentDirectory()) // Directory where the json files are located
                                                      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                                      .AddEnvironmentVariables()
                                                      .Build();
            camundaEngineClient = new CamundaEngineClient(new System.Uri(_config.GetSection("Camunda:Uri").Value), null, null);
            filePath = $"CMA.ISMAI.Delivery.FileProcessing.CrossCutting.Camunda.WorkFlow.FileProcessingISMAI.bpmn";
            workers = new Dictionary<string, Action<ExternalTask>>();
            _mediator = mediator;
            _notificationService = notificationService;
            _log = log;
        }

        public void RegistWorkers()
        {
            workers = new Dictionary<string, Action<ExternalTask>>();

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

                    var generateCoverPageCommand = new GenerateCoverPageCommand(getStudentName.Value.ToString(), getCordenatorName.Value.ToString(),getDefenition.Value.ToString(),getTitle.Value.ToString(), getPath.Value.ToString(), _config.GetSection("Logo:Path").Value);


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

                    var generateJuryPageCommand = new GenerateJuryPageCommand(getStudentName.Value.ToString(), getStudentNumber.Value.ToString(), getCourseName.Value.ToString(), getInstituteName.Value.ToString(), getPath.Value.ToString(), _config.GetSection("FilePathJury:Path").Value);


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

            registerWorker("send_to_cloud", async externalTask =>
            {
                try
                {
                    var delivery = externalTask.Variables;
                    var getPath = returnVariableValue(delivery, "deliveryPath");
                    var getStudentEmail = returnVariableValue(delivery, "studentEmail");
                    var getStudentName = returnVariableValue(delivery, "studentName");
                    var getInstituteName = returnVariableValue(delivery, "instituteName");
                    var getCourseName = returnVariableValue(delivery, "courseName");
                    var getStudentNumber = returnVariableValue(delivery, "studentNumber");

                    var generateJuryPageCommand = new FileTransferCommand(getPath.Value.ToString(), _config.GetSection("OneDrive:Path").Value, getStudentEmail.Value.ToString(), getStudentNumber.Value.ToString());


                    var result = await _mediator.Send(generateJuryPageCommand);

                    if (result.IsValid)
                    {
                        _notificationService.SendEmail(_config.GetSection("Notification:Email").Value, string.Format("Olá, <br/> Uma entrega irá para o OneDrive da Instituição. A informação do Aluno é <br/> <br/> Nome:{0}, Instituição: {1}, Número de estudante:{2}, Curso:{3}. <br/> <br/> Obrigado",
                        getStudentName.Value.ToString(), getInstituteName.Value.ToString(), getStudentNumber.Value.ToString(), getCourseName.Value.ToString()));

                        _notificationService.SendEmail(getStudentEmail.Value.ToString(), string.Format("Olá, <br/> A tua entrega irá para o OneDrive da Instituição. Para mais informação, contacta a tua Instituição. Obrigado",
                        getStudentName.Value.ToString(), getInstituteName.Value.ToString(), getStudentNumber.Value.ToString(), getCourseName.Value.ToString()));
                    }
                    Dictionary<string, object> dictionaryToPassVariable = returnDictionary(delivery);
                    dictionaryToPassVariable["ok"] = result.IsValid;
                    dictionaryToPassVariable["Worker"] = "send_to_cloud";
                    camundaEngineClient.ExternalTaskService.Complete("FileProcessingISMAI", externalTask.Id, dictionaryToPassVariable);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    _log.Fatal(ex.ToString());
                }
            });



            registerWorker("manual_processing_processing", externalTask =>
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

                    _notificationService.SendEmail(_config.GetSection("Notification:Email").Value, string.Format("Olá, <br/> Algo correu mal na entrega. <br/> <br/> A entrega falhou no diagrama FileProcessing.<br/> <br/> Nome:{0}, Instituição: {1}, Número de estudante:{2}, Curso:{3}. <br/> <br/> Falhou na fase {4} . <br/> <br/> Obrigado",
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

            pollingTimer = new Timer(_ => StartPolling(), null, Convert.ToInt64(_config.GetSection("TimeToFetch:Time").Value), Timeout.Infinite);

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
            pollingTimer.Change(Convert.ToInt64(_config.GetSection("TimeToFetch:Time").Value), Timeout.Infinite);
        }

        private void PollTasks()
        {
            try
            {
                var tasks = camundaEngineClient.ExternalTaskService.FetchAndLockTasks("FileProcessingISMAI", Convert.ToInt32(_config.GetSection("TaskPerFetch:Tasks").Value), workers.Keys, long.Parse((Convert.ToInt64(_config.GetSection("TimeToFetch:Time").Value) / 2).ToString()), null);
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


        public bool StartWorkFlow(Core.Model.Delivery delivery)
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



        private Dictionary<string, object> AddValuesToTheDictionary(Dictionary<string, object> parameters, Core.Model.Delivery message)
        {
            foreach (PropertyInfo propertyInfo in message.GetType().GetProperties())
                parameters.Add(propertyInfo.Name, propertyInfo.GetValue(message, null));
            parameters.Add("download", message.GetType() == typeof(Core.Model.DeliveryWithLink));

            return parameters;
        }
    }
}
